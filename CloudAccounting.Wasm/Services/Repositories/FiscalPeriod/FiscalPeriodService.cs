using CloudAccounting.Wasm.Models.Authentication;

namespace CloudAccounting.Wasm.Services.Repositories.FiscalPeriod;

public class FiscalPeriodService
(
    IHttpClientFactory clientFactory,
    ILogger<FiscalPeriodService> logger
) : IFiscalPeriodService
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient("CloudAccountingAPI");
    private const string RelativePath = "/api/v1/fiscalyears";

    public async Task<Result> UpdateCurrentFiscalPeriodAsync(UpdateUserFiscalPeriodCommand command)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(command);
            StringContent content = new(jsonString, Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await _httpClient.PutAsync($"{RelativePath}", content);

            response.EnsureSuccessStatusCode();

            return Result.Success();

        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode.HasValue)
            {
                logger.LogError("FiscalPeriodService.UpdateCurrentFiscalPeriodAsync: Status Code: {statusCode}", e.StatusCode.Value);
            }

            return Result.Failure(
                new Error("FiscalPeriodService.UpdateCurrentFiscalPeriodAsync", Helpers.GetExceptionMessage(e))
            );
        }
        catch (Exception ex)
        {
            var errMsg = Helpers.GetExceptionMessage(ex);
            logger.LogError("FiscalPeriodService.UpdateCurrentFiscalPeriodAsync: {errMsg}", errMsg);

            return Result.Failure(
                new Error("FiscalPeriodService.UpdateCurrentFiscalPeriodAsync", errMsg)
            );
        }


        // return await Task.FromResult(new Result { Success = true });
    }
}