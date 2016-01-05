namespace RunningJournalApi
{
    class AddJournalEntryCommand : IAddJournalEntryCommand
    {
        private readonly dynamic db;

        public AddJournalEntryCommand(dynamic db)
        {
            this.db = db;
        }

        public void AddJournalEntry(JournalEntryModel journal, string userName)
        {
            var userId = this.db.User
                .FindAllByUserName(userName)
                .Select(this.db.User.UserId)
                .ToScalarOrDefault<int>();

            if (userId == 0)
            {
                userId = this.db.User.Insert(UserName: userName).UserId;
            }

            this.db.JournalEntry.Insert(
                UserId: userId,
                Time: journal.Time,
                Distance: journal.Distance,
                Duration: journal.Duration);
        }
    }
}
