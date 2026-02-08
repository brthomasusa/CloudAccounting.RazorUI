using System;

namespace CloudAccounting.Wasm.Services.Repositories.Company
{
    public class CompanyService
    (
        IHttpClientFactory factory,
        ILogger<CompanyService> logger
    ) : ICompanyService
    {
        private readonly HttpClient _httpClient = factory.CreateClient("CloudAccountingApi");
        private readonly ILogger<CompanyService>? _logger = logger;
        private const string relativePath = "/api/v1/companies";

        public async Task<Result<CompanyDetail>> GetCompanyByIdAsync(int companyCode)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{relativePath}/{companyCode}");

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<CompanyDetail>();
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string msg = $"A company with company code {companyCode} was not found.";

                _logger!.LogWarning("CompanyService.GetCompanyByIdAsync: {message}", msg);

                return Result<CompanyDetail>.Failure<CompanyDetail>(
                    new Error("CompanyService.GetCompanyByIdAsync", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.GetCompanyByIdAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<CompanyDetail>.Failure<CompanyDetail>(
                    new Error("CompanyService.GetCompanyByIdAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                _logger!.LogError("CompanyService.GetCompanyByIdAsync: Request timed out or was canceled: {errMsg}", e.Message);

                return Result<CompanyDetail>.Failure<CompanyDetail>(
                    new Error("CompanyService.GetCompanyByIdAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("CompanyService.GetCompanyByIdAsync: {errMsg}", errMsg);

                return Result<CompanyDetail>.Failure<CompanyDetail>(
                    new Error("CompanyService.GetCompanyByIdAsync", errMsg)
                );
            }
        }

        public async Task<Result<List<CompanyDetail>>> GetCompaniesAsync
        (
            int pageNumber,
            int pageSize
        )
        {
            try
            {
                var queryParams = new Dictionary<string, string?>
                {
                    ["pageNumber"] = pageNumber.ToString(),
                    ["pageSize"] = pageSize.ToString()
                };

                HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(relativePath, queryParams));

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<List<CompanyDetail>>();
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.GetCompaniesAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<List<CompanyDetail>>.Failure<List<CompanyDetail>>(
                    new Error("CompanyService.GetCompaniesAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                _logger!.LogError("CompanyService.GetCompaniesAsync: Request timed out or was canceled: {errMsg}", e.Message);

                return Result<List<CompanyDetail>>.Failure<List<CompanyDetail>>(
                    new Error("CompanyService.GetCompaniesAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("CompanyService.GetCompaniesAsync: {errMsg}", errMsg);

                return Result<List<CompanyDetail>>.Failure<List<CompanyDetail>>(
                    new Error("CompanyService.GetCompaniesAsync", errMsg)
                );
            }
        }

        public async Task<Result<CompanyDetail>> CreateCompanyAsync(CompanyDetail company)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(company);
                StringContent content = new(jsonString, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await _httpClient.PostAsync(relativePath, content);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<CompanyDetail>();
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.CreateCompanyAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<CompanyDetail>.Failure<CompanyDetail>(
                    new Error("CompanyService.UpdateCompanyAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                _logger!.LogError("CompanyService.CreateCompanyAsync: Request timed out or was canceled: {errMsg}", e.Message);

                return Result<CompanyDetail>.Failure<CompanyDetail>(
                    new Error("CompanyService.CreateCompanyAsync", e.Message)
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError(ex, "{Message}", errMsg);

                return Result<CompanyDetail>.Failure<CompanyDetail>(
                    new Error("CompanyService.CreateCompanyAsync", errMsg)
                );
            }
        }

        public async Task<Result> UpdateCompanyAsync(CompanyDetail company)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(company);
                StringContent content = new(jsonString, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await _httpClient.PutAsync(relativePath, content);

                response.EnsureSuccessStatusCode();

                return Result.Success();
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.UpdateCompanyAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<CompanyDetail>.Failure<CompanyDetail>(
                    new Error("CompanyService.UpdateCompanyAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                _logger!.LogError("CompanyService.UpdateCompanyAsync: Request timed out or was canceled: {errMsg}", e.Message);

                return Result.Failure(new Error("CompanyService.UpdateCompanyAsync", e.Message));
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError(ex, "{Message}", errMsg);

                return Result.Failure(new Error("CompanyService.UpdateCompanyAsync", errMsg));
            }
        }

        public async Task<Result> DeleteCompanyAsync(int companyCode)
        {
            try
            {
                DeleteCompanyCommand command = new(companyCode);

                var memStream = new MemoryStream();
                await JsonSerializer.SerializeAsync(memStream, command);
                memStream.Seek(0, SeekOrigin.Begin);

                var request = new HttpRequestMessage(HttpMethod.Delete, relativePath);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using var requestContent = new StreamContent(memStream);
                request.Content = requestContent;
                requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();

                return Result.Success();
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.DeleteCompanyAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<CompanyDetail>.Failure<CompanyDetail>(
                    new Error("CompanyService.DeleteCompanyAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                _logger!.LogError("CompanyService.DeleteCompanyAsync: Request timed out or was canceled: {errMsg}", e.Message);

                return Result.Failure(new Error("CompanyService.DeleteCompanyAsync", e.Message));
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError(ex, "{Message}", errMsg);

                return Result.Failure(new Error("CompanyService.DeleteCompanyAsync", errMsg));
            }
        }

        public async Task<Result<CompanyWithFiscalPeriodsDto>> GetCompanyFiscalYearAsync(int companyCode)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{relativePath}/fiscalyear/{companyCode}");

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<CompanyWithFiscalPeriodsDto>();
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string msg = $"A company with company code {companyCode} was not found.";

                _logger!.LogWarning("CompanyService.GetCompanyByIdAsync: {message}", msg);

                return Result<CompanyWithFiscalPeriodsDto>.Failure<CompanyWithFiscalPeriodsDto>(
                    new Error("CompanyService.GetCompanyByIdAsync", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.GetCompanyDtoWithoutFiscalYearAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<CompanyWithFiscalPeriodsDto>.Failure<CompanyWithFiscalPeriodsDto>(
                    new Error("CompanyService.GetCompanyDtoWithoutFiscalYearAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                _logger!.LogError("CompanyService.GetCompanyDtoWithoutFiscalYearAsync: Request timed out or was canceled: {errMsg}", e.Message);

                return Result<CompanyWithFiscalPeriodsDto>.Failure<CompanyWithFiscalPeriodsDto>(
                    new Error("CompanyService.GetCompanyDtoWithoutFiscalYearAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("CompanyService.GetCompanyDtoWithoutFiscalYearAsync: {errMsg}", errMsg);

                return Result<CompanyWithFiscalPeriodsDto>.Failure<CompanyWithFiscalPeriodsDto>(
                    new Error("CompanyService.GetCompanyDtoWithoutFiscalYearAsync", errMsg)
                );
            }
        }

        public async Task<Result<CompanyWithFiscalPeriodsDto>> CreateCompanyFiscalYearAsync(CreateFiscalYearCommand command)
        {
            try
            {
                var jsonCompany = JsonSerializer.Serialize(command);
                var requestContent = new StringContent(jsonCompany, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await _httpClient.PostAsync($"{relativePath}/fiscalyear", requestContent);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return await response.Content.ReadFromJsonAsync<CompanyWithFiscalPeriodsDto>();
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string msg = $"Failed to create new fiscal year {command.FiscalYear} with starting month {command.StartMonthNumber}.";

                _logger!.LogError("CompanyService.GetCompanyByIdAsync: {message}", msg);

                return Result<CompanyWithFiscalPeriodsDto>.Failure<CompanyWithFiscalPeriodsDto>(
                    new Error("CompanyService.CreateCompanyFiscalYearAsync", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.GetCompanyDtoWithoutFiscalYearAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<CompanyWithFiscalPeriodsDto>.Failure<CompanyWithFiscalPeriodsDto>(
                    new Error("CompanyService.CreateCompanyFiscalYearAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                _logger!.LogError("CompanyService.GetCompanyDtoWithoutFiscalYearAsync: Request timed out or was canceled: {errMsg}", e.Message);

                return Result<CompanyWithFiscalPeriodsDto>.Failure<CompanyWithFiscalPeriodsDto>(
                    new Error("CompanyService.CreateCompanyFiscalYearAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("CompanyService.CreateCompanyFiscalYearAsync: {errMsg}", errMsg);

                return Result<CompanyWithFiscalPeriodsDto>.Failure<CompanyWithFiscalPeriodsDto>(
                    new Error("CompanyService.CreateCompanyFiscalYearAsync", errMsg)
                );
            }

        }

        public async Task<Result<DateTime>> GetNextValidFiscalYearStartDateAsync(int companyCode)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{relativePath}/validstartdate/{companyCode}");

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<DateTime>();
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.GetCompanyDtoWithoutFiscalYearAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<DateTime>.Failure<DateTime>(
                    new Error("CompanyService.GetNextValidFiscalYearStartDateAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                _logger!.LogError("CompanyService.GetCompanyDtoWithoutFiscalYearAsync: Request timed out or was canceled: {errMsg}", e.Message);

                return Result<DateTime>.Failure<DateTime>(
                    new Error("CompanyService.GetNextValidFiscalYearStartDateAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("CompanyService.GetNextValidFiscalYearStartDateAsync: {errMsg}", errMsg);

                return Result<DateTime>.Failure<DateTime>(
                    new Error("CompanyService.GetCompanyDtoWithoutFiscalYearAsync", errMsg)
                );
            }
        }

        public async Task<Result> DeleteCompanyFiscalYearAsync(int companyCode, int fiscalYear)
        {
            try
            {
                string uri = $"{relativePath}/fiscalyear";
                DeleteFiscalYearCommand command = new(companyCode, fiscalYear);

                var memStream = new MemoryStream();
                await JsonSerializer.SerializeAsync(memStream, command);
                memStream.Seek(0, SeekOrigin.Begin);

                var request = new HttpRequestMessage(HttpMethod.Delete, uri);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using var requestContent = new StreamContent(memStream);
                request.Content = requestContent;
                requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();

                return Result.Success();
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.DeleteCompanyFiscalYearAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }
                return Result.Failure(new Error("CompanyService.DeleteCompanyFiscalYearAsync", Helpers.GetExceptionMessage(e)));
            }
            catch (TaskCanceledException e)
            {
                _logger!.LogError("CompanyService.DeleteCompanyFiscalYearAsync: Request timed out or was canceled: {errMsg}", e.Message);
                return Result.Failure(new Error("CompanyService.DeleteCompanyFiscalYearAsync", e.Message));
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError(ex, "{Message}", errMsg);
                return Result.Failure(new Error("CompanyService.DeleteCompanyFiscalYearAsync", errMsg));
            }
        }
    }
}
