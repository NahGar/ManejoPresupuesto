namespace ManejoPresupuesto.Models {
    public class TransaccionActualizacionViewModel : TransaccionCreacionViewModel {

        public int CuentaIdAnterior { get; set; }
        public decimal MontoAnterior { get; set; }
    }
}
