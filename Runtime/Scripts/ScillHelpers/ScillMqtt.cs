using System;
using System.Collections.Generic;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCILL;
using SCILL.Model;
using UnityEngine;

namespace ScillHelpers
{
    enum BattlePassPayloadType
    {
        ChallengeChanged = 0,
        RewardClaimed = 1,
        Expired = 2
    };


    public delegate void MqttConnectionEstablishedHandler(ScillMqtt mqttClient);

    public class ScillMqtt
    {
        public bool IsConnected { get; private set; }

        public event MqttConnectionEstablishedHandler OnMqttConnectionEstablished;

        private WebSocket _mqttWS;

        private ushort CurrentPacketIdentifier = 0;

        private Dictionary<string, BattlePassChangedNotificationHandler> callbacksBattlePassChanged =
            new Dictionary<string, BattlePassChangedNotificationHandler>();

        private Dictionary<string, LeaderboardChangedNotificationHandler> callbacksLeaderboardChanged =
            new Dictionary<string, LeaderboardChangedNotificationHandler>();

        private Dictionary<string, ChallengeChangedNotificationHandler> callbacksPersonalChallengeChanged =
            new Dictionary<string, ChallengeChangedNotificationHandler>();

        public ScillMqtt()
        {
            _mqttWS = new WebSocket("wss://mqtt.scillgame.com:8083/mqtt");

            _mqttWS.OnOpen += MqttWSOnOnOpen;
            _mqttWS.OnError += MqttWSOnOnError;
            _mqttWS.OnMessage += MqttWSOnOnMessage;
            _mqttWS.OnClose += MqttWSOnOnClose;

            _mqttWS.Connect();
        }

        ~ScillMqtt()
        {
            Close();
        }

        public void Ping()
        {
            if (IsConnected)
            {
                ScillMqttPacketPing pingPacket = new ScillMqttPacketPing();
                pingPacket.Buffer = pingPacket.ToBuffer();
                _mqttWS.Send(pingPacket.Buffer);
            }
        }


        public void DispatchMessageQueue()
        {
#if !UNITY_WEBGL || UNITY_EDITOR

            _mqttWS.DispatchMessageQueue();
#endif
        }

        public void Close()
        {
            if (null != _mqttWS && (_mqttWS.State == WebSocketState.Open || _mqttWS.State == WebSocketState.Connecting))
            {
                _mqttWS.Close();
            }
        }

        private void MqttWSOnOnOpen()
        {
            Debug.Log("TCP connection opened");
            ScillMqttPacketConnect connectPacket = new ScillMqttPacketConnect
            {
                KeepAlive = 300, WillRetain = false, WillQoS = 0, CleanSession = true
            };


            connectPacket.Buffer = connectPacket.ToBuffer();
            _mqttWS.Send(connectPacket.Buffer);
        }


        private void MqttWSOnOnMessage(byte[] data)
        {
            // Debug.Log("Received any message");
            ScillMqttPacketBase packet = ScillMqttPacketBase.FromBuffer(data);
            if (MqttCommandType.CONNACK == packet.CommandType)
            {
                HandleConnAckPacket(packet);
            }
            else if (MqttCommandType.PUBLISH == packet.CommandType)
            {
                HandlePublishPacket(packet);
            }
            else if (MqttCommandType.PINGRESP == packet.CommandType)
            {
                // Debug.Log("Got a ping response.");
            }
            else if (MqttCommandType.SUBACK == packet.CommandType)
            {
                // Debug.Log("Acknowledged subscription");
            }
            else if (MqttCommandType.UNSUBACK == packet.CommandType)
            {
                // Debug.Log("Acknowledged unsubscription");
            }
            else
            {
                // Debug.Log("Received Unhandled Mqtt Message");
            }
        }

