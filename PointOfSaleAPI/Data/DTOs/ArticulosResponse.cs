namespace PointOfSaleAPI.Data.DTOs
{
    public class ArticulosResponse
    {
        public int Total { get; set; }
        public List<ArticuloResponse> Articulos { get; set; }

        public int CurrentPage { get; set; }

        public int ItemsPerPage { get; set; }
    }
}
