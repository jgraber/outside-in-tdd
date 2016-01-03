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
            var connStr = ConfigurationManager.ConnectionStrings["running-journal"].ConnectionString;
            var db = Database.OpenConnection(connStr);

            var entries = db.JournalEntry
                .FindAll(db.JournalEntry.User.UserName == "foo")
                .ToArray<JournalEntryModel>();

            return this.Request.CreateResponse(HttpStatusCode.OK, new JournalModel {Entries = entries});
        }

        public HttpResponseMessage Post(JournalEntryModel journal)
        {
            var connStr = ConfigurationManager.ConnectionStrings["running-journal"].ConnectionString;
            var db = Database.OpenConnection(connStr);

            var userId = db.User.Insert(UserName: "foo").UserId;

            db.JournalEntry.Insert(
                UserId: userId,
                Time: journal.Time,
                Distance: journal.Distance,
                Duration: journal.Duration);

            return this.Request.CreateResponse();
        }
    }
}
