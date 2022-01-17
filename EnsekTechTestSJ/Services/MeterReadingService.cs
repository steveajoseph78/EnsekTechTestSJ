using Ensek.API.Dtos;
using Ensek.API.Interfaces;
using Ensek.Data;
using Ensek.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ensek.API.Services
{
    public class MeterReadingService : IMeterReadingService
    {
        internal readonly EnsekDbContext _context;

        const int _numDataFields = 3;

        const string _accountId = "accountid";
        const string _meterReadingDateTime = "meterreadingdatetime";
        const string _meterReadingValue = "meterreadvalue";

        List<MeterReadingDto> _meterReadingDtos = new List<MeterReadingDto>();


        public MeterReadingService(EnsekDbContext context)
        {
            _context = context;
        }

        public async Task<MeterReadingUploadSuccessFailures> ValidateAndProcessCSVFile(IFormFile file)
        {
            MeterReadingUploadSuccessFailures meterReadingUploadSuccessFailures = new MeterReadingUploadSuccessFailures();
            
            if (file == null)
            {
                return meterReadingUploadSuccessFailures;
            }


            if (file.FileName.EndsWith(".csv"))
            {
                using (var sreader = new StreamReader(file.OpenReadStream()))
                {
                    string[] headers = sreader.ReadLine().Split(',');

                    if (ValidateHeaderRow(headers) == false)
                    {
                        //File not valid
                        return meterReadingUploadSuccessFailures;
                    }

                    while (!sreader.EndOfStream)
                    {
                        string[] meterDataRow = sreader.ReadLine().Split(',');
                        if (await ValidateMeterReadingAndUpload(meterDataRow) == true)
                        {
                            meterReadingUploadSuccessFailures.SuccessfullUploads++;
                        }
                        else
                        {
                            meterReadingUploadSuccessFailures.FailedUploads++;
                        }
                    }                                  
                }
            }
            else
            {                 
                return meterReadingUploadSuccessFailures;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                Console.WriteLine("An error has ocured writing to the database");
            }
            return meterReadingUploadSuccessFailures;
        }


        private bool ValidateHeaderRow(string [] headers)
        {
            if (headers.Count() != _numDataFields)
            {
                Console.WriteLine("Unepected Number of fields in header row");
                return false;
            }
            
            if (headers[0].Trim().ToLower()==_accountId && headers[1].Trim().ToLower() == _meterReadingDateTime && headers[2].Trim().ToLower() == _meterReadingValue){
                return true;
            }

            Console.WriteLine("Bad data found in header row");
            return false;
        }

        private async Task<int> FindMatchingAccountId(int AccountId)
        {
            Account account = await _context.Accounts.Where(a => a.Id == AccountId).FirstOrDefaultAsync();
            if (account != null)
            {
                return account.Id;
            }
            else
            {
                Console.WriteLine("No matching account found");
                return 0;
            }
        }

        private async Task<bool> CheckNotDuplicateMeterReading(MeterReading meterReading)
        {
            MeterReading duplicateMeterReading = await _context.MeterReadings.Where(a => a.AccountId == meterReading.AccountId && a.MeterReadingDateTime == meterReading.MeterReadingDateTime && a.MeterReadValue == meterReading.MeterReadValue).FirstOrDefaultAsync();
            if (duplicateMeterReading == null)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Duplicate meter reading found");
                return false;
            }
        }

        private async Task<bool> ValidateMeterReadingAndUpload(string[] meterDataRow)
        {
            MeterReading meterReading = new MeterReading();
           
            if (meterDataRow.Count() != _numDataFields)
            {
                Console.WriteLine("Unepected Number of fields in row");
                return false;
            }

            int accountId;
            if (int.TryParse(meterDataRow[0].ToString(), out accountId))
            {
                int id = await FindMatchingAccountId(accountId);
                if (id != 0)
                {
                    meterReading.Account = null;
                    meterReading.AccountId = id;
                }
                else
                {
                    Console.WriteLine("No matching Account found");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Unable to cast Account Id to Int");
                return false;
            }
            DateTime meterReadingDateTime;

            if (DateTime.TryParse(meterDataRow[1].ToString(), out meterReadingDateTime))
            {
                meterReading.MeterReadingDateTime = meterReadingDateTime;
            }
            else
            {
                Console.WriteLine("Unable to cast Meter Reading DateTime to a valid DateTime");
                return false;
            }
            int meterReadingValue;

            if (int.TryParse(meterDataRow[2].ToString(), out meterReadingValue))
            {
                if (meterDataRow[2].Length == 5)
                {
                    meterReading.MeterReadValue = meterReadingValue;
                }
                else
                {
                    Console.WriteLine("Meter Reading Value must contain 5 digits");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Unable to cast Meter Reading Value to a valid int");
                return false;
            }

            if (await CheckNotDuplicateMeterReading(meterReading) == false)
            {
                Console.WriteLine("A duplicate meter reading has been found");
                return false;
            }         
            
            DateTime lastDateMeterReadingDate = await _context.MeterReadings.Where(a => a.AccountId == meterReading.AccountId).OrderBy(m=>m.MeterReadingDateTime).Select(m=>m.MeterReadingDateTime).FirstOrDefaultAsync();

            if (meterReading.MeterReadingDateTime <= lastDateMeterReadingDate)
            {
                Console.WriteLine("A newer meter reading has already been recorded against this account Id in the db");
                return false;
            }
            else 
            {
                // Maybe we have a pending write so lets also check the collection for a new reading
                lastDateMeterReadingDate =  _meterReadingDtos.Where(m=>m.AccountId == meterReading.AccountId).OrderBy(m => m.MeterReadingDateTime).Select(m => m.MeterReadingDateTime).FirstOrDefault();
                if (meterReading.MeterReadingDateTime <= lastDateMeterReadingDate)
                {
                    Console.WriteLine("A newer meter reading against this account Id is pending to be written to db");
                    return false;
                }

                _meterReadingDtos.Add(new MeterReadingDto
                {
                    AccountId = meterReading.AccountId,
                    MeterReadingDateTime = meterReading.MeterReadingDateTime,
                    MeterReadValue = meterReading.MeterReadValue
                });

                _context.Entry(meterReading).State = EntityState.Added;
                await _context.MeterReadings.AddAsync(meterReading);
                return true;
            }
        }
    }
}
