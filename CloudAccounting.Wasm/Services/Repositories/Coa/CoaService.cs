namespace CloudAccounting.Wasm.Services.Repositories.Coa;

public class CoaService(IHttpClientFactory clientFactory, ILogger<CoaService> logger) : ICoaService
{
    private readonly HttpClient _httpClient = clientFactory.CreateClient("CloudAccountingAPI");
    private const string RelativePath = "/api/v1/coa";

    public async Task<Result<PagedResponse<ChartOfAccountDto>>> RetrieveAllAsync(int pageNumber, int pageSize, int companyCode)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["pageNumber"] = pageNumber.ToString(),
            ["pageSize"] = pageSize.ToString(),
            ["companyCode"] = companyCode.ToString()
        };

        try
        {
            var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(RelativePath, queryParams!));

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Request successful for {RequestUri}", RelativePath);
                return await response.Content.ReadFromJsonAsync<PagedResponse<ChartOfAccountDto>>();
            }
            else
            {
                logger.LogError("Request failed with status code {StatusCode} for {RequestUri}", response.StatusCode, RelativePath);
                return Result.Failure<PagedResponse<ChartOfAccountDto>>(new Error("CoaService.RetrieveAllAsync", $"Request failed with status code {response.StatusCode}"));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while sending request to {RequestUri}", RelativePath);

            return Result.Failure<PagedResponse<ChartOfAccountDto>>(new Error("CoaService.RetrieveAllAsync", $"Exception occurred while sending request to {RelativePath}: {ex.Message}"));
        }
    }

    public async Task<Result<PagedResponse<ChartOfAccountDto>>> RetrieveAllAsync(int pageNumber, int pageSize, int companyCode, string accountNumber)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["pageNumber"] = pageNumber.ToString(),
            ["pageSize"] = pageSize.ToString(),
            ["companyCode"] = companyCode.ToString(),
            ["accountNumber"] = accountNumber
        };

        var requestUri = QueryHelpers.AddQueryString(RelativePath, queryParams!);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        logger.LogInformation("Sending request to {RequestUri}", requestUri);

        try
        {
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Request successful for {RequestUri}", requestUri);
                var result = JsonSerializer.Deserialize<Result<PagedResponse<ChartOfAccountDto>>>(content);
                return result!;
            }
            else
            {
                logger.LogError("Request failed with status code {StatusCode} for {RequestUri}", response.StatusCode, requestUri);

                return Result.Failure<PagedResponse<ChartOfAccountDto>>(new Error("CoaService.RetrieveAllAsync", $"Request failed with status code {response.StatusCode}"));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while sending request to {RequestUri}", requestUri);

            return Result.Failure<PagedResponse<ChartOfAccountDto>>(new Error("CoaService.RetrieveAllAsync", $"Exception occurred while sending request to {requestUri}: {ex.Message}"));
        }
    }

    public async Task<Result<ChartOfAccountDto>> RetrieveByIdAsync(int companyCode, string accountNumber)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{RelativePath}/{companyCode}/{accountNumber}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ChartOfAccountDto>();
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode.HasValue)
            {
                logger.LogError("CoaService.RetrieveByIdAsync: Status Code: {statusCode}", e.StatusCode.Value);
            }

            return Result.Failure<ChartOfAccountDto>(
                new Error("CoaService.RetrieveByIdAsync", Helpers.GetExceptionMessage(e))
            );
        }
        catch (Exception ex)
        {
            var errMsg = Helpers.GetExceptionMessage(ex);
            logger.LogError("CoaService.RetrieveByIdAsync: {errMsg}", errMsg);

            return Result.Failure<ChartOfAccountDto>(
                new Error("CoaService.RetrieveByIdAsync", errMsg)
            );
        }
    }

    public async Task<Result<ChartOfAccountDto>> CreateAsync(CreateChartOfAccountCommand command)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(command);
            StringContent content = new(jsonString, Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await _httpClient.PostAsync($"{RelativePath}", content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ChartOfAccountDto>();
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode.HasValue)
            {
                logger.LogError("CoaService.CreateAsync: Status Code: {statusCode}", e.StatusCode.Value);
            }

            return Result.Failure<ChartOfAccountDto>(
                new Error("CoaService.CreateAsync", Helpers.GetExceptionMessage(e))
            );
        }
        catch (Exception ex)
        {
            var errMsg = Helpers.GetExceptionMessage(ex);
            logger.LogError("CoaService.CreateAsync: {errMsg}", errMsg);

            return Result.Failure<ChartOfAccountDto>(
                new Error("CoaService.CreateAsync", errMsg)
            );
        }
    }

    public async Task<Result> UpdateAsync(UpdateChartOfAccountCommand command)
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
                logger.LogError("CoaService.UpdateAsync: Status Code: {statusCode}", e.StatusCode.Value);
            }

            return Result.Failure(
                new Error("CoaService.UpdateAsync", Helpers.GetExceptionMessage(e))
            );
        }
        catch (Exception ex)
        {
            var errMsg = Helpers.GetExceptionMessage(ex);
            logger.LogError("CoaService.UpdateAsync: {errMsg}", errMsg);

            return Result.Failure(
                new Error("CoaService.UpdateAsync", errMsg)
            );
        }
    }

    public async Task<Result> DeleteAsync(DeleteChartOfAccountCommand command)
    {
        try
        {
            using HttpResponseMessage response = await _httpClient.DeleteAsync($"{RelativePath}/{command.CompanyCode}/{command.AccountCode}");

            response.EnsureSuccessStatusCode();

            return Result.Success();
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode.HasValue)
            {
                logger.LogError("CoaService.DeleteAsync: Status Code: {statusCode}", e.StatusCode.Value);
            }

            return Result.Failure(
                new Error("CoaService.DeleteAsync", Helpers.GetExceptionMessage(e))
            );
        }
        catch (Exception ex)
        {
            var errMsg = Helpers.GetExceptionMessage(ex);
            logger.LogError("CoaService.DeleteAsync: {errMsg}", errMsg);

            return Result.Failure(
                new Error("CoaService.DeleteAsync", errMsg)
            );
        }
    }
}