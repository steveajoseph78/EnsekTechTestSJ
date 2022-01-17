using Ensek.API.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensek.API.Interfaces
{
    public interface IMeterReadingService
    {
        Task<MeterReadingUploadSuccessFailures> ValidateAndProcessCSVFile(IFormFile file);

    }
}
