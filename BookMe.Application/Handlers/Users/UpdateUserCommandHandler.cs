using System.Linq.Expressions;
using BookMe.Application.Commands.Users;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Mappings;
using MediatR;

namespace BookMe.Application.Handlers.Users;

public class UpdateUserCommandHandler(IRepository<User> repository)
    : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await repository.GetByIdAsync(request.UserDto.Id);
        if (user == null)
        {
            return Result<UserDto>.Failure(Error.NotFound("User not found"), ErrorType.NotFound);
        }

        await repository.UpdateSpecificPropertiesAsync(
            user.Id,
            new Dictionary<Expression<Func<User, object>>, object>
            {
                { x => x.Name, request.Name },
                { x => x.Surname, request.Surname },
            },
            false
        );

        return Result.Success(user.MapToDto());
    }
}
