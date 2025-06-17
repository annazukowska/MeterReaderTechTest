using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace MeterReaderTechTest.Models
{
    public class MeterReading
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime ReadingDateTime { get; set; }
        public string ReadingValue { get; set; }
        public Account Account { get; set; }

    }

}
