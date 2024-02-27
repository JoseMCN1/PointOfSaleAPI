using System.ComponentModel.DataAnnotations;

namespace PointOfSaleAPI.Data.DTOs
{
    public class RegistroRequest
    {
        [Required(ErrorMessage = "Nombre de usuario requerido")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Contrasena requerida")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Contrasena debe tener al menos 8 caracteres con letras, numeros y caracteres especiales")]
        public string Contrasena { get; set; }

    }
}
