using Microsoft.EntityFrameworkCore;
using SGA_Smash.Models;

namespace SGA_Smash.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }

        public DbSet<Producto> Producto { get; set; }
        public DbSet<Proveedor> Proveedore { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Usuario> Usuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Empleado
            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.ToTable("Empleado");
                entity.Property(e => e.SalarioBase).HasColumnType("decimal(10,2)");
            });

            // Mantener la configuración de Cliente si la tienes
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Cliente");
                entity.Property(e => e.FechaRegistro).HasColumnName("FechaRegistro");
            });
        }
    }
}