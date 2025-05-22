using charac.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace charac.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>  // Inherit from IdentityDbContext with IdentityUser
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)   // Pass the DbContextOptions to the base class (IdentityDbContext)
        {
        }

        // Define your other DbSets for your models
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterBiography> CharactersBiography { get; set; }
        public DbSet<Acts> Acts { get; set; }
    }
}
