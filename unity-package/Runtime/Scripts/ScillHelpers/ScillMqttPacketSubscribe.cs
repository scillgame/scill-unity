using UnityEngine.Assertions;

namespace ScillHelpers
{
    public class ScillMqttPacketSubscribe : ScillMqttPacketBase
    {
        public ushort PacketIdentifier;
        public string[] TopicFilter;
        public byte[] RequestedQoS;

        public ScillMqttPacketSubscribe()
        {
            CommandType = MqttCommandType.SUBSCRIBE;
        }

        public override byte[] ToBuffer()
        {
            Assert.AreEqual(TopicFilter.Length, RequestedQoS.Length);

            // calculate packet length
            {
                // start with length of packet identifier
                int varLength = 2;

                for (int i = 0; i < TopicFilter.Length; i++)
                {
                    // 2Bytes for the Topic Length + Number of characters in the topic + 1 Byte for QoS
                    varLength += TopicFilter[i].Length + 2 + 1;
                }

                RemainingLength = varLength;
            }
            Length = GetPacketLengthFromRemainingLength(RemainingLength);

            byte[] buffer = new byte[Length];
            int pointer = 0;

            // ----------- Write Header
            buffer[pointer] = GetNonPublishControlHeader(CommandType);
            pointer++;

            WriteRemainingLengthIntoBuffer(ref buffer, ref pointer, RemainingLength);


            // ---------- Variable Header ----------------

            // Write Packet Identifier into buffer
            WriteTwoByteNumberIntoBuffer(ref buffer, ref pointer, PacketIdentifier);


            // --------------- Payload ---------------

            for (int i = 0; i < TopicFilter.Length; i++)
            {
                int topicLength = TopicFilter[i].Length;
                // Write number of characters in topic into buffer
                WriteTwoByteNumberIntoBuffer(ref buffer, ref pointer, (ushort) topicLength);
                // Write topic into buffer
                WriteStringIntoBuffer(ref buffer, ref pointer, TopicFilter[i]);

                // Write QoS into buffer
                buffer[pointer] = RequestedQoS[i];
                pointer++;
            }

            return buffer;
        }
    }
}