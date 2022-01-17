using Ensek.API;
using Ensek.API.Controllers;
using Ensek.API.Dtos;
using Ensek.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Xunit;
 

namespace Ensek.API.Test
{
    public class MeterReadingsControllerTests
    {
        [Fact]
        public void MeterReadings_NoFileInRequest_ReturnsBadRequest()
        {

            MeterReadingUploadSuccessFailures emptyMeterReadingUploadSuccessFailures = new MeterReadingUploadSuccessFailures();

            // Arrange
            var meterReadingServiceStub = new Mock<IMeterReadingService>();
            meterReadingServiceStub.Setup(service => service.ValidateAndProcessCSVFile(null))
                .ReturnsAsync(emptyMeterReadingUploadSuccessFailures);

            var controller = new MeterReadingsController(meterReadingServiceStub.Object);

            // Act
            var result =  controller.MeterReadingsAsyc(null);

            // Assert
            Assert.Equal(new MeterReadingUploadSuccessFailures().FailedUploads , result.Result.Value.FailedUploads);
            Assert.Equal(new MeterReadingUploadSuccessFailures().SuccessfullUploads, result.Result.Value.SuccessfullUploads);
        }
    }
}
