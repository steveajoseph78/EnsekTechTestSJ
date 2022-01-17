using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ensek.API.Dtos
{
    public class MeterReadingUploadSuccessFailures
    {
        public int SuccessfullUploads { get; set; }
        public int FailedUploads { get; set; }

    }
}
