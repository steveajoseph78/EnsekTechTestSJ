using Ensek.API.Dtos;
using Ensek.Data;
using Ensek.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ensek.API.Interfaces;

namespace Ensek.API.Controllers
{
    [ApiController]
    [Route("/V1/[controller]")]
    public class MeterReadingsController : ControllerBase
    {
        private readonly IMeterReadingService _meterReadingService;

       // internal readonly EnsekDbContext _context;
    
        public MeterReadingsController( IMeterReadingService meterReadingService)
        {
            //_context = context;
            _meterReadingService = meterReadingService;
        }
  

        [HttpPost("meter-reading-uploads")]
        public async Task<ActionResult<MeterReadingUploadSuccessFailures>> MeterReadingsAsyc(IFormFile file)
        {
            MeterReadingUploadSuccessFailures meterReadingUploadSuccessFailures = await _meterReadingService.ValidateAndProcessCSVFile(file);

            if (meterReadingUploadSuccessFailures!=null)
            {
                return meterReadingUploadSuccessFailures;
            }
            else 
            {
                return BadRequest();
            }
        }

    }
}
