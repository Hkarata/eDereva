namespace eDereva.Core.Contracts.Responses
{
    public class UserDto
    {
        public string Nin { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string NationalIdNumber { get; set; } = string.Empty;
    }
}
