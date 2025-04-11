namespace BookMe.Application.Common.Errors;
public enum ErrorType
{
    None = 0,
    NotFound = 1,
    InternalServerError = 2,
    BadRequest = 3,
    Unauthorized = 4,
}
