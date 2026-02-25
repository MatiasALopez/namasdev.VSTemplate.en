using namasdev.Core.Entity;

namespace MyApp.Entities
{
    public partial class EmailParameters : Entity<short>
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public string CC { get; set; }

        public override string ToString()
        {
            return Subject;
        }
    }
}
