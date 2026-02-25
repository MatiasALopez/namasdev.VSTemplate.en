namespace MyApp.Business.DTO.Emails
{
    public class SendActivateAccountParameters
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ActivateAccountUrl { get; set; }
    }
}
