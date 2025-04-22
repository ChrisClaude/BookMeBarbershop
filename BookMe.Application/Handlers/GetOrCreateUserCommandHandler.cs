using System;
using BookMe.Application.Commands;
using BookMe.Application.Common.Dtos;
using MediatR;

namespace BookMe.Application.Handlers;

public class GetOrCreateUserCommandHandler : IRequestHandler<GetOrCreateUserCommand, UserDto>
{
    public Task<UserDto> Handle(GetOrCreateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
