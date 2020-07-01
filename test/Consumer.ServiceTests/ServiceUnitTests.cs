using Consumer.Contracts.Interfaces;
using Consumer.Contracts.Models;
using Consumer.Service.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Consumer.ServiceTests
{
    public class ServiceUnitTests
    {
        const string _exceptionMsg = "bad thing";

        // Given an empty feed
        // When passed to the reporting engine
        // Then we expect no output calls
        [Fact]
        public async Task EmptyFeedShouldOutputNothing()
        {
            var testEnv = EstablishEnvironment(withNoData:true, withException:false);

            await testEnv.ownersReport.CreateReport();

            testEnv.mockDataClient.Verify(x => x.GetOwners(), Times.Once);
            testEnv.mockReportOutput.Verify(x => x.Print(It.IsAny<string>()), Times.Never);
        }

        // Given a feed is called to get owners
        // When the feed throws an exception
        // Then we expect the for the exception to be propogated and no processing to take place.
        [Fact]
        public async Task VerifyFeedExceptionPassThrough()
        {
            var testEnv = EstablishEnvironment(withNoData: false, withException: true);

            Exception ex = await Assert.ThrowsAsync<Exception>(() => testEnv.ownersReport.CreateReport());

            Assert.Equal(_exceptionMsg, ex.Message);
            testEnv.mockReportOutput.Verify(x => x.Print(It.IsAny<string>()), Times.Never);
        }

        // Given a valid feed
        // When passed to the reporting engine
        // Then we expect output calls
        [Fact]
        public async Task ValidFeedShouldOutputSomething()
        {
            var testEnv = EstablishEnvironment(withNoData: false, withException: false);

            await testEnv.ownersReport.CreateReport();

            testEnv.mockDataClient.Verify(x => x.GetOwners(), Times.Once);
            testEnv.mockReportOutput.Verify(x => x.Print(It.IsAny<string>()), Times.Once);
        }



        (OwnersReport ownersReport, 
            Mock<IDataClient> mockDataClient, 
            Mock<IReportOutput> mockReportOutput
        ) EstablishEnvironment(bool withNoData, bool withException)
        {
            var mockDataClient = new Mock<IDataClient>();
            var mockReportOutput = new Mock<IReportOutput>();

            if (withNoData)
            {
                mockDataClient.Setup(x => x.GetOwners()).ReturnsAsync(new List<Owner>());
            }
            else if (withException)
            {
                mockDataClient.Setup(x => x.GetOwners()).ThrowsAsync(new Exception(_exceptionMsg));
            }
            else
            {
                var owners = new List<Owner>() {
                    new Owner() {
                        Name = "Bob",
                        Age = 30,
                        Gender="Male",
                        Pets = new List<Pet>() { 
                            new Pet() { Name = "Tom", Type = "Cat" },
                            new Pet() { Name = "Jerry", Type = "Mouse" } } },
                    new Owner() {
                        Name = "Neil",
                        Age = 35,
                        Gender="Male",
                        Pets = new List<Pet>() {
                            new Pet() { Name = "Pluto", Type = "Dog" } } },
                    new Owner() {
                        Name = "Mary",
                        Age = 35,
                        Gender="Female",
                        Pets = null },
                    new Owner() {
                        Name = "Sue",
                        Age = 35,
                        Gender="Female",
                        Pets = new List<Pet>() {
                            new Pet() { Name = "Tweety", Type = "Bird" } } },
                };
                mockDataClient.Setup(x => x.GetOwners()).ReturnsAsync(owners);
            }

            var ownersReport = new OwnersReport(mockDataClient.Object, mockReportOutput.Object);

            return (ownersReport, mockDataClient, mockReportOutput);
        }

        private object List<T>()
        {
            throw new NotImplementedException();
        }
    }
}
