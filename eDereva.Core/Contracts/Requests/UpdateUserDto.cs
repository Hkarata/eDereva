namespace eDereva.Core.Contracts.Requests
{
    public class UpdateUserDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
