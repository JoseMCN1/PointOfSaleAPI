using System;
using System.Collections.Generic;

namespace PointOfSaleAPI.Model
{
    public partial class ArticuloFactura
    {
        public int IdArticulo { get; set; }
        public int IdFactura { get; set; }
        public decimal MontoTotal { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public byte AplicaIva { get; set; }
        public DateTime FechaAgregado { get; set; }

        public virtual Articulo IdArticuloNavigation { get; set; } = null!;
        public virtual Factura IdFacturaNavigation { get; set; } = null!;
    }
}
