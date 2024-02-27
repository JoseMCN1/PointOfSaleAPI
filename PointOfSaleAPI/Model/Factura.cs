using System;
using System.Collections.Generic;

namespace PointOfSaleAPI.Model
{
    public partial class Factura
    {
        public Factura()
        {
            ArticuloFacturas = new HashSet<ArticuloFactura>();
        }

        public int Id { get; set; }
        public string Consecutivo { get; set; } = null!;
        public string NumeroFactura { get; set; } = null!;
        public int VendedorId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public decimal Subtotal { get; set; }
        public decimal MontoTotal { get; set; }

        public virtual Usuario Vendedor { get; set; } = null!;
        public virtual ICollection<ArticuloFactura> ArticuloFacturas { get; set; }
    }
}
