using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace RobotOrchestrator.FleetManager.Tests
{
    public class IotHubRegistryClientTest
    {
        private Mock<ILogger<FleetManager>> mockLogger;

        public IotHubRegistryClientTest()
        {
            mockLogger = new Mock<ILogger<FleetManager>>();
        }

        [Theory]
        [InlineData("HostName=iot-hub-test.azure-devices.net;SharedAccessKeyName=device;SharedAccessKey=fakekey=")]
        public void GetHostName_Success(string validConnectionString)
        { 
            Assert.Equal("HostName=iot-hub-test.azure-devices.net", IotHubRegistryClient.GetHostName(validConnectionString));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("HostName=iot-hub-test.azure-devices.net")]
        public void GetHostName_Failed(string invalidConnectionString)
        {
            Action expectedException = () => { IotHubRegistryClient.GetHostName(invalidConnectionString); };

            var exception = Record.Exception(expectedException);

            Assert.NotNull(exception);
        }

        [Fact]
        public void GetDeviceConnectionStringTest()
        {
            string validConnectionString = "HostName=iot-hub-test.azure-devices.net;SharedAccessKeyName=device;SharedAccessKey=fakekey=";
            IotHubRegistryClient iotHubRegistryClient = new IotHubRegistryClient(validConnectionString, mockLogger.Object);

            string expectedResult = iotHubRegistryClient.GetDeviceConnectionString("device", "key");
            string factResult = "HostName=iot-hub-test.azure-devices.net;DeviceId=device;SharedAccessKey=key";

            Assert.Equal(factResult, expectedResult);
        }
    }
}
