using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCWithBlazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCWithBlazor.Data
{
    public class ReportDbContext : IdentityDbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options)
            : base(options)
        { }

        public DbSet<PlcModel> Plcs { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        // Add Index Model To EE for Databse
        public DbSet<IndexModel> Indexes { get; set; }
        // Add Prognoza Energie Model To EE for Database
        public DbSet<PrognozaEnergieModel> PrognozaEnergieModels { get; set; }
    }
}
