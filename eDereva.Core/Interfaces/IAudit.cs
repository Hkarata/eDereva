namespace eDereva.Core.Interfaces;

internal interface IAudit
{
    public DateTime CreatedAt { get; }
    public DateTime? ModifiedAt { get; set; }
}