using apiprofessionals.Models;
using Microsoft.EntityFrameworkCore;

namespace apiprofessionals.RegisterDto
{
    public class VendingDbContext : DbContext
    {
        public VendingDbContext(DbContextOptions<VendingDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
    }
}