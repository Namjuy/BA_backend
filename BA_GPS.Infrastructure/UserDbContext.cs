using System;
using System.Collections.Generic;
using BA_GPS.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace BA_GPS.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/01/2024 Created
    /// </Modified>
    public class UserDbContext : DbContext
    {
       

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> UserInfor { get; set; }
  
    }
}

