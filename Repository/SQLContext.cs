using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class SQLContext : DbContext
    {
        public SQLContext(DbContextOptions<SQLContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding Languages
            modelBuilder.Entity<Language>().HasData(
                  new Language
                  {
                      Id = Guid.Parse("2a90154d-d5a5-473e-a8e0-367ff1c8ec71"),
                      languageName = "EN",
                      languageCulture = "en-US"
                  }, new Language
                  {
                      Id = Guid.Parse("fc39a0fe-3525-4064-8d77-e7d8d08bcef5"),
                      languageName = "AR",
                      languageCulture = "ar-EG"
                  });

        }


        // class tables here

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<ProfilePassword> ProfilePasswords { get; set; }

        public DbSet<RegisterAttempt> RegisterAttempt { get; set; }


        public DbSet<Friend> Friends { get; set; }

        public DbSet<FriendInvitation> FriendInvitations { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostInteraction> PostsInteraction { get; set; }


        public DbSet<Language> Languages { get; set; }

        public DbSet<LoacalizProperty> loacalizProperties { get; set; }

        public DbSet<Log> Log { get; set; }

    }

}
