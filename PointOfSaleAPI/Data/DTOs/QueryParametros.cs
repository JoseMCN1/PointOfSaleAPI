namespace PointOfSaleAPI.Data.DTOs
{
    public class QueryParametros
    {
        public string ?Code { get; set; } = null;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}
