using System.Collections.Generic;

using namasdev.Core.IO;

namespace MyApp.Business.DTO.Emails
{
    public class SendParametersBase
    {
        public string To { get; set; }
        public IDictionary<string, string> BodyVariables { get; set; }
        public string BCC { get; set; }
        public IEnumerable<File> Attachments { get; set; }
    }
}
