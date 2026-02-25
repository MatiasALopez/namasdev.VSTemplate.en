using System.Linq;

using AutoMapper;

using MyApp.Entities;
using MyApp.Business.DTO.Users;
using MyApp.Web.Portal.Models.Users;
using MyApp.Web.Portal.ViewModels.Users;

namespace MyApp.Web.Portal.Automapper
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.Role, src => src.MapFrom((u, vm) => MapUserRol(u)))
                .ReverseMap();

            CreateMap<User, UserItemModel>()
                .ForMember(dest => dest.Role, src => src.MapFrom((u, vm) => MapUserRol(u)));

            CreateMap<UserViewModel, AddParameters>();
            CreateMap<UserViewModel, UpdateParameters>();
        }

        private string MapUserRol(User user)
        {
            return user.Roles?.Select(r => r.Name).FirstOrDefault();
        }
    }
}