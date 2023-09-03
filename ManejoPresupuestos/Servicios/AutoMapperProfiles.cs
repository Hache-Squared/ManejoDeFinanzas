using AutoMapper;
using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
        }
    }
}
