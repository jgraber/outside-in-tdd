using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningJournalApi
{
    public class JournalEntryModel
    {
        public DateTimeOffset Time { get; set; }

        public int Distance { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
