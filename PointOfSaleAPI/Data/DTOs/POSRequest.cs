namespace PointOfSaleAPI.Data.DTOs
{
    public class POSRequest
    {
        public decimal MontoTotal { get; set; }
        public decimal SubTotal { get; set; }
        public List<ArticuloFacturaRequest> Articulos { get; set; }
    }

}
