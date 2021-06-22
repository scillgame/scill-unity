namespace ScillHelpers
{
    public enum ScillMqttConnackCode : byte
    {
        ACCEPTED = 0, // Connection Accepted

        REFUSED_VERSION =
            1, // Connection Refused, unacceptable protocol version		// The Server does not support the level of the MQTT protocol requested by the Client

        REFUSED_ID =
            2, // Connection Refused, identifier rejected					// The Client identifier is correct UTF-8 but not allowed by the Server

        REFUSED_UNAVAILABLE =
            3, // Connection Refused, Server unavailable					// The Network Connection has been made but the MQTT service is unavailable

        REFUSED_USER =
            4, // Connection Refused, bad user name or password			// The data in the user name or password is malformed
        REFUSED_AUTH = 5 // Connection Refused, not authorized						// The Client is not authorized to connect
    };

    public class ScillMqttPacketConnack : ScillMqttPacketBase
    {
        public ScillMqttConnackCode Code;
        public bool SessionPresent;

        public static ScillMqttPacketConnack FromBuffer(byte[] buffer)
        {
            int pointer = 0;
            ScillMqttPacketConnack result = new ScillMqttPacketConnack();
            result.CommandType = GetCommandTypeFromBuffer(buffer);
            pointer++;

            // Skip remaining length since it is always 2
            pointer++;

            // session preset
            result.SessionPresent = buffer[pointer] > 0 ? true : false;
            pointer++;

            result.Code = (ScillMqttConnackCode) (buffer[pointer]);
            pointer++;

            return result;
        }
    }
}