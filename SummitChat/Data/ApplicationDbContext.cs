using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SummitChat.Models;

namespace SummitChat.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<SummitChat.Models.Message>? Message { get; set; }
        
        public DbSet<SummitChat.Models.User>? User { get; set; }
    }
}