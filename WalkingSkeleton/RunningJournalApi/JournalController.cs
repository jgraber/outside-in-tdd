﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Simple.Data;

namespace RunningJournalApi
{
    public class JournalController : ApiController
    {
        private readonly IUserNameProjection userNameProjection;
        private readonly IJournalEntriesQuery query;
        private readonly IAddJournalEntryCommand addCommand;

        public JournalController(
            IUserNameProjection userNameProjection, 
            IJournalEntriesQuery query, 
            IAddJournalEntryCommand addCommand)
        {
            this.userNameProjection = userNameProjection;
            this.query = query;
            this.addCommand = addCommand;
        }

        public HttpResponseMessage Get()
        {
            var userName = GetUserName();

            var entries = this.query.GetJournalEntries(userName);

            return this.Request.CreateResponse(
                HttpStatusCode.OK, 
                new JournalModel
                {
                    Entries = entries.ToArray()
                });
        }

        public HttpResponseMessage Post(JournalEntryModel journal)
        {
            var userName = GetUserName();

            this.addCommand.AddJournalEntry(journal, userName);

            return this.Request.CreateResponse();
        }
        
        private string GetUserName()
        {
            SimpleWebToken swt;
            SimpleWebToken.TryParse(this.Request.Headers.Authorization.Parameter, out swt);
            var userName = swt.Single(c => c.Type == "userName").Value;
            return userName;
        }
    }
}
