using Microsoft.AspNetCore.Http;

namespace HospitalAPI.Infrastructure.Exceptions;

public class CustomHttpException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public class ConflictException(string message)
    : CustomHttpException(message, StatusCodes.Status409Conflict);

public class NotFoundException(string message)
    : CustomHttpException(message, StatusCodes.Status404NotFound);

public class BadRequestException(string message)
    : CustomHttpException(message, StatusCodes.Status400BadRequest);

public class UnauthorizedException(string message)
    : CustomHttpException(message, StatusCodes.Status401Unauthorized);
