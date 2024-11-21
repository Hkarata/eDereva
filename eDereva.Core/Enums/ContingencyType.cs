namespace eDereva.Core.Enums;

public enum ContingencyType
{
    None, // No contingency occurred
    PowerFailure, // Power failure during the session
    SystemCrash, // System or hardware crash
    NetworkIssue, // Network connectivity issues
    NaturalDisaster, // Natural disaster (e.g., flood, earthquake)
    SecurityBreach, // Security-related issues
    Pandemic, // Pandemic outbreak of a disease
    Other // Any other type of contingency
}