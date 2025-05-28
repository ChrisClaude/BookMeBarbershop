using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;

namespace BookMe.Application.Interfaces.Queries;

public interface IUserQueries
{
    public Task<Result<UserDto>> GetUserAsync(Guid id);
}
