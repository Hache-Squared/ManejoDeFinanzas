using ManejoPresupuestos.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class TipoCuenta // : IValidatableObject
    {
        public int Id { get; set; }

        
        [Required(ErrorMessage ="El campo {0} es requerido")]
        //Validacion Personalizada
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }


        //Esta es una validacion de tipo Modelo,
        //es por eso hay una etiqueta con atributo 'asp-validation-summary="ModelOnly"' que muestra errores del modelo

        //public ienumerable<validationresult> validate(validationcontext validationcontext)
        //{
        //    if(nombre != null && nombre.length > 0)
        //    {
        //        var primeraletra = nombre[0].tostring();
        //        if(primeraletra != primeraletra.toupper())
        //        {
        //            yield return new validationresult("la primera letra debe ser mayuscula",new[]
        //            {
        //                nameof(nombre)
        //            });
        //        }
        //    }
        //}


    }
}
