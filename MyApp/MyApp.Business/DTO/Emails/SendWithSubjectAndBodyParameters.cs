namespace MyApp.Business.DTO.Emails
{
    public class SendWithSubjectAndBodyParameters : SendParametersBase
    {
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
