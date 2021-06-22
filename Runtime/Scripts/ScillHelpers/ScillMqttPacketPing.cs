namespace ScillHelpers
{
    public class ScillMqttPacketPing : ScillMqttPacketBase
    {
        public override byte[] ToBuffer()
        {
            Length = 2;
            RemainingLength = 0;

            byte[] buffer = new byte[Length];
            buffer[0] = GetNonPublishControlHeader(MqttCommandType.PINGREQ);
            buffer[1] = (byte) RemainingLength;

            return buffer;
        }
    }
}