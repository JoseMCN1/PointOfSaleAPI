using System;
using System.Collections.Generic;

namespace PointOfSaleAPI.Model
{
    public partial class Articulo
    {
        public Articulo()
        {
            ArticuloFacturas = new HashSet<ArticuloFactura>();
        }

        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public byte AplicaIva { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int Eliminado { get; set; }

        public virtual ICollection<ArticuloFactura> ArticuloFacturas { get; set; }
    }
}
