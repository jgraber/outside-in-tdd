﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Xunit;

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

        [Theory]
        [InlineData("Invalid")]
        [InlineData("Not-Bearer")]
        [InlineData("Bear")]
        [InlineData("Bearer-it-is-not")]
        public void GetUserNameFromRequestWithIncorrectAuthorizationSchemeReturnsCorrectResult(
            string invalidSchema)
        {
            var sut = new SimpleWebTokenUserNameProjection();

            var request = new HttpRequestMessage();
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    invalidSchema,
                    new SimpleWebToken(new Claim("userName", "dummy")).ToString());

            var actual = sut.GetUserName(request);

            Assert.Null(actual);
        }

        [Fact]
        public void GetUserNameFromInvalidSimpleWebTokenReturnsCorrectResult()
        {
            var sut = new SimpleWebTokenUserNameProjection();

            var request = new HttpRequestMessage();
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    "invalid token value");

            var actual = sut.GetUserName(request);

            Assert.Null(actual);
        }

        [Fact]
        public void GetUserNAmeFromSimpleWebTokenWithNoUserNameReturnsCorrectResult()
        {
            var sut = new SimpleWebTokenUserNameProjection();

            var request = new HttpRequestMessage();
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    new SimpleWebToken(new Claim("someClaim", "dummy")).ToString());

            var actual = sut.GetUserName(request);

            Assert.Null(actual);
        }
    }
}
