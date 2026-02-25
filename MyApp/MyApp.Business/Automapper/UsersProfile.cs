using AutoMapper;

using MyApp.Business.DTO.Users;
using MyApp.Entities;

namespace namasdev.Apps.Negocio.Automapper
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<AddParameters, User>();
            CreateMap<UpdateParameters, User>();
            CreateMap<SetDeletedParameters, User>();
            CreateMap<UnsetDeletedParameters, User>();
        }
    }
}
