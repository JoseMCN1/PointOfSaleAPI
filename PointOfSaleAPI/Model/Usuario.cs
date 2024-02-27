using System;
using System.Collections.Generic;

namespace PointOfSaleAPI.Model
{
    public partial class Usuario
    {
        public Usuario()
        {
            Facturas = new HashSet<Factura>();
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string ContrasenaSalt { get; set; } = null!;
        public string ContrasenaHash { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public int IntentosLogin { get; set; }
        public string? Token { get; set; }
        public int Eliminado { get; set; }

        public virtual ICollection<Factura> Facturas { get; set; }
    }
}
