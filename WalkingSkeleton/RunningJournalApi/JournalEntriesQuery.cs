using System.Collections.Generic;

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
