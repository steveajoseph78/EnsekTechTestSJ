using Ensek.Data.Configuration;
using Ensek.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensek.Data
{
    public class EnsekDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }

        //public string ConnectionString = "Data Source=DESKTOP-0QN76L8\\SQLEXPRESS; Initial Catalog=Ensek;User Id=Steve;Password=XYZ123;";

        public string _connectionString {get;set;}
        public EnsekDbContext(DbContextOptions options) : base(options)
        {
            var sqlServerOptionsExtension = options.FindExtension<SqlServerOptionsExtension>();
            if (sqlServerOptionsExtension != null)
            {
                _connectionString = sqlServerOptionsExtension.ConnectionString;
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(_connectionString);           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AccountSeeding.Seed(modelBuilder);
        }
      
    } 
}
