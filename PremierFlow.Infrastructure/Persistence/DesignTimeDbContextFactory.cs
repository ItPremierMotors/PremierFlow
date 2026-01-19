using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremierFlow.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory: IDesignTimeDbContextFactory<PremierFlowDbContext>
    {
        public PremierFlowDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PremierFlowDbContext>();

            // Tu connection string aquí
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=PremierFlowDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new PremierFlowDbContext(optionsBuilder.Options);
        }
    }
}
