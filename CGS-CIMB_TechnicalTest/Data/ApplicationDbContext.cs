using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CGS_CIMB_TechnicalTest.Models;

namespace CGS_CIMB_TechnicalTest.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CGS_CIMB_TechnicalTest.Models.Reports> Reports { get; set; }
        public DbSet<CGS_CIMB_TechnicalTest.Models.ReportFiles> ReportFiles { get; set; }
    }
}
