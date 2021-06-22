namespace ScillHelpers
{
    public class ScillMqttPacketPublish : ScillMqttPacketBase
    {
        public bool Duplicate;
        public byte QoS;
        public bool Retain;

        public string TopicName;
        public ushort PacketIdentifier;

        public string Payload;

        public static ScillMqttPacketPublish FromBuffer(byte[] buffer)
        {
            ScillMqttPacketPublish result = new ScillMqttPacketPublish();
            int pointer = 0;

            result.CommandType = GetCommandTypeFromBuffer(buffer);

            // Get the Packet Control Flags
            result.PacketControlFlags = (byte) (buffer[0] & 0x0f); // set everything to zero except the last 8 bit

            result.Duplicate = (result.PacketControlFlags & 0x01) != 0;
            result.QoS = (byte) ((result.PacketControlFlags & 0x06) >> 1);
            result.Retain = (result.PacketControlFlags & 0x08) != 0;

            result.RemainingLength = GetRemainingLengthFromBuffer(buffer);
            result.Length = GetPacketLengthFromRemainingLength(result.RemainingLength);
            pointer += GetFixedHeaderLengthFromRemaining(result.RemainingLength);

            int topicLength = GetTwoByteNumberFromBuffer(buffer, ref pointer);
            result.TopicName = GetStringFromBuffer(buffer, ref pointer, topicLength);

            if (result.QoS > 0)
            {
                result.PacketIdentifier = GetTwoByteNumberFromBuffer(buffer, ref pointer);
            }

            int payLoadLength = result.Length - pointer;
            result.Payload = GetStringFromBuffer(buffer, ref pointer, payLoadLength);

            return result;
        }
    }
}