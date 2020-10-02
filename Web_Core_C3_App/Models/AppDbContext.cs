using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Core_C3_App.Models
{
    public class AppDbContext : DbContext
    {
    
    public AppDbContext (DbContextOptions options) : base(options)
    {}
    public DbSet<Employee> employees { get; set; }
    public DbSet<Company> companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
    }

}
