using System.Text;
using TelemetryStash.Shared;

namespace TelemetryStash.Aidon.Sensor
{
    public static class AidonMessageValidator
    {
        const char CrcBegin = '!';

        public static bool IsValid(string message, out string error)
        {
            error = null;

            if (message == null)
            {
                error = "Message is null";
                return false;
            }

            var messageParts = message.Split(CrcBegin);
            if (messageParts.Length != 2)
            {
                error = $"One CRC begin character ({CrcBegin}) expected. Actual: {messageParts.Length - 1}";
                return false;
            }

            var messageBytes = Encoding.UTF8.GetBytes(messageParts[0] + CrcBegin);
            var calculatedCrc = CrcCalculator.CalculateCrc16Arc(messageBytes).ToString("X4");

            if (messageParts[1] != calculatedCrc)
            {
                error = $"Invalid CRC: expected '{messageParts[1]}', actual '{calculatedCrc}'";
                return false;
            }

            return true;
        }
    }
}
