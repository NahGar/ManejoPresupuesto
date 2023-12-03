using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios {

    public interface IRepositorioTiposCuentas {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId, int id = 0);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }

    public class RepositorioTiposCuentas : IRepositorioTiposCuentas {

        private readonly string connectionString;

        public RepositorioTiposCuentas(IConfiguration configuration) {

            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta) {
            
            using var connection = new SqlConnection(connectionString);

            //var id = await connection.QuerySingleAsync<int>(@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden) 
            //                                     VALUES (@Nombre, @UsuarioId, 0);
            //                                     SELECT SCOPE_IDENTITY()", tipoCuenta);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar", 
                                                            new {
                                                                usuarioId = tipoCuenta.UsuarioId,
                                                                nombre = tipoCuenta.Nombre
                                                            },
                                                            commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;

        }

        public async Task<bool> Existe(string nombre, int usuarioId, int id = 0) {

            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM TiposCuentas 
                                         WHERE Nombre=@Nombre AND UsuarioId=@UsuarioId AND Id <> @Id", 
                                         new {nombre, usuarioId, id});
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId) {

            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden FROM TiposCuentas 
                                                 where UsuarioId=@UsuarioId ORDER BY Orden", new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta) {

            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas set Nombre=@Nombre 
                                            where Id=@Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId) {

            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden FROM TiposCuentas
                                            where Id=@Id and UsuarioId=@UsuarioId", new { id, usuarioId });
        }

        public async Task Borrar(int id) {

            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM TiposCuentas
                                            where Id=@Id", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados ) {

            //Dapper va a ejecutar el update por cada tipoCuenta que esté en el IEnumerable
            using var connection = new SqlConnection(connectionString);
            var query = "UPDATE TiposCuentas SET Orden = @Orden where Id = @Id;";
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }

    }
}
