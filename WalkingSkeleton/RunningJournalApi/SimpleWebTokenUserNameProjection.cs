﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RunningJournalApi
{
    public class SimpleWebTokenUserNameProjection : IUserNameProjection
    {
        public string GetUserName(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.Headers.Authorization == null)
            {
                return null;
            }

            if (request.Headers.Authorization.Scheme != "Bearer")
            {
                return null;
            }

            SimpleWebToken swt;
            if (!SimpleWebToken.TryParse(request.Headers.Authorization.Parameter, out swt))
            {
                return null;
            }
            var userName = swt.Single(c => c.Type == "userName").Value;
            return userName;
        }
    }
}
