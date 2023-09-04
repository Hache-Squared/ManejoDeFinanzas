using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ManejoPresupuestos.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Crear(Transaccion transaccion);
    }
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;


        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync <int>(
                "Transacciones_Insertar",
                new {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: CommandType.StoredProcedure
                );

            transaccion.Id = id;
        }
    }
}
