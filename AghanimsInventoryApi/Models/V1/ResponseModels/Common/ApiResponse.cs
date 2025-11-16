using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AghanimsInventoryApi.Models.V1.ResponseModels.Common;

public record ApiResponse
{
    private int StatusCode { get; set; }

    public object? Data { get; set; }

    public bool IsSuccessful { get; set; } = false;

    public ProblemDetails? Error { get; set; }

    private ApiResponse(HttpStatusCode statusCode)
    {
        StatusCode = (int)statusCode;
    }

    public static ApiResponse Successful(HttpStatusCode statusCode, object data)
    {
        return new ApiResponse(statusCode)
        {
            Data = data,
            IsSuccessful = true
        };
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
        return StatusCode;
    }
}

public record NoData { }

public record ApiResponse<T>
{
    public T? Data { get; set; }

    public bool IsSuccessful { get; set; } = false;

    public ProblemDetails? Error { get; set; }
}
