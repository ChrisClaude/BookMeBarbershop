using System;
using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using BookMe.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BookMe.Application.Behaviors;

internal sealed class AuthRequestPipelineBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor) : IPipelineBehavior<TRequest, TResponse>
	where TRequest : AuthenticatedRequest<TResponse>
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (httpContextAccessor.HttpContext.Items[Constant.HTTP_CONTEXT_USER_ITEM_KEY] is UserDto userDto)
		{
			request.UserDTo = userDto;
		}
		else
		{
			throw new HttpContextUserLoadingProcessFailureException("User details not found in the request context.");
		}

		return await next();
	}
}
