﻿using MeterReaderTechTest.Models;

namespace MeterReaderTechTest.DTOs
{
    public class MeterReadingDto
    {
        public string? AccountId { get; set; }
        public string? ReadingDateTime { get; set; }
        public string? ReadValue { get; set; }
    }
}
