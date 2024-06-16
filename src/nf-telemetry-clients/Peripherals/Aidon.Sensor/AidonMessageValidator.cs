namespace TelemetryStash.Aidon.Sensor
{
    public static class AidonMessageValidator
    {
        public const int MinMessageLength = 650;

        const int exclamationPointLocation = 7;
        const char Slash = '/';
        const char ExclamationPoint = '!';

        // TODO: CRC16 validation
        // https://www.convertcase.com/hashing/crc-16-checksum-calculator
        public static bool IsValid(string message, out string error)
        {
            error = null;

            if (message == null)
            {
                error = "Message is null";
            }

            else if (message.Length < MinMessageLength)
            {
                error = "Invalid minimum message length. Expected '" + MinMessageLength + "' . Actual '" + message.Length + "'";
            }

            else if (message[0] != Slash)
            {
                error = "Invalid initial byte. Expected '" + Slash + "' (/). Actual '" + message[0] + "'";
            }

            else if (message[message.Length - exclamationPointLocation] != ExclamationPoint)
            {
                error = "Expected '" + ExclamationPoint + "'. Actual '" + message[message.Length - exclamationPointLocation] + "'";
            }

            return error == null;
        }
    }
}
