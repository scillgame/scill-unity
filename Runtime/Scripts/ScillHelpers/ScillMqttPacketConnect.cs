using System;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace ScillHelpers
{
    [Flags]
    enum ScillMqttConnectFlags : byte
    {
        CLEAN_SESSION = 2,
        WILL = 4,
        WILL_QOS = 24,
        WILL_RETAIN = 32,
        PASSWORD = 64,
        USER_NAME = 128
    };

    class ScillMqttPacketConnect : ScillMqttPacketBase
    {
        public string ProtocolName = "MQTT";
        public byte ProtocolLevel = 4; // Protocol Level, version 3.1.1 is written as 4
        public byte ConnectFlags;
        public ushort KeepAlive;
        public bool WillRetain;
        public byte WillQoS;
        public bool CleanSession;
        public string ClientId = string.Empty;
        [CanBeNull] public string WillTopic;
        [CanBeNull] public string WillMessage;
        [CanBeNull] public string UserName;
        [CanBeNull] public string Password;

        public override byte[] ToBuffer()
        {
            CommandType = MqttCommandType.CONNECT;

            // calculate Lengths of package
            {
                // Number of bytes for protocol name length + protocol level + connect flags + keep alive
                int variableLength = 2 + 1 + 1 + 2;

                // add payload length
                variableLength += ProtocolName.Length;
                variableLength += ClientId.Length + 2; // Client Id has to be present
                variableLength += GetPayloadEntryLength(WillTopic);
                variableLength += GetPayloadEntryLength(WillMessage);
                variableLength += GetPayloadEntryLength(UserName);
                variableLength += GetPayloadEntryLength(Password);

                this.Length = GetPacketLengthFromRemainingLength(variableLength);
                this.RemainingLength = variableLength;
            }


            // ------------ Control Header + Remaining Length -------------

            byte[] buffer = new byte[this.Length];
            int pointer = 0;

            // write control header
            buffer[pointer] = GetNonPublishControlHeader(CommandType);
            pointer++;

            // write remaining length
            WriteRemainingLengthIntoBuffer(ref buffer, ref pointer, this.RemainingLength);


            // ------------ Start Variable Header -------------------

            // write protocol name length into two bytes
            WriteTwoByteLengthAndStringIntoBuffer(ref buffer, ref pointer, ProtocolName);


            // write protocol level (version 3.1.1 is written as 4)
            buffer[pointer] = ProtocolLevel;
            pointer++;

            // Write Connect Flags
            SetConnectFlags();
            buffer[pointer] = ConnectFlags;
            pointer++;

            // Write Keep Alive Duration
            WriteTwoByteNumberIntoBuffer(ref buffer, ref pointer, KeepAlive);


            // ------------ Start Writing Payload ---------------


            // Write Client Identifier 
            WriteTwoByteLengthAndStringIntoBuffer(ref buffer, ref pointer, ClientId);


            // Write Will Topic and message
            Assert.AreEqual((byte) (ScillMqttConnectFlags.WILL), (byte) 4);
            if ((ConnectFlags & (byte) ScillMqttConnectFlags.WILL) != 0)
            {
                WriteTwoByteLengthAndStringIntoBuffer(ref buffer, ref pointer, WillTopic);
                WriteTwoByteLengthAndStringIntoBuffer(ref buffer, ref pointer, WillMessage);
            }

            // Write User Name
            if ((ConnectFlags & (byte) ScillMqttConnectFlags.USER_NAME) != 0)
            {
                WriteTwoByteLengthAndStringIntoBuffer(ref buffer, ref pointer, UserName);
            }

            if ((ConnectFlags & (byte) ScillMqttConnectFlags.PASSWORD) != 0)
            {
                WriteTwoByteLengthAndStringIntoBuffer(ref buffer, ref pointer, Password);
            }

            return buffer;
        }

        

        void SetConnectFlags()
        {
            // set user name flag
            ConnectFlags = (byte) (null != UserName ? ScillMqttConnectFlags.USER_NAME : 0);
            // set password flag
            ConnectFlags += (byte) (null != Password && null != UserName ? ScillMqttConnectFlags.PASSWORD : 0);
            // set "will retain" flag
            ConnectFlags += (byte) (WillRetain && null != WillMessage && null != WillTopic
                ? ScillMqttConnectFlags.WILL_RETAIN
                : 0);
            // set "will QoS" flags (2bit)
            ConnectFlags += (byte) (null != WillMessage && null != WillTopic ? WillQoS % 4 << 3 : 0);
            // set "Will Flag"
            ConnectFlags += (byte) (null != WillMessage && null != WillTopic ? ScillMqttConnectFlags.WILL : 0);
            // set "Clean Session" flag
            ConnectFlags += (byte) (CleanSession ? ScillMqttConnectFlags.CLEAN_SESSION : 0);
        }
    }
}