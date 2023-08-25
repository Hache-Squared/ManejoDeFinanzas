using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }
        [Display(Name ="Nombre")]
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "El campo {0} debe estar entre {2} y {1} caracteres")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }


        /*
         Pruebas de validaciones por defecto
         */
        [Required(ErrorMessage ="{0} es requerido")]
        [EmailAddress(ErrorMessage = "{0} No es valido")]
        public string Email { get; set; }

        [Range(minimum:18, maximum:130, ErrorMessage = "El valor debe ser entre {1} y {2}")]
        public int Edad { get; set; }

        [Url(ErrorMessage ="{0} No valida")]
        public string Url { get; set; }

        [Display(Name ="Tarjeta de Credito")]
        [CreditCard(ErrorMessage ="Tarjeta no valida")]
        public string TarjetaDeCredito { get; set; }
    }
}