        private void HandlePublishPacket(ScillMqttPacketBase packet)
        {
            // Debug.Log("Message identified as publish message");
            ScillMqttPacketPublish publishPacket = (ScillMqttPacketPublish) packet;
            if (callbacksPersonalChallengeChanged.ContainsKey(publishPacket.TopicName))
            {
                ChallengeWebhookPayload payload =
                    JsonConvert.DeserializeObject<ChallengeWebhookPayload>(publishPacket.Payload);

                var callback = callbacksPersonalChallengeChanged[publishPacket.TopicName];
                if (null != callback)
                {
                    callback.Invoke(payload);
                }
            }
            else if (callbacksBattlePassChanged.ContainsKey(publishPacket.TopicName))
            {
                var jObject = (JObject) JsonConvert.DeserializeObject(publishPacket.Payload);
                if (jObject != null)
                {
                    string webhookType = jObject["webhook_type"].Value<string>();
                    Debug.Log($"Webhooktype: {webhookType}");

                    BattlePassChallengeChangedPayload payload =
                        JsonConvert.DeserializeObject<BattlePassChallengeChangedPayload>(publishPacket.Payload);
                    BattlePassChangedNotificationHandler callback =
                        callbacksBattlePassChanged[publishPacket.TopicName];
                    if (null != callback)
                    {
                        callback.Invoke(payload);
                    }

                    // TODO: deserialize the payloads correctly according to webhook type
                    // switch (webhookType)
                    // {
                    //     case "battlepass-challenge-changed":
                    //         BattlePassChallengeChangedPayload payload =
                    //             JsonConvert.DeserializeObject<BattlePassChallengeChangedPayload>(publishPacket.Payload);
                    //         BattlePassChangedNotificationHandler callback =
                    //             callbacksBattlePassChanged[publishPacket.TopicName];
                    //         if (null != callback)
                    //         {
                    //             callback.Invoke(payload);
                    //         }
                    //         break;
                    //     case "battlepass-level-reward-claimed":
                    //         break;
                    //     case "battlepass-expired":
                    //         break;
                    // }
                }
            }
        }


        private void HandleConnAckPacket(ScillMqttPacketBase packet)
        {
            ScillMqttPacketConnack connackPacket = (ScillMqttPacketConnack) packet;
            if (ScillMqttConnackCode.ACCEPTED == connackPacket.Code)
            {
                IsConnected = true;
                // Debug.Log("Mqtt Connection Established.");
                OnMqttConnectionEstablished?.Invoke(this);
            }
            else
            {
                Debug.LogError("MQTT Connection refused with code: " + connackPacket.Code);
            }
        }


        private void MqttWSOnOnError(string errormsg)
        {
            Debug.LogError("ScillMqtt threw a Websocket Error: " + errormsg);
        }

        private void MqttWSOnOnClose(WebSocketCloseCode closecode)
        {
            IsConnected = false;
            Debug.Log("Closed connection to MQTT Server with code: " + closecode);
        }

        public bool IsSubscriptionActive(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                return false;

            bool active = callbacksPersonalChallengeChanged.ContainsKey(topic);
            active |= callbacksBattlePassChanged.ContainsKey(topic);
            active |= callbacksLeaderboardChanged.ContainsKey(topic);
            return active;
        }


        public void SubscribeToTopicBattlePass(string topic, BattlePassChangedNotificationHandler callback)
        {
            if (!IsSubscriptionActive(topic))
            {
                callbacksBattlePassChanged.Add(topic, callback);
                SubscribeToTopic(topic);
            }
        }

        public void SubscribeToTopicLeaderboard(string topic, LeaderboardChangedNotificationHandler callback)
        {
            if (!IsSubscriptionActive(topic))
            {
                callbacksLeaderboardChanged.Add(topic, callback);
                SubscribeToTopic(topic);
            }
        }

        public void SubscribeToTopicChallenge(string topic, ChallengeChangedNotificationHandler callback)
        {
            if (!IsSubscriptionActive(topic))
            {
                callbacksPersonalChallengeChanged.Add(topic, callback);
                SubscribeToTopic(topic);
            }
        }

        public void UnsubscribeFromTopic(string topic)
        {
            TryRemoveCallback(topic, callbacksBattlePassChanged);
            TryRemoveCallback(topic, callbacksPersonalChallengeChanged);
            ScillMqttPacketUnsubscribe unsubscribe = new ScillMqttPacketUnsubscribe();
            unsubscribe.PacketIdentifier = ++CurrentPacketIdentifier;
            unsubscribe.TopicFilter = new[] {topic};
            unsubscribe.Buffer = unsubscribe.ToBuffer();
            _mqttWS.Send(unsubscribe.Buffer);
        }

        private void TryRemoveCallback<T>(string topic, Dictionary<string, T> fromDictionary)
        {
            fromDictionary.Remove(topic);
        }

        private void SubscribeToTopic(string topic, byte qoS = 0)
        {
            // Debug.Log($"Requested subscription with topic: {topic}");
            ScillMqttPacketSubscribe subcribePacket = new ScillMqttPacketSubscribe();
            subcribePacket.PacketIdentifier = ++CurrentPacketIdentifier;
            subcribePacket.TopicFilter = new[] {topic};
            subcribePacket.RequestedQoS = new[] {qoS};

            subcribePacket.Buffer = subcribePacket.ToBuffer();
            _mqttWS.Send(subcribePacket.Buffer);
        }
    }
}