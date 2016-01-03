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
        private readonly static List<JournalEntryModel> entries = new List<JournalEntryModel>();

        public HttpResponseMessage Get()
        {
            SimpleWebToken swt;
            SimpleWebToken.TryParse(this.Request.Headers.Authorization.Parameter, out swt);
            var userName = swt.Single(c => c.Type == "userName").Value;

            var connStr = ConfigurationManager.ConnectionStrings["running-journal"].ConnectionString;
            var db = Database.OpenConnection(connStr);

            var entries = db.JournalEntry
                .FindAll(db.JournalEntry.User.UserName == userName)
                .ToArray<JournalEntryModel>();

            return this.Request.CreateResponse(HttpStatusCode.OK, new JournalModel {Entries = entries});
        }

        public HttpResponseMessage Post(JournalEntryModel journal)
        {
            SimpleWebToken swt;
            SimpleWebToken.TryParse(this.Request.Headers.Authorization.Parameter, out swt);
            var userName = swt.Single(c => c.Type == "userName").Value;

            var connStr = ConfigurationManager.ConnectionStrings["running-journal"].ConnectionString;
            var db = Database.OpenConnection(connStr);

            var userId = db.User.Insert(UserName: userName).UserId;

            db.JournalEntry.Insert(
                UserId: userId,
                Time: journal.Time,
                Distance: journal.Distance,
                Duration: journal.Duration);

            return this.Request.CreateResponse();
        }
    }
}
