using AutoMapper;
using MyApp.Business.DTO.Emails;

namespace MyApp.Business.Automapper
{
    public class EmailsProfile : Profile
    {
        public EmailsProfile()
        {
            CreateMap<SendWithIdParameters, SendWithParametersParameters>();
            CreateMap<SendWithSubjectAndBodyParameters,SendWithParametersParameters>();
        }
    }
}
