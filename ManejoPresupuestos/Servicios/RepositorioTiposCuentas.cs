using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
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
                                                 @"INSERT INTO TiposCuentas(Nombre, UsuarioId, Orden)
                                                    VALUES (@Nombre, @UsuarioId, 0);
                                                     SELECT SCOPE_IDENTITY();",
                                                     tipoCuenta
                                                    );

            
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

    }
}
