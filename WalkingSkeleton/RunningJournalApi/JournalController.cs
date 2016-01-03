using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RunningJournalApi
{
    public class JournalController : ApiController
    {
        private readonly static List<JournalEntryModel> entries = new List<JournalEntryModel>();

        public HttpResponseMessage Get()
        {
            return this.Request.CreateResponse(HttpStatusCode.OK, new JournalModel {Entries = entries.ToArray()});
        }

        public HttpResponseMessage Post(JournalEntryModel journal)
        {
            entries.Add(journal);
            return this.Request.CreateResponse();
        }
    }
}
