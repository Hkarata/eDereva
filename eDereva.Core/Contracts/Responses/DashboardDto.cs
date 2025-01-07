using eDereva.Core.Enums;

namespace eDereva.Core.Contracts.Responses;

public class DashboardDto
{
    public string Nin {get; set;} = string.Empty;
    public string FullName {get; set;} = string.Empty;
    public Sex Sex {get; set;}
    public int Age {get; set;}
    public List<string>? LicenseClasses {get; set;}
}