using AutoMapper;
using MessengerApp.Domain.Entities;
using MessengerApp.Domain.Models;

namespace MessengerApp.Services.Mapping;

public sealed class EntityToDtoProfile : Profile
{
    public EntityToDtoProfile()
    {
        CreateMap<UserEntity, UserDto>();
    }
}