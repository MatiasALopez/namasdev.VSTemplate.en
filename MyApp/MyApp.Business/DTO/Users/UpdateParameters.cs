namespace MyApp.Business.DTO.Users
{
    public class UpdateParameters : ParametersEntityBase<string>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}
