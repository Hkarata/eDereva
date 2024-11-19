using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace eDereva.Workflows.Services;

public interface IPublicHolidayService
{
    IEnumerable<(DateTime Date, string Name)> GetPublicHolidays(DateTime startDate, DateTime endDate);
    IEnumerable<(DateTime Date, string Name)> GetUpcomingHolidays(int days = 30);
}

public class PublicHolidayService : IPublicHolidayService
{
    private readonly ILogger<PublicHolidayService> _logger;
    
    private static readonly IReadOnlyList<(int Month, int Day, string Name)> FixedHolidays = new List<(int Month, int Day, string Name)>
    {
        (1, 1, "New Year's Day"),
        (4, 7, "Good Friday"),
        (4, 9, "Easter Monday"),
        (5, 1, "Labour Day"),
        (7, 7, "Saba Saba (Industry Day)"),
        (8, 8, "Nane Nane (Farmers' Day)"),
        (10, 14, "Independence Day"),
        (12, 9, "Uhuru Day"),
        (12, 25, "Christmas Day"),
        (12, 26, "Boxing Day")
    };

    private readonly HijriCalendar _hijriCalendar = new();
    private readonly GregorianCalendar _gregorianCalendar = new();

    public PublicHolidayService(ILogger<PublicHolidayService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all public holidays within a given date range.
    /// </summary>
    /// <param name="startDate">Start date of the range</param>
    /// <param name="endDate">End date of the range</param>
    /// <returns>List of holiday dates with their names</returns>
    public IEnumerable<(DateTime Date, string Name)> GetPublicHolidays(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("Calculating public holidays between {StartDate} and {EndDate}", startDate, endDate);

        if (startDate > endDate)
        {
            _logger.LogError("Invalid date range: Start date {StartDate} is after end date {EndDate}", startDate, endDate);
            throw new ArgumentException("Start date must be before or equal to end date");
        }

        var holidays = new List<(DateTime Date, string Name)>();

        // Add fixed holidays
        _logger.LogDebug("Processing fixed holidays for year {Year}", startDate.Year);
        foreach (var (month, day, name) in FixedHolidays)
        {
            try
            {
                var holidayDate = new DateTime(startDate.Year, month, day);
                if (holidayDate < startDate || holidayDate > endDate) continue;
                _logger.LogDebug("Adding fixed holiday: {HolidayName} on {HolidayDate}", name, holidayDate);
                holidays.Add((holidayDate, name));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, "Invalid date for fixed holiday {HolidayName} ({Month}/{Day}/{Year})", 
                    name, month, day, startDate.Year);
            }
        }

        // Add Islamic holidays
        try
        {
            _logger.LogDebug("Calculating Islamic holidays for year {Year}", startDate.Year);
            var islamicHolidays = new[]
            {
                (CalculateIslamicHoliday(startDate.Year, 10, 1), "Eid al-Fitr"),
                (CalculateIslamicHoliday(startDate.Year, 12, 10), "Eid al-Adha"),
                (CalculateIslamicHoliday(startDate.Year, 3, 12), "Mawlid al-Nabi")
            };

            foreach (var (date, name) in islamicHolidays)
            {
                if (date < startDate || date > endDate) continue;
                _logger.LogDebug("Adding Islamic holiday: {HolidayName} on {HolidayDate}", name, date);
                holidays.Add((date, name));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating Islamic holidays for year {Year}", startDate.Year);
        }

        var orderedHolidays = holidays.OrderBy(h => h.Date).ToList();
        _logger.LogInformation("Found {Count} holidays in specified date range", orderedHolidays.Count);
        
        return orderedHolidays;
    }

    /// <summary>
    /// Get all public holidays for the next N days from today.
    /// </summary>
    /// <param name="days">Number of days to look ahead</param>
    /// <returns>List of holiday dates with their names</returns>
    public IEnumerable<(DateTime Date, string Name)> GetUpcomingHolidays(int days = 30)
    {
        _logger.LogInformation("Retrieving upcoming holidays for next {Days} days", days);
        
        var startDate = DateTime.Today;
        var endDate = startDate.AddDays(days);
        
        return GetPublicHolidays(startDate, endDate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private DateTime CalculateIslamicHoliday(int year, int month, int day)
    {
        _logger.LogDebug("Calculating Islamic holiday date for Hijri {Year}/{Month}/{Day}", year, month, day);

        try
        {
            var hijriDate = new DateTime(year, month, day, _hijriCalendar);

            if (year > _hijriCalendar.MaxSupportedDateTime.Year)
            {
                _logger.LogWarning(
                    "Requested year {Year} exceeds maximum supported Hijri year {MaxYear}. Using max supported date.", 
                    year, 
                    _hijriCalendar.MaxSupportedDateTime.Year
                );
                hijriDate = _hijriCalendar.MaxSupportedDateTime;
            }

            var gregorianDate = new DateTime(
                hijriDate.Year,
                hijriDate.Month,
                hijriDate.Day,
                _gregorianCalendar
            );

            _logger.LogDebug(
                "Converted Hijri date {HijriDate} to Gregorian date {GregorianDate}", 
                $"{year}/{month}/{day}", 
                gregorianDate.ToString("yyyy/MM/dd")
            );

            return gregorianDate;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to calculate Islamic holiday for Hijri date {Year}/{Month}/{Day}",
                year, month, day
            );
            throw;
        }
    }
}