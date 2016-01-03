using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningJournalApi
{
    class JournalEntriesQuery : IJournalEntriesQuery
    {
        private readonly dynamic db;

        public JournalEntriesQuery(dynamic db)
        {
            this.db = db;
        }
        public IEnumerable<JournalEntryModel> GetJournalEntries(string userName)
        {
            var entries = this.db.JournalEntry
                .FindAll(db.JournalEntry.User.UserName == userName)
                .ToArray<JournalEntryModel>();
            return entries;
        }
    }
}
