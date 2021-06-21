using System;
using JetBrains.Annotations;
using NativeWebSocket;
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

    public delegate void ChallengeChangedNotificationHandler(ChallengeWebhookPayload payload);

    public delegate void BattlePassChangedNotificationHandler(BattlePassChallengeChangedPayload payload);

    public class ScillMqtt
    {
        private WebSocket _mqttWS;

        ScillMqtt()
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
            if (null != _mqttWS && (_mqttWS.State == WebSocketState.Open || _mqttWS.State == WebSocketState.Connecting))
            {
                _mqttWS.Close();
            }
        }

        private void MqttWSOnOnClose(WebSocketCloseCode closecode)
        {
            throw new System.NotImplementedException();
        }

        private void MqttWSOnOnMessage(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        private void MqttWSOnOnError(string errormsg)
        {
            Debug.LogError("Could not establish connection to MQTT Server: " + errormsg);
        }

        private void MqttWSOnOnOpen()
        {
        }

        public void SubscribeToTopicBattlePass(string Topic, BattlePassChangedNotificationHandler callback)
        {
        }

        public void SubscribeToTopicChallenge(string Topic, ChallengeChangedNotificationHandler callback)
        {
        }

        public void SubscribeToTopic(string Topic)
        {
        }
    }

    enum MqttCommandType : byte
    {
        CONNECT = 1,
        CONNACK = 2,
        PUBLISH = 3,
        PUBACK = 4,
        PUBREC = 5,
        PUBREL = 6,
        PUBCOMP = 7,
        SUBSCRIBE = 8,
        SUBACK = 9,
        UNSUBSCRIBE = 10,
        UNSUBACK = 11,
        PINGREQ = 12,
        PINGRESP = 13,
        DISCONNECT = 14
    };

    class ScillMqttPacketBase
    {
        public MqttCommandType PacketType;
        public byte PacketFlags;
        public byte[] Buffer;

        public int RemainingLength;
        public int Length;

        public virtual byte[] ToBuffer()
        {
            return new byte[0];
        }

        public static ulong GetRemainingLengthFromBuffer(byte[] buffer)
        {
            return 0;
        }

        public static ulong CalculateLengthFromRemaining(ulong remainingLength)
        {
            return 0;
        }

        public static byte FixedHeaderLengthFromRemaining(ulong remainingLength)
        {
            return 0;
        }

        public static int GetNumBytesRequiredForRemainingLength(int varlength)
        {
            return varlength < 128 ? 1 : varlength < 16384 ? 2 : varlength < 2097152 ? 3 : 4;
        }

        public static byte GetControlHeader(MqttCommandType commandType)
        {
            // TODO: Adjust for Control Flags when using Subscribe or unsubscribe
            return (byte) (Convert.ToByte(commandType) << 4);
        }
    }


    class ScillMqttPacketConnect : ScillMqttPacketBase
    {
        public override byte[] ToBuffer()
        {
            PacketType = MqttCommandType.CONNECT;

            // calculate Lengths of package
            {
                // Number of bytes for protocol name length + protocol level + connect flags + keep alive
                int variableLength = 2 + 1 + 1 + 2;

                // add payload length
                variableLength += ProtocolName.Length;
                variableLength += GetPayloadEntryLength(WillTopic);
                variableLength += GetPayloadEntryLength(WillMessage);
                variableLength += GetPayloadEntryLength(UserName);
                variableLength += GetPayloadEntryLength(Password);

                int packageLength = 1; // fixed header without remaining length
                packageLength += GetNumBytesRequiredForRemainingLength(variableLength);
                packageLength += variableLength;

                this.Length = packageLength;
                this.RemainingLength = variableLength;
            }


            // start filling the package

            byte[] buffer = new byte[this.Length];
            int pointer = 0;

            // write control header
            byte controlHeader = GetControlHeader(PacketType);
            buffer[pointer] = controlHeader;
            pointer++;

            // write remaining length
            {
                int remainingLength = this.RemainingLength;
                do
                {
                    buffer[pointer] = (byte) (remainingLength % 128);
                    remainingLength = remainingLength / 128;
                    if (remainingLength > 0)
                        remainingLength += 128; // add MSB to signal that the remaining length is continued
                    pointer++;
                } while (remainingLength > 0);
            }

            return buffer;
        }

        /// <summary>
        /// If the entry is not null, always reserve additional
        /// two bytes for setting the length of the payload entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        int GetPayloadEntryLength(string entry)
        {
            return null != entry ? entry.Length + 2 : 0;
        }

        void SetConnectFlags()
        {
        }

        string ProtocolName = "MQTT";
        byte ProtocolLevel = 4;
        byte ConnectFlags;
        ushort KeepAlive;
        bool WillRetain;
        byte WillQoS;
        bool CleanSession;
        string ClientId;
        [CanBeNull] string WillTopic;
        [CanBeNull] string WillMessage;
        [CanBeNull] string UserName;
        [CanBeNull] string Password;
    };
}