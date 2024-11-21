namespace eDereva.Core.Contracts.Responses;

public class UserData
{
    public Guid Id { get; set; }
    public string Nin { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public int Age { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}