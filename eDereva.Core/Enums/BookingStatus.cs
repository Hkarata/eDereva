namespace eDereva.Core.Enums;

public enum BookingStatus
{
    Pending,      // Waiting for session start
    Active,       // Exam in progress
    Completed,    // Exam finished
    Canceled,     // Booking canceled
    Contingency   // Booking affected by contingency
}