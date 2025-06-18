using AutoMapper;
using MeterReaderTechTest.Data;
using MeterReaderTechTest.DTOs;
using MeterReaderTechTest.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddScoped<MeterReadingService>();
builder.Services.AddScoped<CsvSeeder>();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<CsvSeeder>();
    var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Test_Accounts.csv");
    seeder.SeedAccounts(path);
}

app.UseCors();

app.UseRouting();

app.MapGet("/", () => "Hello, Meter Reader API!");

app.MapGet("/accounts", async (AppDbContext db, IMapper mapper) =>
{
    var accounts = await db.Accounts.Include(a => a.MeterReadings).ToListAsync();

    var accountDtos = mapper.Map<List<AccountDto>>(accounts);

    return Results.Ok(accountDtos);
});

app.MapPost("/meter-reading-uploads", async (HttpRequest request, MeterReadingService service) =>
{
    var file = request.Form.Files["file"];
    if (file == null || file.Length == 0)
        return Results.BadRequest("CSV file is required.");

    using var stream = file.OpenReadStream();
    var (successful, emptyLines, failed) = await service.ProcessCsvAsync(stream);

    return Results.Ok(new
    {
        successful,
        emptyLines,
        failed = failed.Count,
        failedReadings = failed
    });
});

app.Run();