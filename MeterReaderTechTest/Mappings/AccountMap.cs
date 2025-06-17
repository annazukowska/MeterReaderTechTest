using CsvHelper.Configuration;
using MeterReaderTechTest.Models;

namespace MeterReaderTechTest.Mappings
{
    public class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Map(m => m.Id).Name("AccountId");
            Map(m => m.FirstName);
            Map(m => m.LastName);
        }
    }
}
