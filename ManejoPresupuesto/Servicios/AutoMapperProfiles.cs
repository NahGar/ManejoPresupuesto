using AutoMapper;
using ManejoPresupuesto.Models;

//Para usar AutoMapper hay que agregar una línea: builder.Services.AddAutoMapper(typeof(Program)); en Program.cs

namespace ManejoPresupuesto.Servicios {
    public class AutoMapperProfiles : Profile {

        public AutoMapperProfiles() {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
            //ReverserMap inidca que se va mapear en el orden indicado y al revés
            CreateMap<TransaccionActualizacionViewModel,Transaccion>().ReverseMap();
        }
    }
}
