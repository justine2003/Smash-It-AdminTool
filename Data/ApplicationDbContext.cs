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
        public DbSet<Reservacion> Reservaciones { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Empleado
            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.ToTable("Empleado");
                entity.Property(e => e.SalarioBase).HasColumnType("decimal(10,2)");
            });

            //Configuracion para Cliente 
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Cliente");
                entity.Property(e => e.FechaRegistro).HasColumnName("FechaRegistro");
            });

            // Configuración para Reservacion
            modelBuilder.Entity<Reservacion>(entity =>
            {
                entity.ToTable("Reservacion");

                entity.HasKey(r => r.Id);

                entity.Property(r => r.Id).HasColumnName("id");
                entity.Property(r => r.ClienteId).HasColumnName("cliente_id");
                entity.Property(r => r.FechaHora).HasColumnName("fecha_hora");
                entity.Property(r => r.Mesa).HasColumnName("mesa");
                entity.Property(r => r.Estado).HasColumnName("estado");
                entity.Property(r => r.RegistradoPor).HasColumnName("registrado_por");

                entity.HasOne(r => r.Cliente)
                      .WithMany()
                      .HasForeignKey(r => r.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);

                
            });

        }

    }
    }