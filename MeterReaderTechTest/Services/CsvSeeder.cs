using System.Globalization;
using CsvHelper;
using MeterReaderTechTest.Data;
using MeterReaderTechTest.Mappings;
using MeterReaderTechTest.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReaderTechTest.Services;

public class CsvSeeder
{
    private readonly AppDbContext _context;

    public CsvSeeder(AppDbContext context)
    {
        _context = context;
    }

    public void SeedAccounts(string filePath)
    {
        if (_context.Accounts.Any()) return;

        try
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<AccountMap>();

            var accounts = csv.GetRecords<Account>().ToList();

            using var transaction = _context.Database.BeginTransaction();

            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Accounts ON");

            _context.Accounts.AddRange(accounts);
            _context.SaveChanges();

            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Accounts OFF");

            transaction.Commit();

            Console.WriteLine($"Seeded {accounts.Count} accounts.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error seeding accounts: {ex.Message}");
        }
    }
}
