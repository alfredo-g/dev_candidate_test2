using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject;

public class AppDbContext : DbContext
{
    public DbSet<EmployeeModel> Employees { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //
        // Fluent API

        modelBuilder.Entity<EmployeeModel>().HasIndex(x => x.RFC).IsUnique();
        modelBuilder.Entity<EmployeeModel>().HasQueryFilter(x => !x.Deleted);            

        // Seed the database with a list of employees
        Seed(modelBuilder);
    }

    /// <summary>
    /// Add a few employees to the database for testing purposes
    /// </summary>
    /// <param name="builder"></param>
    private static void Seed(ModelBuilder builder)
    {
        var list = new List<EmployeeModel>();

        var names = new string[] {
            "Noelle Mcdowell", "Jaquan Yu", "Emilee Hernandez", "Stephany Allison", "Marlene Franco",
            "Isabell Green", "Isis Kaiser", "Adelyn Rush", "Bentley Mercer", "Edward Levy", "Davian Yates", "Arielle Salazar"
        };

        for(int i = 0; i < names.Length; i++) 
        {
            list.Add(new EmployeeModel 
            {
                ID = i+1,
                Name = names[i],
                RFC = "XEXT990101N14", // TODO: Set a valid RFC for each one
                BornDate = DateTime.Now.AddYears(-18-i),
                Status = (EmployeeStatus)new Random().Next(1, 3)
            });
        }

        builder.Entity<EmployeeModel>().HasData(list);
    }
}