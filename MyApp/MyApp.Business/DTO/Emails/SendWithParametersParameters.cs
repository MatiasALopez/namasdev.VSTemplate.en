using MyApp.Entities;

namespace MyApp.Business.DTO.Emails
{
    public class SendWithParametersParameters : SendParametersBase
    {
        public EmailParameters Parameters { get; set; }
    }
}
