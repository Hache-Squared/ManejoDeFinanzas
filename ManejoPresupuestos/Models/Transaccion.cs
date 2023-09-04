﻿
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestos.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }

        /*
        [Display(Name = "Fecha Transaccion")]
        [DataType(DataType.DateTime)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm tt")); //TAMBIEN SE PUEDE REMPLAZAR POR "G"

        */
        [Display(Name = "Fecha Transaccion")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;


        public decimal Monto { get; set; }

        [Range(0, maximum: int.MaxValue, ErrorMessage ="Debe seleccionar una categoria")]
        public int CategoriaId { get; set; }

        [StringLength(maximumLength: 1000, ErrorMessage ="La nota no debe pasar de {1} caracteres")]
        public string Nota { get; set; }


        [Range(0, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        public int CuentaId { get; set; }
    }
}
