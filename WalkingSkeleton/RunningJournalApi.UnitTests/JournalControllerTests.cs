using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using Xunit;
using Moq;
using Xunit.Extensions;

namespace RunningJournalApi.UnitTests
{
    public class JournalControllerTests
    {
        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        [InlineData("baz")]
        public void GetReturnsCorrectResult(string userName)
        {
            var projectionStub = new Mock<IUserNameProjection>();
            var queryStub = new Mock<IJournalEntriesQuery>();
            var cmdDummy = new Mock<IAddJournalEntryCommand>();
            var sut = new JournalController(
                projectionStub.Object, 
                queryStub.Object, 
                cmdDummy.Object)
            {
                Request = new HttpRequestMessage()
            };
            sut.Request.Properties.Add(
                HttpPropertyKeys.HttpConfigurationKey,
                new HttpConfiguration());

            projectionStub.Setup(p => p.GetUserName(sut.Request)).Returns(userName);

            var expected = new[]
            {
                new JournalEntryModel
                {
                    Time = new DateTime(2016, 1, 4),
                    Distance = 5780,
                    Duration = TimeSpan.FromMinutes(33)
                },
                new JournalEntryModel
                {
                    Time = new DateTime(2016, 1, 3),
                    Distance = 5760,
                    Duration = TimeSpan.FromMinutes(31)
                },
                new JournalEntryModel
                {
                    Time = new DateTime(2016, 1, 2),
                    Distance = 5700,
                    Duration = TimeSpan.FromMinutes(30)
                }
            };

            queryStub.Setup(q => q.GetJournalEntries(userName)).Returns(expected);

            var response = sut.Get();
            var actual = response.Content.ReadAsAsync<JournalModel>().Result;

            Assert.True(expected.SequenceEqual(actual.Entries));
        }

        [Fact]
        public void GetWithoutUserNameReturnsCorrectResponse()
        {
            var projectionStub = new Mock<IUserNameProjection>();
            var queryDummy = new Mock<IJournalEntriesQuery>();
            var cmdMock = new Mock<IAddJournalEntryCommand>();
            var sut = new JournalController(
                projectionStub.Object,
                queryDummy.Object,
                cmdMock.Object)
            {
                Request = new HttpRequestMessage()
            };
            sut.Request.Properties.Add(
                HttpPropertyKeys.HttpConfigurationKey,
                new HttpConfiguration());

            projectionStub
                .Setup(p => p.GetUserName(It.IsAny<HttpRequestMessage>()))
                .Returns((string) null);

            var response = sut.Get();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public void PostWithoutUserNameReturnsCorrectResponse()
        {
            var projectionStub = new Mock<IUserNameProjection>();
            var queryDummy = new Mock<IJournalEntriesQuery>();
            var cmdDummy = new Mock<IAddJournalEntryCommand>();
            var sut = new JournalController(
                projectionStub.Object,
                queryDummy.Object,
                cmdDummy.Object)
            {
                Request = new HttpRequestMessage()
            };
            sut.Request.Properties.Add(
                HttpPropertyKeys.HttpConfigurationKey,
                new HttpConfiguration());

            projectionStub
                .Setup(p => p.GetUserName(It.IsAny<HttpRequestMessage>()))
                .Returns((string)null);

            var dummyEntry = new JournalEntryModel();
            
            var response = sut.Post(dummyEntry);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        [InlineData("baz")]
        public void PostInsertsEntry(string userName)
        {
            var projectionStub = new Mock<IUserNameProjection>();
            var queryDummy = new Mock<IJournalEntriesQuery>();
            var cmdMock = new Mock<IAddJournalEntryCommand>();
            var sut = new JournalController(
                projectionStub.Object, 
                queryDummy.Object, 
                cmdMock.Object)
            {
                Request = new HttpRequestMessage()
            };
            sut.Request.Properties.Add(
                HttpPropertyKeys.HttpConfigurationKey,
                new HttpConfiguration());

            projectionStub.Setup(p => p.GetUserName(sut.Request)).Returns(userName);

            var entry = new JournalEntryModel();
            sut.Post(entry);

            cmdMock.Verify(c => c.AddJournalEntry(entry, userName));
        }
    }
}
