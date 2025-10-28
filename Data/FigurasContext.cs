using FigurasGeometricasApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FigurasGeometricasApi.Data
{
    public class FigurasContext : DbContext
    {
        public FigurasContext(DbContextOptions<FigurasContext> options) : base(options) { }

        public DbSet<Figura> Figuras { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Single Table Inheritance (TPH)
            modelBuilder.Entity<Figura>()
                .HasDiscriminator<string>("Tipo")
                .HasValue<Circulo>("Circulo")
                .HasValue<Rectangulo>("Rectangulo")
                .HasValue<Triangulo>("Triangulo");

            modelBuilder.Entity<Figura>().Property(f => f.Nombre).IsRequired().HasMaxLength(120);
            base.OnModelCreating(modelBuilder);
        }
    }
}
