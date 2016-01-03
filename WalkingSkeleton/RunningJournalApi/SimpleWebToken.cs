using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;

namespace RunningJournalApi
{
    public class SimpleWebToken : IEnumerable<Claim>
    {
        private readonly IEnumerable<Claim> claims;
        public SimpleWebToken(params Claim[] claims)
        {
            this.claims = claims;
        }

        public IEnumerator<Claim> GetEnumerator()
        {
            return this.claims.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}