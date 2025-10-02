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
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Planilla> Planillas { get; set; }
        public DbSet<Vacacion> Vacaciones { get; set; }

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

            // Configuración para Planilla
            modelBuilder.Entity<Planilla>(entity =>
            {
                entity.ToTable("Planilla");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.EmpleadoId).HasColumnName("empleado_id");
                entity.Property(p => p.Mes).HasColumnName("mes");
                entity.Property(p => p.Anio).HasColumnName("anio");
                entity.Property(p => p.SalarioBase).HasColumnName("salario_base").HasColumnType("decimal(10,2)");
                entity.Property(p => p.Bonificaciones).HasColumnName("bonificaciones").HasColumnType("decimal(10,2)");
                entity.Property(p => p.Deducciones).HasColumnName("deducciones").HasColumnType("decimal(10,2)");

                
                entity.HasOne(p => p.Empleado)
                      .WithMany()
                      .HasForeignKey(p => p.EmpleadoId)
                      .HasConstraintName("FK_Planilla_Empleado");
            });

            // Configuración para Vacacion
            modelBuilder.Entity<Vacacion>(entity =>
            {
                entity.ToTable("Vacacion");
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Id).HasColumnName("id");
                entity.Property(v => v.EmpleadoId).HasColumnName("empleado_id");
                entity.Property(v => v.FechaInicio).HasColumnName("fecha_inicio");
                entity.Property(v => v.FechaFin).HasColumnName("fecha_fin");
                entity.Property(v => v.Estado).HasColumnName("estado");
                entity.Property(v => v.DiasSolicitados).HasColumnName("dias_solicitados");
                entity.Property(v => v.FechaSolicitud).HasColumnName("fecha_solicitud");
                entity.Property(v => v.AprobadoPor).HasColumnName("aprobado_por");

                entity.HasOne(v => v.Empleado)
                    .WithMany()
                    .HasForeignKey(v => v.EmpleadoId);

                entity.HasOne(v => v.Aprobador)
                    .WithMany()
                    .HasForeignKey(v => v.AprobadoPor);
            });

        }

    }
    }