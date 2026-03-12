using Application.Dto.Tasks;
using Application.Dto.Users;
using AutoMapper;
using Core.Entities;

namespace webapi.Configuration
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            //CreateMap<SourceObject, DestinationObject>();
            CreateMap<CreateUserInput, User>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<CreateTaskInput, TaskItem>().ReverseMap();
            CreateMap<TaskItemDto, TaskItem>().ReverseMap();
            // ... other mappings ...
        }

    }
}
