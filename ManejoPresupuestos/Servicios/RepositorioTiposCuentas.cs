using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        void Crear(TipoCuenta tipoCuenta);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public void Crear(TipoCuenta tipoCuenta)
        {

            //SELECT SCOPE_IDENTITY(); NOS RETORNA EL ID DEL ULTIMO REGISTRO CREADO
            //DAPPER ES EL RESPONSABLE DE CREAR LA RELACION ENTRE EL MODELO Y LA CONSULTA, 
            //POR EJEMPLO SI EN EL MODELO HAY UNA PROPIEDAD LLAMADA "Nombre" EL LO QUE HARA ES RELACIONARLA CON @Nombre
            using var connection = new SqlConnection(connectionString);
            var id = connection.QuerySingle<int>($@"INSERT INTO TiposCuentas(Nombre, UsuarioId, Orden)
                                                    VALUES (@Nombre, @UsuarioId, 0);
                                                     SELECT SCOPE_IDENTITY();",
                                                     tipoCuenta
                                                    );

            
        }

    }
}
