namespace ManejoPresupuesto.Models {
    public class PaginacionViewModel {

        public int pagina { get; set; } = 1;
        public int registrosPorPagina = 10;

        public readonly int cantidadMaximaRegistrosPorPagina = 50;
        public int RegistrosPorPagina {
            get {
                return registrosPorPagina;
            }
            set {
                registrosPorPagina = (value > cantidadMaximaRegistrosPorPagina) ? cantidadMaximaRegistrosPorPagina : value;
            }
        }

        public int RegistrosASaltar => registrosPorPagina * (pagina - 1);

    }
}
