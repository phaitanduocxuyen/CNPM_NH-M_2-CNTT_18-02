using Banvemaybay.Models;
using Microsoft.EntityFrameworkCore;

namespace Banvemaybay.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ChuyenBay> ChuyenBays { get; set; }
        public DbSet<SanBay> SanBays { get; set; }
        public DbSet<Ve> Ves { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
    }
}