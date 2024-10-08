using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCard.Core.Data
{
    public class BusinessCardDbContext:DbContext
    {

        public class BusinessCardDbContextFactory : IDesignTimeDbContextFactory<BusinessCardDbContext>
        {
            public BusinessCardDbContext CreateDbContext(string[] args)
            {
                // Define the options to use for the DbContext
                var optionsBuilder = new DbContextOptionsBuilder<BusinessCardDbContext>();

                // Use the same connection string used in your application
                var connectionString = "Server=DESKTOP-CIA1FQD;Database=BusinessCard;integrated security=true;multipleactiveresultsets=true;trustservercertificate=true;";

                optionsBuilder.UseSqlServer(connectionString);

                return new BusinessCardDbContext(optionsBuilder.Options);
            }
        }

        public BusinessCardDbContext(DbContextOptions<BusinessCardDbContext> options) : base(options)
        {

        }

      public DbSet<BusinessCards> BusinessCards { get; set; }
    }

}
