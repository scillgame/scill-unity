using System.Text;
using UnityEngine.Assertions;

namespace ScillHelpers
{
    public enum MqttCommandType : byte
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

    public class ScillMqttPacketBase
    {
        public MqttCommandType CommandType;
        public byte PacketControlFlags;
        public byte[] Buffer;

        public int RemainingLength;
        public int Length;

        public virtual byte[] ToBuffer()
        {
            return new byte[0];
        }

        public static ScillMqttPacketBase FromBuffer(byte[] buffer)
        {
            MqttCommandType commandType = GetCommandTypeFromBuffer(buffer);
            if (MqttCommandType.CONNACK == commandType)
            {
                return new ScillMqttPacketConnack(buffer);
            }

            if (MqttCommandType.PUBLISH == commandType)
            {
                return new ScillMqttPacketPublish(buffer);
            }

            var packet = new ScillMqttPacketBase();
            packet.CommandType = commandType;
            return packet;
        }

        public static MqttCommandType GetCommandTypeFromBuffer(byte[] buffer)
        {
            byte first = buffer[0];
            return (MqttCommandType) ((first & 0xf0) >> 4);
        }


        protected static int GetPacketLengthFromRemainingLength(int remainingLength)
        {
            return remainingLength + GetFixedHeaderLengthFromRemaining(remainingLength);
        }

        protected static int GetFixedHeaderLengthFromRemaining(int remainingLength)
        {
            int remainingLengthBytes = 1 + (remainingLength < 128 ? 1 :
                remainingLength < 16384 ? 2 :
                remainingLength < 2097152 ? 3 : 4);
            return remainingLengthBytes;
        }

        protected static ushort GetTwoByteNumberFromBuffer(byte[] buffer, ref int pointer)
        {
            ushort number = (ushort) (buffer[pointer] * 256);
            pointer++;
            number += buffer[pointer];
            pointer++;
            return number;
        }

        protected static string GetStringFromBuffer(byte[] buffer, ref int pointer, int stringLength)
        {
            string resultString = Encoding.UTF8.GetString(buffer, pointer, stringLength);
            pointer += stringLength;
            return resultString;
        }

        protected static int GetRemainingLengthFromBuffer(byte[] buffer)
        {
            int pointer = 1;

            int multiplier = 1;
            int length = 0;

            byte currentByte = 0;
            do
            {
                currentByte = buffer[pointer];
                pointer++;

                length += (currentByte & 127) * multiplier;
                multiplier *= 128;
            } while ((currentByte & 128) != 0);

            return length;
        }

        /// <summary>
        /// This is only valid for control headers that are not of type Publish. Publish Packages require a
        /// special control byte.
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected static byte GetNonPublishControlHeader(MqttCommandType commandType)
        {
            byte controlHeader = (byte) ((byte) commandType << 4);
            if (MqttCommandType.PUBREL == commandType || MqttCommandType.SUBSCRIBE == commandType ||
                MqttCommandType.UNSUBSCRIBE == commandType)
                controlHeader += 2;
            return controlHeader;
        }

        protected static void WriteRemainingLengthIntoBuffer(ref byte[] buffer, ref int pointer, int remainingLength)
        {
            do
            {
                buffer[pointer] = (byte) (remainingLength % 128);
                remainingLength = remainingLength / 128;

                // only 7 bit are used for encoding length, MSB is reserved to signal if the next byte is also used
                // for remaining length encoding

                // add MSB to signal that the remaining length is continued
                if (remainingLength > 0)
                    buffer[pointer] += 128;
                pointer++;
            } while (remainingLength > 0);
        }

        protected static void WriteTwoByteLengthAndStringIntoBuffer(ref byte[] buffer, ref int pointer, string payload)
        {
            WriteTwoByteLengthOfStringIntoBuffer(ref buffer, ref pointer, payload);
            WriteStringIntoBuffer(ref buffer, ref pointer, payload);
        }

        protected static void WriteStringIntoBuffer(ref byte[] buffer, ref int pointer, string payload)
        {
            if (null != payload && payload.Length > 0)
            {
                byte[] payloadAsBytes = Encoding.UTF8.GetBytes(payload);
                Assert.AreEqual(payloadAsBytes.Length, payload.Length);
                for (int i = 0; i < payloadAsBytes.Length; i++)
                {
                    buffer[pointer] = payloadAsBytes[i];
                    pointer++;
                }
            }
        }

        protected static void WriteTwoByteNumberIntoBuffer(ref byte[] buffer, ref int pointer, ushort numToWrite)
        {
            buffer[pointer] = (byte) (numToWrite / 256); //MSB
            pointer++;
            buffer[pointer] = (byte) (numToWrite % 256); //MSB
            pointer++;
        }

        protected static void WriteTwoByteLengthOfStringIntoBuffer(ref byte[] buffer, ref int pointer, string payload)
        {
            ushort length = (ushort) (null != payload ? payload.Length : 0);
            WriteTwoByteNumberIntoBuffer(ref buffer, ref pointer, length);
        }

        /// <summary>
        /// If the entry is not null, always reserve additional
        /// two bytes for setting the length of the payload entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static int GetPayloadEntryLength(string entry)
        {
            return null != entry ? entry.Length + 2 : 0;
        }
    }
}