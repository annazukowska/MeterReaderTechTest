using MeterReaderTechTest.Models;

namespace MeterReaderTechTest.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
    }
}
