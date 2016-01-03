using System;
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
        private readonly IJournalEntriesQuery query;
        private readonly IAddJournalEntryCommand addCommand;

        public JournalController(IJournalEntriesQuery query, IAddJournalEntryCommand addCommand)
        {
            this.query = query;
            this.addCommand = addCommand;
        }

        public HttpResponseMessage Get()
        {
            var userName = GetUserName();

            var entries = GetJournalEntries(userName);

            return this.Request.CreateResponse(
                HttpStatusCode.OK, 
                new JournalModel
                {
                    Entries = entries
                });
        }

        private dynamic GetJournalEntries(string userName)
        {
            var connStr = ConfigurationManager.ConnectionStrings["running-journal"].ConnectionString;
            var db = Database.OpenConnection(connStr);

            var entries = db.JournalEntry
                .FindAll(db.JournalEntry.User.UserName == userName)
                .ToArray<JournalEntryModel>();
            return entries;
        }

        public HttpResponseMessage Post(JournalEntryModel journal)
        {
            var userName = GetUserName();

            AddJournalEntry(journal, userName);

            return this.Request.CreateResponse();
        }

        private void AddJournalEntry(JournalEntryModel journal, string userName)
        {
            var connStr = ConfigurationManager.ConnectionStrings["running-journal"].ConnectionString;
            var db = Database.OpenConnection(connStr);

            var userId = db.User.Insert(UserName: userName).UserId;

            db.JournalEntry.Insert(
                UserId: userId,
                Time: journal.Time,
                Distance: journal.Distance,
                Duration: journal.Duration);
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
