using System;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;

namespace BookMe.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}
