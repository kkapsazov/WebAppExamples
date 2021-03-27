using System;
using System.Collections.Generic;
using System.IO;
using GraphQLExample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GraphQLExample
{
    public class CoreDbContext : DbContext
    {
        public CoreDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                string? env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                IConfigurationRoot config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{env}.json")
                    .Build();
                string connectionString = config.GetConnectionString("SQLite");


                optionsBuilder.UseSqlite(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(new List<User>
            {
                new()
                {
                    Id = 1,
                    Username = "test",
                    Password = "test",
                    Salt = null
                }
            });

            modelBuilder.Entity<Author>().HasData(new List<Author>
            {
                new()
                {
                    Id = 1,
                    Name = "Uncle Bob"
                }
            });

            modelBuilder.Entity<Book>().HasData(new List<Book>
            {
                new()
                {
                    Id = 1,
                    Name = "Clean code",
                    UserId = 1,
                    AuthorId = 1
                }
            });
        }
    }
}
