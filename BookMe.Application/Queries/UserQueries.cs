using System;
using AutoMapper;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Errors;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Interfaces.Queries;

namespace BookMe.Application.Queries;

public class UserQueries(IRepository<User> repository, IMapper mapper) : IUserQueries
{
    public async Task<Result<UserDto>> GetUserAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
        {
            return Result<UserDto>.Failure(Error.NotFound("User not found"), ErrorType.NotFound);
        }

        return Result<UserDto>.Success(mapper.Map<UserDto>(user));
    }
}
