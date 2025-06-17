using MeterReaderTechTest.Models;

namespace MeterReaderTechTest.DTOs
{
    public class MeterReadingDto
    {
        public int AccountId { get; set; }
        public DateTime ReadingDateTime { get; set; }
        public string ReadingValue { get; set; }
    }
}
