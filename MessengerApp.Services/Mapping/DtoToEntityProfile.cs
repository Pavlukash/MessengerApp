using AutoMapper;
using MessengerApp.Domain.Entities;
using MessengerApp.Domain.Models;

namespace MessengerApp.Services.Mapping;

public sealed class DtoToEntityProfile : Profile
{
    public DtoToEntityProfile()
    {
        CreateMap<UserDto, UserEntity>();
    }
}