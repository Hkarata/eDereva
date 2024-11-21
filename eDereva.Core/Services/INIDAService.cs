using eDereva.Core.Contracts.Responses;

namespace eDereva.Core.Services;

public interface INIDAService
{
    Task<UserDto?> LoadUserDataAsync(string NIN);
}