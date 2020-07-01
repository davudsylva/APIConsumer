using Consumer.Contracts.Interfaces;
using Consumer.Contracts.Models;
using Consumer.OwnersFeed.Clients;
using Consumer.Service.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Consumer.ServiceTests
{
    public class DataLayerUnitTests
    {
        const string _exceptionMsg = "bad thing";

        // Given an empty feed
        // When the API is called
        // Then we expect an emptry resultset
        [Fact]
        public async Task EmptyFeedShouldOutputNothing()
        {
            var testEnv = EstablishEnvironment(withNoData:true, withException:false);

            var owners = await testEnv.dataClient.GetOwners();

            Assert.Empty(owners);
        }

        // Given a feed is called to get owners
        // When the feed throws an exception
        // Then we expect the for the exception to be propogated and no processing to take place.
        [Fact]
        public async Task VerifyFeedExceptionPassThrough()
        {
            var testEnv = EstablishEnvironment(withNoData: false, withException: true);

            Exception ex = await Assert.ThrowsAsync<Exception>(() => testEnv.dataClient.GetOwners());

            Assert.Equal(_exceptionMsg, ex.Message);
        }

        // Given a valid feed
        // When passed to the reporting engine
        // Then we expect output calls
        [Fact]
        public async Task ValidFeedShouldOutputSomething()
        {
            var testEnv = EstablishEnvironment(withNoData: false, withException: false);

            var owners = await testEnv.dataClient.GetOwners();

            Assert.Equal(6, owners.Count());

            var results = new List<Owner>(owners);
            Assert.Equal(2, results[0].Pets.Count());
            Assert.Equal(1, results[1].Pets.Count());
            Assert.Null(results[2].Pets);
            Assert.Equal(4, results[3].Pets.Count());
            Assert.Equal(1, results[4].Pets.Count());
            Assert.Equal(2, results[5].Pets.Count());
        }



        (DataClient dataClient,
            Mock<HttpClient> mockClient
        ) EstablishEnvironment(bool withNoData, bool withException)
        {
            var mockClient = new Mock<HttpClient>();
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            if (withNoData)
            {
                var mockResponse = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]") };

                handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                        )
                        .ReturnsAsync(mockResponse)
                        .Verifiable();
            }
            else if (withException)
            {
                handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                        )
                        .ThrowsAsync(new Exception(_exceptionMsg));
            }
            else
            {
                var mockResponse = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(FileLoader.FeedJson) };

                handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                        )
                        .ReturnsAsync(mockResponse)
                        .Verifiable();
            }

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };


            var mockConfiguration = new Mock<IConfigurationRoot>();
            mockConfiguration.SetupGet(m => m[It.Is<string>(s => s == "url")]).Returns(@"http://abc.def");
            var dataClient = new DataClient(httpClient, mockConfiguration.Object);

            return (dataClient, mockClient);
        }

        private object List<T>()
        {
            throw new NotImplementedException();
        }
    }
}
