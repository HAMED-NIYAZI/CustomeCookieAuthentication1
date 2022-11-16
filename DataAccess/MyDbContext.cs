using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomeCookieAuthentication.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CustomeCookieAuthentication.DataAccess
{
    public class MyDbContext : DbContext
    {


        public MyDbContext(DbContextOptions<MyDbContext> options) :
        base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}