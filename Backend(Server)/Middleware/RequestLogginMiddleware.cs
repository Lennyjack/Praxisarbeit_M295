using Microsoft.AspNetCore.Http;
using Praxisarbeit_M295.Data;
using Praxisarbeit_M295.Models;
using System.Threading.Tasks;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var log = new Log
        {
            Endpoint = context.Request.Path,
            HttpMethod = context.Request.Method,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            await _next(context);
            log.StatusCode = context.Response.StatusCode.ToString();
        }
        catch (Exception ex)
        {
            log.StatusCode = "500";
            log.Message = ex.Message;
            throw;
        }
        finally
        {
            // Log speichern
            dbContext.Logs.Add(log);
            await dbContext.SaveChangesAsync();
        }
    }
}
