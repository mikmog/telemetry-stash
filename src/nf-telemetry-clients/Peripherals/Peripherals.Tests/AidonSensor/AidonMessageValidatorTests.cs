using nanoFramework.TestFramework;
using TelemetryStash.Aidon.Sensor;
using TelemetryStash.ServiceModels;

namespace TelemetryStash.Peripherals.Tests.AidonSensor
{
    [TestClass]
    public class AidonMessageValidatorTests
    {
        [TestMethod]
        public void Validates_mocked_message()
        {
            // Act
            var result = AidonMessageValidator.IsValid(TestData.AidonMessage, out string error);

            // Assert
            Assert.IsTrue(result, error);
            Assert.IsNull(error, error);
        }

        [TestMethod]
        public void Validates_message()
        {
           // Act
           var result = AidonMessageValidator.IsValid(TestData.AidonMessage, out string error);

            // Assert
            Assert.IsTrue(result, error);
            Assert.IsNull(error, error);
        }
    }
}
