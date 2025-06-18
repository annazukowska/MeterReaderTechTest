using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using MeterReaderTechTest.Data;
using MeterReaderTechTest.DTOs;
using MeterReaderTechTest.Mappings;
using MeterReaderTechTest.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReaderTechTest.Services;

public class MeterReadingService
{
    private readonly AppDbContext _context;

    public MeterReadingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int successful, int emptyLines, List<FailedMeterReadingDto> failed)> ProcessCsvAsync(Stream csvStream)
    {
        var failedReadings = new List<FailedMeterReadingDto>();
        int successful = 0;
        int emptyLines = 0;
        var records = new List<MeterReadingDto>();

        try 
        {
            using var reader = new StreamReader(csvStream);
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                IgnoreBlankLines = true,
            };

            using var csv = new CsvReader(reader, csvConfig);
            csv.Context.RegisterClassMap<MeterReadingMap>();

            records = csv.GetRecords<MeterReadingDto>().ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error processing CSV file", ex);
        }
        
        var validAccountIds = await _context.Accounts.Select(a => a.Id).ToListAsync();

        foreach (var record in records)
        {
            if (string.IsNullOrWhiteSpace(record.AccountId) && string.IsNullOrWhiteSpace(record.ReadValue) && string.IsNullOrWhiteSpace(record.ReadingDateTime))
            {
                emptyLines++;
                continue;
            }

            if (!int.TryParse(record.AccountId, out var accountId) || !validAccountIds.Contains(accountId))
            {
                failedReadings.Add(Fail(record, "Invalid Account ID"));
                continue;
            }   

            if (string.IsNullOrEmpty(record.ReadValue) || !Regex.IsMatch(record.ReadValue, @"^\d{5}$"))
            {
                failedReadings.Add(Fail(record, "Invalid format for ReadingValue (should be 5 digits)"));
                continue;
            }

            if (!DateTime.TryParse(record.ReadingDateTime, out var readingDateTime))
            {
                failedReadings.Add(Fail(record, "Invalid date format"));
                continue;
            }

            bool isDuplicate = await _context.MeterReadings.AnyAsync(r =>
                r.AccountId == accountId &&
                r.ReadingDateTime == readingDateTime);

            if (isDuplicate)
            {
                failedReadings.Add(Fail(record, "Duplicate reading"));
                continue;
            }

            var latestReading = await _context.MeterReadings
                .Where(r => r.AccountId == accountId)
                .OrderByDescending(r => r.ReadingDateTime)
                .FirstOrDefaultAsync();

            if (latestReading != null && readingDateTime <= latestReading.ReadingDateTime)
            {
                failedReadings.Add(Fail(record, "Reading is older than the latest one"));
                continue;
            }

            var newReading = new MeterReading
            {
                AccountId = accountId,
                ReadingDateTime = readingDateTime,
                ReadValue = record.ReadValue
            };

            _context.MeterReadings.Add(newReading);
            successful++;
        }

        await _context.SaveChangesAsync();
        return (successful, emptyLines, failedReadings);
    }

    private FailedMeterReadingDto Fail(MeterReadingDto record, string error)
    {
        return new FailedMeterReadingDto
        {
            AccountId = record.AccountId,
            ReadValue = record.ReadValue,
            ReadingDateTime = record.ReadingDateTime,
            ErrorMessage = error
        };
    }
}
