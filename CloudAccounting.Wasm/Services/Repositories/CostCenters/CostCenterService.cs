using CloudAccounting.Wasm.Models.CostCenters;

namespace CloudAccounting.Wasm.Services.Repositories.CostCenters;

public class CostCenterService
(
    IHttpClientFactory clientFactory,
    ILogger<CostCenterService> logger
) : ICostCenterService
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient("CloudAccountingAPI");
    private const string RelativePath = "/api/v1/costcenters";

    public async Task<Result<List<CostCenterDto>>> RetrieveAllAsync(int companyCode)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{RelativePath}/{companyCode}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<CostCenterDto>>();
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode.HasValue)
            {
                logger.LogError("CostCenterService.RetrieveAllAsync: Status Code: {statusCode}", e.StatusCode.Value);
            }

            return Result.Failure<List<CostCenterDto>>(
                new Error("CostCenterService.RetrieveAllAsync", Helpers.GetExceptionMessage(e))
            );
        }
        catch (Exception ex)
        {
            var errMsg = Helpers.GetExceptionMessage(ex);
            logger.LogError("CostCenterService.RetrieveAllAsync: {errMsg}", errMsg);

            return Result.Failure<List<CostCenterDto>>(
                new Error("CostCenterService.RetrieveAllAsync", errMsg)
            );
        }
    }

    public async Task<Result<CostCenterDto>> RetrieveByIdAsync(int companyCode, string costCenterCode)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{RelativePath}/{companyCode}/{costCenterCode}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CostCenterDto>();
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode.HasValue)
            {
                logger.LogError("CostCenterService.RetrieveByIdAsync: Status Code: {statusCode}", e.StatusCode.Value);
            }

            return Result.Failure<CostCenterDto>(
                new Error("CostCenterService.RetrieveByIdAsync", Helpers.GetExceptionMessage(e))
            );
        }
        catch (Exception ex)
        {
            var errMsg = Helpers.GetExceptionMessage(ex);
            logger.LogError("CostCenterService.RetrieveByIdAsync: {errMsg}", errMsg);

            return Result.Failure<CostCenterDto>(
                new Error("CostCenterService.RetrieveByIdAsync", errMsg)
            );
        }
    }

    public async Task<Result> UpdateAsync(UpdateCostCenterCommand command)
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
                logger.LogError("CostCenterService.UpdateAsync: Status Code: {statusCode}", e.StatusCode.Value);
            }

            return Result.Failure(
                new Error("CostCenterService.UpdateAsync", Helpers.GetExceptionMessage(e))
            );
        }
        catch (Exception ex)
        {
            var errMsg = Helpers.GetExceptionMessage(ex);
            logger.LogError("CostCenterService.UpdateAsync: {errMsg}", errMsg);

            return Result.Failure(
                new Error("CostCenterService.UpdateAsync", errMsg)
            );
        }
    }

    public async Task<Result<CostCenterDto>> CreateAsync(CostCenterDto command)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(command);
            StringContent content = new(jsonString, Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await _httpClient.PostAsync($"{RelativePath}", content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CostCenterDto>();
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode.HasValue)
            {
                logger.LogError("CostCenterService.CreateAsync: Status Code: {statusCode}", e.StatusCode.Value);
            }

            return Result.Failure<CostCenterDto>(
                new Error("CostCenterService.CreateAsync", Helpers.GetExceptionMessage(e))
            );
        }
        catch (Exception ex)
        {
            var errMsg = Helpers.GetExceptionMessage(ex);
            logger.LogError("CostCenterService.CreateAsync: {errMsg}", errMsg);

            return Result.Failure<CostCenterDto>(
                new Error("CostCenterService.CreateAsync", errMsg)
            );
        }
    }
    
    public async Task<Result> DeleteAsync(int companyCode, string costCenterCode)
    {
        try
        {
            using HttpResponseMessage response = await _httpClient.DeleteAsync($"{RelativePath}/{companyCode}/{costCenterCode}");

            response.EnsureSuccessStatusCode();

            return Result.Success();
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode.HasValue)
            {
                logger.LogError("CostCenterService.DeleteAsync: Status Code: {statusCode}", e.StatusCode.Value);
            }

            return Result.Failure(
                new Error("CostCenterService.DeleteAsync", Helpers.GetExceptionMessage(e))
            );
        }
        catch (Exception ex)
        {
            var errMsg = Helpers.GetExceptionMessage(ex);
            logger.LogError("CostCenterService.DeleteAsync: {errMsg}", errMsg);

            return Result.Failure(
                new Error("CostCenterService.DeleteAsync", errMsg)
            );
        }
    }
}