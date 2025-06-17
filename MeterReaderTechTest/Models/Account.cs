using System.ComponentModel.DataAnnotations;

namespace MeterReaderTechTest.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
    }
}
