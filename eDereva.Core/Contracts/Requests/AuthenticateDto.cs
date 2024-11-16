namespace eDereva.Core.Contracts.Requests;

public class AuthenticateDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsDriver { get; set; }
}