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
        public DbSet<Notificacion> Notificacion { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Planilla> Planillas { get; set; }
        public DbSet<Vacacion> Vacaciones { get; set; }
        public DbSet<ContratoProveedor> ContratoProveedores { get; set; }
        public DbSet<Gasto> Gasto { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
         public DbSet<HistorialCambiosPlanilla> HistorialCambiosPlanillas { get; set; }
        public DbSet<Mesa> Mesas { get; set; }

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
            modelBuilder.Entity<Planilla>()
                .HasIndex(p => new { p.EmpleadoId, p.Mes, p.Anio })
                .IsUnique();

            // Relación Empleado - Planilla
            modelBuilder.Entity<Planilla>()
                .HasOne(p => p.Empleado)
                .WithMany()
                .HasForeignKey(p => p.EmpleadoId);

            // Relación HistorialCambiosPlanilla -> Empleado / Usuario
            modelBuilder.Entity<HistorialCambiosPlanilla>()
                .HasOne(h => h.Empleado)
                .WithMany()
                .HasForeignKey(h => h.EmpleadoId);

            modelBuilder.Entity<HistorialCambiosPlanilla>()
                .HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.UsuarioId);

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
                    .HasForeignKey(v => v.EmpleadoId)
                    .OnDelete(DeleteBehavior.NoAction);
                // NO navegación para AprobadoPor
            });

            //Configuración para ContratoProveedor
            modelBuilder.Entity<ContratoProveedor>(entity =>
            {
                entity.ToTable("ContratoProveedor");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasColumnName("id");
                entity.Property(c => c.ProveedorId).HasColumnName("proveedor_id");
                entity.Property(c => c.FechaInicio).HasColumnName("fecha_inicio");
                entity.Property(c => c.FechaFin).HasColumnName("fecha_fin");
                entity.Property(c => c.MontoTotal).HasColumnName("monto_total").HasColumnType("decimal(18,2)");
                entity.Property(c => c.Estado).HasColumnName("estado");
                entity.Property(c => c.RutaArchivo).HasColumnName("ruta_archivo");
                entity.HasOne(c => c.Proveedor)
                      .WithMany()
                      .HasForeignKey(c => c.ProveedorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Gasto>(entity =>
            {
                entity.ToTable("Gasto");

                entity.HasOne(g => g.RegistroEmpleado).WithMany().HasForeignKey(g => g.registrado_por);
            });

            // Configuración para Mesa
            modelBuilder.Entity<Mesa>(entity =>
            {
                entity.ToTable("Mesas");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).HasColumnName("id");
                entity.Property(m => m.Numero).HasColumnName("numero").HasMaxLength(10);
                entity.Property(m => m.Capacidad).HasColumnName("capacidad");
                entity.Property(m => m.Estado).HasColumnName("estado").HasMaxLength(20);
            });
        }

    }
}