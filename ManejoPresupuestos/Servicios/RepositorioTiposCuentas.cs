using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id, int usuarioId);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentaOrdenados);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {

            //SELECT SCOPE_IDENTITY(); NOS RETORNA EL ID DEL ULTIMO REGISTRO CREADO
            //DAPPER ES EL RESPONSABLE DE CREAR LA RELACION ENTRE EL MODELO Y LA CONSULTA, 
            //POR EJEMPLO SI EN EL MODELO HAY UNA PROPIEDAD LLAMADA "Nombre" EL LO QUE HARA ES RELACIONARLA CON @Nombre
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                            "TiposCuentas_Insertar",
                                new
                                {
                                    usuarioId = tipoCuenta.UsuarioId,
                                    nombre = tipoCuenta.Nombre,

                                },
                                commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            //TRAER LO PRIMERO QUE SE ENCUENTRE O QUE POR DEFECTO RETORNE ALGO DEL VALOR SOLICITADO
            //Valor por defecto de int es 0
            //el parametro new { nombre,usuarioId } pondra en la consulta los valores respectivos en el orden correspondiente

            //La consulta retornara 1 si al menos hay un registro obtenido, y 0 si no hay nada ya que el valor por defecto de int es 0
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                                @"SELECT 1
                                                FROM TiposCuentas
                                                WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;",
                                                new { nombre,usuarioId });

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<TipoCuenta>(
                                @"SELECT id,Nombre, Orden  FROM TiposCuentas WHERE UsuarioId = @UsuarioId ORDER BY Orden;",
                                new { usuarioId }
                                );
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection( connectionString);

            await connection.ExecuteAsync(
                                        @"UPDATE TiposCuentas SET Nombre = @Nombre WHERE Id = @Id",
                                        tipoCuenta
                                        );

        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(
                                                @"SELECT Id, Nombre, Orden
                                                    FROM TiposCuentas
                                                    WHERE  Id = @id AND UsuarioId = @usuarioId",
                                                new { id, usuarioId }
                );
        }

        public async Task Borrar(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE Id = @Id AND UsuarioId = @usuarioId", new { id, usuarioId });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentaOrdenados)
        {
            //ESTAMOS PASANDO IEnumerable lo que quiere decir que:
            /*
             await connection.ExecuteAsync(query, tipoCuentaOrdenados);
                
            se hara dependiendo el tamaño de tipoCuentaOrdenados, se hace en automatico cada consulta.
             */
            var query = "UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentaOrdenados);
        }
    }
}
