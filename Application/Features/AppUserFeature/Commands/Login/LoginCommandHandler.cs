using Application.Abstractions.Authentication;
using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Features.AppUserFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.AppUserFeature.Commands.Login;

public sealed class LoginCommandHandler(IRepository<AppUser> appUserRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IValidator<LoginCommand> validator, IMapper<AppUser, AppUserDto> appUserMapper) : IRequestHandler<LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var appUser = await appUserRepository.SingleOrDefaultAsync(user => user.Email == request.Email);

        if (appUser is null || !passwordHasher.Verify(request.Password, appUser.HashedPassword, appUser.PasswordSalt))
            throw new UnauthorizedException("E-posta adresi veya parola hatalı.");

        appUser.LastLoginDate = DateTimeOffset.UtcNow;

        if (passwordHasher.NeedsRehash(appUser.HashedPassword))
        {
            var (hash, saltBase64) = passwordHasher.Hash(request.Password);
            appUser.HashedPassword = hash;
            appUser.PasswordSalt = saltBase64;
        }

        appUserRepository.Update(appUser);
        await appUserRepository.SaveChangesAsync();

        var response = jwtTokenGenerator.GenerateToken(appUser);
        response.User = appUserMapper.Map(appUser);

        return response;
    }
}