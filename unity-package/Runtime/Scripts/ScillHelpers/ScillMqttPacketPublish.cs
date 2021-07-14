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

        public ScillMqttPacketPublish(byte[] buffer)
        {
            int pointer = 0;

            CommandType = GetCommandTypeFromBuffer(buffer);

            // Get the Packet Control Flags
            PacketControlFlags = (byte) (buffer[0] & 0x0f); // set everything to zero except the last 8 bit

            Duplicate = (PacketControlFlags & 0x01) != 0;
            QoS = (byte) ((PacketControlFlags & 0x06) >> 1);
            Retain = (PacketControlFlags & 0x08) != 0;

            RemainingLength = GetRemainingLengthFromBuffer(buffer);
            Length = GetPacketLengthFromRemainingLength(RemainingLength);
            pointer += GetFixedHeaderLengthFromRemaining(RemainingLength);

            int topicLength = GetTwoByteNumberFromBuffer(buffer, ref pointer);
            TopicName = GetStringFromBuffer(buffer, ref pointer, topicLength);

            if (QoS > 0)
            {
                PacketIdentifier = GetTwoByteNumberFromBuffer(buffer, ref pointer);
            }

            int payLoadLength = Length - pointer;
            Payload = GetStringFromBuffer(buffer, ref pointer, payLoadLength);
        }
    }
}