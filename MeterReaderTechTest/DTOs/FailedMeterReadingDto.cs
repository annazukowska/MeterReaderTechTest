using MeterReaderTechTest.Models;

namespace MeterReaderTechTest.DTOs
{
    public class FailedMeterReadingDto
    {
        public string? AccountId { get; set; }
        public string? ReadingDateTime { get; set; }
        public string? ReadValue { get; set; }
        public string ErrorMessage { get; set; }
    }
}
