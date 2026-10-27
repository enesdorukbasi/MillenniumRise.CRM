using Application.Abstractions.Mappers;
using Application.Features.AppUserFeature.DTOs;
using Domain.Entities;

namespace Application.Features.AppUserFeature.Mappers;

public sealed class AppUserMapper : IMapper<AppUser, AppUserDto>
{
    public AppUserDto Map(AppUser entity)
    {
        return new AppUserDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            FullName = entity.FullName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            LastLoginDate = entity.LastLoginDate,
            LastLoginIp = entity.LastLoginIp,
            LastLoginDevice = entity.LastLoginDevice
        };
    }

    public List<AppUserDto> Map(List<AppUser> entities)
    {
        return entities.Select(Map).ToList();
    }
}
