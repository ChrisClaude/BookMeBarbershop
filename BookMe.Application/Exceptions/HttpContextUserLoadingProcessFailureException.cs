namespace BookMe.Application.Exceptions;

public class HttpContextUserLoadingProcessFailureException: Exception
{
    public HttpContextUserLoadingProcessFailureException(string message): base(message)
    {
    }
}
