using System;
using BookMe.Application.Commands.Abstractions;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Enums;
using BookMe.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BookMe.Application.Behaviors;

internal sealed class AuthRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : AuthenticatedRequest<TResponse>
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public AuthRequestBehavior(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		if (_httpContextAccessor.HttpContext.Items[Constant.HTTP_CONTEXT_USER_ITEM_KEY] is UserDto userDto)
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
