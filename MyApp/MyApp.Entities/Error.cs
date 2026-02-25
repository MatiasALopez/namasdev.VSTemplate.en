using System;

using namasdev.Core.Entity;

namespace MyApp.Entities
{
    public partial class Error : Entity<Guid>
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string Arguments { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
