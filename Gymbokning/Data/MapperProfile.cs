using AutoMapper;
using Gymbokning.Models;
using Gymbokning.ViewModels;

namespace Gymbokning.Data
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // CreateMap<GymClass, GymClassesViewModel>();

            CreateMap<GymClass, GymClassesViewModel>()
                .ForMember(dest => dest.Attending, from => from.MapFrom(
                    (src, dest, _, context) => src.AttendingMembers.Any(a => a.ApplicationUserId == context.Items["Id"].ToString())));
        }
    }
}
