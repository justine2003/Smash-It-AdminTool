namespace SGA_Smash.Models
{
    public class Producto_Proveedor
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int ProveedorId { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int TiempoEntregaDias { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public Producto? Producto { get; set; }
        public Proveedor? Proveedor { get; set; }
    }
}
