using nanoFramework.TestFramework;
using TelemetryStash.ServiceModels;

namespace DataCollector.Tests
{
    [TestClass]
    public class RegisterExtensionTests
    {
        [TestMethod]
        public void ToRegister_Returns_identifier()
        {
            // Arrange
            const string identifier = "Test";

            // Act
            var register = RegisterExtension.ToRegister(identifier, 1, 1);

            // Assert
            Assert.AreEqual(identifier, register.Identifier);
        }

        [TestMethod]
        public void ToRegister_IntPart_returns_value()
        {
            // Arrange
            const double value1 = 10.51;

            // Act
            var register = RegisterExtension.ToRegister("Test", value1, 1);

            // Assert
            Assert.AreEqual(10, register.IntPart);
        }

        [TestMethod]
        public void ToRegister_Handles_zero()
        {
            // Arrange
            const int value1 = 0;
            const double value2 = 0.0;
            const double value3 = 0.00;

            // Act
            var register1 = RegisterExtension.ToRegister("Test", value1, 1);
            var register2 = RegisterExtension.ToRegister("Test", value2, 1);
            var register3 = RegisterExtension.ToRegister("Test", value3, 1);

            // Assert
            Assert.AreEqual(0, register1.IntPart);
            Assert.AreEqual((short)0, register1.Fractions);

            Assert.AreEqual(0, register2.IntPart);
            Assert.AreEqual((short)0, register2.Fractions);

            Assert.AreEqual(0, register3.IntPart);
            Assert.AreEqual((short)0, register3.Fractions);
        }

        [TestMethod]
        public void ToRegister_Fractions_returns_zero()
        {
            // Arrange
            const double value1 = 10;
            const double value2 = 10.0;
            const double value3 = 10.00000;

            // Act
            var register1 = RegisterExtension.ToRegister("Test", value1, 1);
            var register2 = RegisterExtension.ToRegister("Test", value2, 1);
            var register3 = RegisterExtension.ToRegister("Test", value3, 1);

            // Assert
            Assert.AreEqual((short)0, register1.Fractions);
            Assert.AreEqual((short)0, register2.Fractions);
            Assert.AreEqual((short)0, register3.Fractions);
        }

        [TestMethod]
        public void ToRegister_Fractions_returns_single_decimal()
        {
            // Arrange
            const double value1 = 10.5;
            const double value2 = 10.51;
            const double value3 = 10.51111111;

            // Act
            var register1 = RegisterExtension.ToRegister("Test", value1, 1);
            var register2 = RegisterExtension.ToRegister("Test", value2, 1);
            var register3 = RegisterExtension.ToRegister("Test", value3, 1);

            // Assert
            Assert.AreEqual((short)5, register1.Fractions);
            Assert.AreEqual((short)5, register2.Fractions);
            Assert.AreEqual((short)5, register3.Fractions);
        }

        [TestMethod]
        public void ToRegister_Fractions_returns_two_decimals_reversed()
        {
            // Arrange
            const double value1 = 10.51;
            const double value2 = 10.51111111;

            // Act
            var register1 = RegisterExtension.ToRegister("Test", value1, 2);
            var register2 = RegisterExtension.ToRegister("Test", value2, 2);

            // Assert
            Assert.AreEqual((short)15, register1.Fractions);
            Assert.AreEqual((short)15, register2.Fractions);
        }

        [TestMethod]
        public void ToRegister_Fractions_returns_three_decimals_reversed()
        {
            // Arrange
            const double value1 = 10.101;
            const double value2 = 10.001;
            const double value3 = 10.010;

            // Act
            var register1 = RegisterExtension.ToRegister("Test", value1, 3);
            var register2 = RegisterExtension.ToRegister("Test", value2, 3);
            var register3 = RegisterExtension.ToRegister("Test", value3, 3);

            // Assert
            Assert.AreEqual((short)101, register1.Fractions);
            Assert.AreEqual((short)100, register2.Fractions);
            Assert.AreEqual((short)10, register3.Fractions);
        }
    }
}
