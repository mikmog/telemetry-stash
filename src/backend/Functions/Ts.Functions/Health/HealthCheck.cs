//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.Extensions.Diagnostics.HealthChecks;
//using System.Net;

//namespace TelemetryStash.Functions.Health;

//public class HealthCheck(HealthCheckService healthCheckService)
//{
//    [Function("HealthCheck")]
//    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req, FunctionContext executionContext)
//    {
//        var status = await healthCheckService.CheckHealthAsync();

//        var response = req.CreateResponse();
//        response.StatusCode = status.Status == HealthStatus.Healthy ? HttpStatusCode.OK : HttpStatusCode.FailedDependency;
//        await response.WriteAsJsonAsync(new Response(Enum.GetName(status.Status), status.TotalDuration.TotalMilliseconds));
//        return response;
//    }

//    private record Response(string? Status, double Duration);
//}
