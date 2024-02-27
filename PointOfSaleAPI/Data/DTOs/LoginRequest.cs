using System.ComponentModel.DataAnnotations;

namespace PointOfSaleAPI.Data.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Nombre de usuario requerido")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Contrasena requerida")]
        public string Contrasena { get; set; }

    }
}
