using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestos.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCategorias repositorioCategorias;

        public TransaccionesController(
            IRepositorioTransacciones repositorioTransacciones, 
            IRepositorioCuentas repositorioCuentas,
            IServicioUsuarios servicioUsuarios,
            IRepositorioCategorias repositorioCategorias
            )
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.repositorioCuentas = repositorioCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCategorias = repositorioCategorias;
        }

        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
            return View(modelo);

        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
        {
            var cuentas = await repositorioCuentas.Buscar(usuarioId);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString())).ToList();
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
        {
            var categorias = await repositorioCategorias.Obtener(usuarioId, tipoOperacion);

            return categorias.Select(x => new SelectListItem(x.Nombre,x.Id.ToString())).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);

            return Ok(categorias); //DECIMOS QUE TODO ESTA BIEN Y ADJUNTAMOS LA DATA OBTENIDA
        }

    }
}
