using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensek.API.Dtos
{
    public class MeterReadingDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }       
        public DateTime MeterReadingDateTime { get; set; }
        public decimal MeterReadValue { get; set; }
    }
}
