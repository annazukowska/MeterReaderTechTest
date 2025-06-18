using CsvHelper.Configuration;
using MeterReaderTechTest.DTOs;

namespace MeterReaderTechTest.Mappings
{
    public class MeterReadingMap : ClassMap<MeterReadingDto>
    {
        public MeterReadingMap()
        {
            Map(m => m.AccountId).Name("AccountId");
            Map(m => m.ReadingDateTime).Name("MeterReadingDateTime");
            Map(m => m.ReadValue).Name("MeterReadValue");
        }
    }
}
