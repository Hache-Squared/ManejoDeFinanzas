using AutoMapper;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace ManejoPresupuestos.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IMapper mapper;

        public TransaccionesController(
            IRepositorioTransacciones repositorioTransacciones, 
            IRepositorioCuentas repositorioCuentas,
            IServicioUsuarios servicioUsuarios,
            IRepositorioCategorias repositorioCategorias,
            IMapper mapper
            )
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.repositorioCuentas = repositorioCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCategorias = repositorioCategorias;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index(int mes,int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes <= 0 || mes > 12 || año <= 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(año, mes, 1);
            }

            //AGREGA UN MES A LA FECHA INICIAL Y LUEGO QUITALE UN DIA ES DECIR ELIGE EL ULTIMO DIA DEL MES DE FECHAINICIO
            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId= usuarioId,
                FechaInicio = fechaInicio,
                FechaFin= fechaFin,
            };

            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(parametro);

            var modelo = new ReporteTransaccionesDetalladas();

            //ORDENAMOS DE MANERA DESENDENTE Y AGRUPAMOS POR FECHA PARA OBTENER TODAS LAS TRANSACCIONES DEL MES,
            //POR ULTIMO CREAMOS UNA CLASE QUE NOS AYUDARA A ENCAPSULAR LAS TRANSACCIONES POR DIA
            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
                                         .GroupBy(x => x.FechaTransaccion)
                                         .Select(grupo => new ReporteTransaccionesDetalladas.TransaccionesPorFecha()
                                         {
                                             FechaTransaccion = grupo.Key,
                                             Transacciones = grupo.AsEnumerable(),
                                         });
            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;

            //dato de fecha de hace un mes
            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;

            //dato de fecha de un mes en adelante
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.añoPosterior = fechaInicio.AddMonths(1).Year;

            ViewBag.urlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;

            return View(modelo);
        }

        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
            return View(modelo);

        }

        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.UsuarioId = usuarioId;
            if(modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.Monto *= -1;
            }

            await repositorioTransacciones.Crear(modelo);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno = null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);

            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTransacciones.Borrar(id);

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");

            }
            else
            {
                return LocalRedirect(urlRetorno);
            }

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


        [HttpGet]
        public async Task<IActionResult> Editar(int id, string urlRetorno = null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);

            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            
            var modelo = mapper.Map<TransaccionActualizacionViewModel>(transaccion);

            modelo.MontoAnterior = modelo.Monto;

            if(modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1;
            }
            modelo.CuentaAnteriorId = transaccion.Id;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.UrlRetorno = urlRetorno;

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizacionViewModel modelo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if (!ModelState.IsValid)
            {
                //modelo.CuentaAnteriorId = transaccion.Id;
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                return View(modelo);
            }

            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = mapper.Map<Transaccion>(modelo);

            if(modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }

            await repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);

            if (string.IsNullOrEmpty(modelo.UrlRetorno))
            {
                return RedirectToAction("Index");

            }
            else
            {
                return LocalRedirect(modelo.UrlRetorno);
            }

        }



    }
}
