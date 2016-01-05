using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
            var userName = this.userNameProjection.GetUserName(this.Request);
            if (userName == null)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No user name supplied");
            }

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
            var userName = this.userNameProjection.GetUserName(this.Request);
            if (userName == null)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No user name supplied");
            }

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
