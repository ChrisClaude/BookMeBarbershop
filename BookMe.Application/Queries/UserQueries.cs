using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Mappings;

namespace BookMe.Application.Queries;

public class UserQueries(IRepository<User> repository) : IUserQueries
{
    public async Task<Result<UserDto>> GetUserAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
        {
            return Result<UserDto>.Failure(Error.NotFound("User not found"), ErrorType.NotFound);
        }

        return Result.Success(user.MapToDto());
    }
}
