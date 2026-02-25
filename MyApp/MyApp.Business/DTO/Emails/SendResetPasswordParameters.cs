namespace MyApp.Business.DTO.Emails
{
    public class SendResetPasswordParameters
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ResetPasswordUrl { get; set; }
    }
}
