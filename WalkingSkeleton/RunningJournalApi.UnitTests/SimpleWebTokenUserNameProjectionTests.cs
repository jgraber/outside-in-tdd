using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace RunningJournalApi.UnitTests
{
    public class SimpleWebTokenUserNameProjectionTests
    {
        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        [InlineData("baz")]
        public void GetUserNameFromProperSimpleWebTokenReturnsCorrectResult(
            string expected)
        {
            var sut = new SimpleWebTokenUserNameProjection();

            var request = new HttpRequestMessage();
            request.Headers.Authorization = 
                new AuthenticationHeaderValue(
                    "Bearer",
                    new SimpleWebToken(new Claim("userName", expected)).ToString());

            var actual = sut.GetUserName(request);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetUserNameFromNullRequestThrows()
        {
            var sut = new SimpleWebTokenUserNameProjection();
            Assert.Throws<ArgumentNullException>(() =>
                sut.GetUserName(null));
        }

        [Fact]
        public void GetUserNameFromRequestWithoutAuthorizationHeaderReturnsCorrectResult()
        {
            var sut = new SimpleWebTokenUserNameProjection();

            var request = new HttpRequestMessage();
            //Guard against future changes in the way the Authorization headers are uses
            Assert.Null(request.Headers.Authorization);

            var actual = sut.GetUserName(request);

            Assert.Null(actual);
        }

        [Fact]
        public void GetUserNameFromRequestWithIncorrectAuthorizationSchemeReturnsCorrectResult()
        {
            var sut = new SimpleWebTokenUserNameProjection();

            var request = new HttpRequestMessage();
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Invalid",
                    new SimpleWebToken(new Claim("userName", "dummy")).ToString());

            var actual = sut.GetUserName(request);

            Assert.Null(actual);

        }
    }
}
