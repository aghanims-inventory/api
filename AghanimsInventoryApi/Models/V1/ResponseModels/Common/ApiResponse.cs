using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AghanimsInventoryApi.Models.V1.ResponseModels.Common;

public record ApiResponse<T>
{
    private int _statusCode { get; set; }

    public T? Data { get; set; }

    public bool IsSuccessful { get; set; } = false;

    public ProblemDetails? Error { get; set; }

    private ApiResponse(HttpStatusCode statusCode)
    {
        _statusCode = (int)statusCode;
    }

    public static ApiResponse<T> Successful(HttpStatusCode statusCode, T data)
    {
        return new ApiResponse<T>(statusCode)
        {
            Data = data,
            IsSuccessful = true
        };
    }

    public static ApiResponse<T> Successful(HttpStatusCode statusCode)
    {
        return new ApiResponse<T>(statusCode)
        {
            IsSuccessful = true
        };
    }

    public static ApiResponse<T> Unsuccessful(HttpStatusCode statusCode, ProblemDetails problemDetails)
    {
        return new ApiResponse<T>(statusCode)
        {
            Error = problemDetails
        };
    }

    public static ApiResponse<T> Unsuccessful(HttpStatusCode statusCode)
    {
        return new ApiResponse<T>(statusCode);
    }

    public int GetStatusCode()
    {
        return _statusCode;
    }
}

public record NoData
{
    public object? Data { get; set; }

    public bool IsSuccessful { get; set; }

    public ProblemDetails? Error { get; set; }
}

public record ApiResponse
{
    private int _statusCode { get; set; }

    public object? Data { get; set; }

    public bool IsSuccessful { get; set; }

    public ProblemDetails? Error { get; set; }

    private ApiResponse(HttpStatusCode statusCode)
    {
        _statusCode = (int)statusCode;
    }

    public static ApiResponse Successful(HttpStatusCode statusCode)
    {
        return new ApiResponse(statusCode)
        {
            IsSuccessful = true
        };
    }

    public static ApiResponse Unsuccessful(HttpStatusCode statusCode, ProblemDetails problemDetails)
    {
        return new ApiResponse(statusCode)
        {
            Error = problemDetails
        };
    }

    public static ApiResponse Unsuccessful(HttpStatusCode statusCode)
    {
        return new ApiResponse(statusCode);
    }

    public int GetStatusCode()
    {
        return _statusCode;
    }
}
