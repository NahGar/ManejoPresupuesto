using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models {
    public class TipoCuenta /*: IValidatableObject*/ {

        public int Id { get; set; }

        //{0} se sustituye por el nombre del campo
        [Required(ErrorMessage = "Falta indicar {0}")]
        [StringLength(maximumLength: 50, ErrorMessage = "{0} no debe superar los {1} caracteres")]
        //Con Display se puede establecer el texto del label que esté asociado con asp-for 
        //[Display(Name = "Nombre")]
        [PrimeraLetraMayuscula]
        //AdditionalFields es para que VerificarExisteTipoCuenta reciba el Id (se pueden enviar más campos separados por comas)
        [Remote(action: "VerificarExisteTipoCuenta", controller:"TiposCuentas", AdditionalFields = "Id")]
        public string Nombre { get; set; }

        public int UsuarioId { get; set; }

        public int Orden { get; set; }

        /*
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if(Nombre != null && Nombre.Length > 0) {
                var primeraLetra = Nombre[0].ToString();
                if(primeraLetra != primeraLetra.ToUpper() ) {
                    //error del modelo
                    //yield return new ValidationResult("La primera letra debe ser mayúscula");
                    //error de un atributo
                    yield return new ValidationResult("La primera letra debe ser mayúscula", new[] { nameof(Nombre) });
                }
                        
            }
        }
        */

        /* Pruebas otras validaciones por defecto */
        /*
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        public string Email { get; set; }

        [Range(minimum:18, maximum:130, ErrorMessage = "El valor debe estar entre {1} y {2}")]
        public int Edad { get; set; }

        [Url(ErrorMessage = "El campo debe contener una URL válida")]
        public string Url { get; set; }

        [CreditCard(ErrorMessage = "La tarjeta de crédito no es válida")]
        [Display(Name = "Tarjeta de crédito")]
        public string TarjetaDeCredito { get; set; }
        */


    }
}
