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
    public delegate void MqttConnectionEstablishedHandler(ScillMqtt mqttClient);

    /// <summary>
    /// Wrapper for accessing and subscribing / unsubscribing to the SCILL Mqtt webhooks using Websockets. This is required in order to be
    /// WebGL compatible.
    /// </summary>
    public class ScillMqtt
    {
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Called once the Mqtt connection to the SCILL server was successfully established. Supplies the mqtt client instance
        /// which established the connection.
        /// </summary>
        public static event MqttConnectionEstablishedHandler OnMqttConnectionEstablished;

        private WebSocket _mqttWS;

        private ushort _currentPacketIdentifier = 0;

        private Dictionary<string, BattlePassChangedNotificationHandler> callbacksBattlePassChanged =
            new Dictionary<string, BattlePassChangedNotificationHandler>();

        private Dictionary<string, LeaderboardChangedNotificationHandler> callbacksLeaderboardChanged =
            new Dictionary<string, LeaderboardChangedNotificationHandler>();

        private Dictionary<string, ChallengeChangedNotificationHandler> callbacksPersonalChallengeChanged =
            new Dictionary<string, ChallengeChangedNotificationHandler>();


        /// <summary>
        /// Creates a websocket connection using the mqtt protocol to the SCILL mqtt server.
        /// Server address used: "wss://mqtt.scillgame.com:8083/mqtt"
        /// </summary>
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

        /// <summary>
        /// Send a keep alive message to avoid having the connection closed by the server due to timeout.
        /// </summary>
        public void Ping()
        {
            if (IsConnected)
            {
                ScillMqttPacketPing pingPacket = new ScillMqttPacketPing();
                pingPacket.Buffer = pingPacket.ToBuffer();
                _mqttWS.Send(pingPacket.Buffer);
            }
        }


        /// <summary>
        /// Required to be called in an update method of a Monobehaviour to ensure that Websocket messages are received by the game logic
        /// on the Main Thread. Only required on non-webgl builds, because only non-webgl builds use an async Websocket implementation.
        /// </summary>
        public void DispatchMessageQueue()
        {
#if !UNITY_WEBGL || UNITY_EDITOR

            _mqttWS.DispatchMessageQueue();
#endif
        }

        /// <summary>
        /// Close the mqtt connection.
        /// </summary>
        public void Close()
        {
            if (null != _mqttWS && (_mqttWS.State == WebSocketState.Open || _mqttWS.State == WebSocketState.Connecting))
            {
                _mqttWS.Close();
            }
        }

        private void MqttWSOnOnOpen()
        {
            // Debug.Log("TCP connection opened");
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
                HandleChallengeUpdate(publishPacket);
            }
            else if (callbacksBattlePassChanged.ContainsKey(publishPacket.TopicName))
            {
                HandleBattlePassUpdate(publishPacket);
            }else if (callbacksLeaderboardChanged.ContainsKey(publishPacket.TopicName))
            {
                HandleLeaderboardUpdate(publishPacket);
            }
        }

        private void HandleLeaderboardUpdate(ScillMqttPacketPublish publishPacket)
        {
            LeaderboardUpdatePayload payload =
                JsonConvert.DeserializeObject<LeaderboardUpdatePayload>(publishPacket.Payload);

            var callback = callbacksLeaderboardChanged[publishPacket.TopicName];
            if (null != callback)
            {
                callback.Invoke(payload);
            }
        }

        private void HandleBattlePassUpdate(ScillMqttPacketPublish publishPacket)
        {
            var jObject = (JObject) JsonConvert.DeserializeObject(publishPacket.Payload);
            if (jObject != null)
            {
                string webhookType = jObject["webhook_type"].Value<string>();
                // Debug.Log($"Webhooktype: {webhookType}");

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

        private void HandleChallengeUpdate(ScillMqttPacketPublish publishPacket)
        {
            ChallengeWebhookPayload payload =
                JsonConvert.DeserializeObject<ChallengeWebhookPayload>(publishPacket.Payload);

            var callback = callbacksPersonalChallengeChanged[publishPacket.TopicName];
            if (null != callback)
            {
                callback.Invoke(payload);
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

        /// <summary>
        /// Check whether a subscription to the given topic is active.
        /// </summary>
        /// <param name="topic">The topic to check.</param>
        /// <returns>True, if there already exists a subscription to the topic, false otherwise.</returns>
        public bool IsSubscriptionActive(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                return false;

            bool active = callbacksPersonalChallengeChanged.ContainsKey(topic);
            active |= callbacksBattlePassChanged.ContainsKey(topic);
            active |= callbacksLeaderboardChanged.ContainsKey(topic);
            return active;
        }


        /// <summary>
        /// Start a mqtt subscription to the given topic and notify the callback on BattlePass relevant publish messages.
        /// </summary>
        /// <param name="topic">Topic to subscribe to.</param>
        /// <param name="callback">Callback.</param>
        public void SubscribeToTopicBattlePass(string topic, BattlePassChangedNotificationHandler callback)
        {
            SubscribeToTopic(topic, callbacksBattlePassChanged, callback);
        }

        /// <summary>
        /// Start a mqtt subscription to the given topic and notify the callback on Leaderboard relevant publish messages.
        /// </summary>
        /// <param name="topic">Topic to subscribe to.</param>
        /// <param name="callback">Callback.</param>
        public void SubscribeToTopicLeaderboard(string topic, LeaderboardChangedNotificationHandler callback)
        {
            SubscribeToTopic(topic, callbacksLeaderboardChanged, callback);
        }

        /// <summary>
        /// Start a mqtt subscription to the given topic and notify the callback on Personal Challenge relevant publish messages.
        /// </summary>
        /// <param name="topic">Topic to subscribe to.</param>
        /// <param name="callback">Callback.</param>
        public void SubscribeToTopicChallenge(string topic, ChallengeChangedNotificationHandler callback)
        {
            SubscribeToTopic(topic, callbacksPersonalChallengeChanged, callback);
        }

        private void SubscribeToTopic<T>(string topic, Dictionary<string, T> callbacks, T callback) where T : Delegate
        {
            if (string.IsNullOrEmpty(topic))
                return;
            if (!IsSubscriptionActive(topic))
            {
                callbacks.Add(topic, callback);
                SubscribeToTopic(topic);
            }
        }

        /// <summary>
        /// Stop a mqtt subscription. 
        /// </summary>
        /// <param name="topic">Topic to unsubscribe from</param>
        public void UnsubscribeFromTopic(string topic)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                TryRemoveCallback(topic, callbacksBattlePassChanged);
                TryRemoveCallback(topic, callbacksPersonalChallengeChanged);
                TryRemoveCallback(topic, callbacksLeaderboardChanged);
                
                ScillMqttPacketUnsubscribe unsubscribe = new ScillMqttPacketUnsubscribe();
                unsubscribe.PacketIdentifier = ++_currentPacketIdentifier;
                unsubscribe.TopicFilter = new[] {topic};
                unsubscribe.Buffer = unsubscribe.ToBuffer();
                _mqttWS.Send(unsubscribe.Buffer);
            }
        }

        private void TryRemoveCallback<T>(string topic, Dictionary<string, T> fromDictionary)
        {
            if (!string.IsNullOrEmpty(topic))
                fromDictionary.Remove(topic);
        }

        private void SubscribeToTopic(string topic, byte qoS = 0)
        {
            // Debug.Log($"Requested subscription with topic: {topic}");
            ScillMqttPacketSubscribe subcribePacket = new ScillMqttPacketSubscribe();
            subcribePacket.PacketIdentifier = ++_currentPacketIdentifier;
            subcribePacket.TopicFilter = new[] {topic};
            subcribePacket.RequestedQoS = new[] {qoS};

            subcribePacket.Buffer = subcribePacket.ToBuffer();
            _mqttWS.Send(subcribePacket.Buffer);
        }
    }
}