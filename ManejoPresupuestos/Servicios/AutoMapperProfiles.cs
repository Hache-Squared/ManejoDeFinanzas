using AutoMapper;
using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
            CreateMap<TransaccionActualizacionViewModel, Transaccion>().ReverseMap(); //configura la conversion en ambas posiciones
        }
    }
}
