using CSV.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace CSV.Data {
   public class ApplicationDbContext : DbContext {
      public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options) {
      }

      // criar 'tabela' Dias
      public DbSet<Dias> Dias { get; set; }

   }
}
