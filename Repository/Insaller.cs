using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public static class Installer
    {
        public static void ConfigureServices(IServiceCollection services, string dbConnString)
        {
            // for init db context             
            services.AddDbContext<SQLContext>(opt => opt.UseSqlServer(dbConnString));


            services.AddScoped<IBaseRepository<Profile>, SQLRepository<Profile>>();
            services.AddScoped<IBaseRepository<ProfilePassword>, SQLRepository<ProfilePassword>>();
            services.AddScoped<IBaseRepository<Friend>, SQLRepository<Friend>>();
            services.AddScoped<IBaseRepository<FriendInvitation>, SQLRepository<FriendInvitation>>();
            services.AddScoped<IBaseRepository<Post>, SQLRepository<Post>>();
            services.AddScoped<IBaseRepository<PostInteraction>, SQLRepository<PostInteraction>>();
            services.AddScoped<IBaseRepository<RegisterAttempt>, SQLRepository<RegisterAttempt>>();


            services.AddScoped<IBaseRepository<Language>, SQLRepository<Language>>();
            services.AddScoped<IBaseRepository<LoacalizProperty>, SQLRepository<LoacalizProperty>>();

            services.AddScoped<IBaseRepository<Log>, SQLRepository<Log>>();


        }

        public static void Configure(IServiceScope serviceScope)
        {
            serviceScope.ServiceProvider.GetService<SQLContext>().Database.Migrate();
        }
    }

}
