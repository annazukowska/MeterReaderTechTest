using AutoMapper;
using MeterReaderTechTest.Models;
using MeterReaderTechTest.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Account, AccountDto>();
    }
}
