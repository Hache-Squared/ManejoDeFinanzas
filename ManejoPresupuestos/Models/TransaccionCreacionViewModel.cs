﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class TransaccionCreacionViewModel : Transaccion
    {
        public IEnumerable<SelectListItem> Cuentas { get; set; }
        public IEnumerable<SelectListItem> Categorias { get;set; }

        [Display(Name = "Tipo de operación")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;
    }
}
