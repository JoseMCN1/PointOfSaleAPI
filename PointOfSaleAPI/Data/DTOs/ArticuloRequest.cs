namespace PointOfSaleAPI.Data.DTOs
{
    public class ArticuloRequest
    {
        public int ?Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public byte AplicaIva { get; set; }
    }
}
