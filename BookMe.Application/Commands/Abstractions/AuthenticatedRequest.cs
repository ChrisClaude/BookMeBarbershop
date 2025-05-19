using System;
using BookMe.Application.Common.Dtos;
using MediatR;

namespace BookMe.Application.Commands.Abstractions;

public abstract class AuthenticatedRequest<T> : IRequest<T>
{
    public UserDto UserDto { get; set; }
}
