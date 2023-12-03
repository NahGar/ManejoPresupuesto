namespace ManejoPresupuesto.Models {
    public class PaginacionRespuesta {

        public int pagina { get; set; } = 1;
        public int registrosPorPagina { get; set; } = 10;
        public int cantidadTotalRegistros { get; set; }
        public int cantidadTotalPaginas => (int)Math.Ceiling((double)cantidadTotalRegistros / registrosPorPagina);
        public string BaseURL { get; set; }
    }

    //este generico es para tener los registros resultantes
    public class PaginacionRespuesta<T>: PaginacionRespuesta {

        public IEnumerable<T> Elementos { get; set; }
    }
}
