using ManejoPresupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models {
    public class Cuenta {

        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50)]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        [Display(Name = "Tipo cuenta")]
        public int TipoCuentaId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 1000)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        //para que funcione el Buscar de RepositorioCuentas. Porque hace join a TiposCuentas: tc.Nombre AS TipoCuenta
        public string TipoCuenta { get; set; }
    }
}
