using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace TrelloMite
{
    public class DateTimeSpan
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public TimeSpan Difference { get { return EndDate.Value.Subtract(StartDate.Value); } }
        public bool IsComplete { get { return StartDate.HasValue && EndDate.HasValue; } }
    }

    public class DateTimeSpanCollection : List<DateTimeSpan>
    {
        public TimeSpan Difference
        {
            get
            {
                return this.Aggregate(TimeSpan.Zero, (current, item) => current.Add(item.Difference));
            }
        }

        public bool AreAllSpansComplete
        {
            get { return this.All(x => x.IsComplete); }
        }
    }
}