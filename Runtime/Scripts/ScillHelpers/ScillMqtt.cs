using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Assertions;

namespace ScillHelpers
{
    enum BattlePassPayloadType
    {
        ChallengeChanged = 0,
        RewardClaimed = 1,
        Expired = 2
    };

    public delegate void ChallengeChangedNotificationHandler(ChallengeWebhookPayload payload);

    public delegate void BattlePassChangedNotificationHandler(BattlePassChallengeChangedPayload payload);

    public class ScillMqtt
    {
        private WebSocket _mqttWS;

        public bool IsConnected { get; private set; }

        private ushort CurrentPacketIdentifier = 0;

        private Dictionary<string, BattlePassChangedNotificationHandler> callbacksBattlePassChanged =
            new Dictionary<string, BattlePassChangedNotificationHandler>();

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
            // Debug.Log("Pinging");
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
            Debug.Log("Received Message");
            ScillMqttPacketBase packet = ScillMqttPacketBase.FromBuffer(data);
            if (MqttCommandType.CONNACK == packet.CommandType)
            {
                ScillMqttPacketConnack connackPacket = (ScillMqttPacketConnack) packet;
                if (ScillMqttConnackCode.ACCEPTED == connackPacket.Code)
                {
                    IsConnected = true;
                    Debug.Log("MQTT Connection Acknowledged and Accepted.");
                }
                else
                {
                    Debug.LogError("MQTT Connection refused with code: " + connackPacket.Code);
                }
            }
            else if (MqttCommandType.PUBLISH == packet.CommandType)
            {
                Debug.Log("Message identified as publish message");
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

                        switch (webhookType)
                        {
                            case "battlepass-challenge-changed":
                                break;
                            case "battlepass-level-reward-claimed":
                                break;
                            case "battlepass-expired":
                                break;
                        }
                    }
                }
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
            return null != topic &&
                   (callbacksBattlePassChanged.ContainsKey(topic) ||
                    callbacksPersonalChallengeChanged.ContainsKey(topic));
        }


        public void SubscribeToTopicBattlePass(string topic, BattlePassChangedNotificationHandler callback)
        {
            callbacksBattlePassChanged.Add(topic, callback);
            SubscribeToTopic(topic);
        }

        public void SubscribeToTopicChallenge(string topic, ChallengeChangedNotificationHandler callback)
        {
            callbacksPersonalChallengeChanged.Add(topic, callback);
            SubscribeToTopic(topic);
        }

        public void SubscribeToTopic(string topic, byte qoS = 0)
        {
            ScillMqttPacketSubscribe subcribePacket = new ScillMqttPacketSubscribe();
            subcribePacket.PacketIdentifier = ++CurrentPacketIdentifier;
            subcribePacket.TopicFilter = new[] {topic};
            subcribePacket.RequestedQoS = new[] {qoS};

            subcribePacket.Buffer = subcribePacket.ToBuffer();
            _mqttWS.Send(subcribePacket.Buffer);
        }
    }
}