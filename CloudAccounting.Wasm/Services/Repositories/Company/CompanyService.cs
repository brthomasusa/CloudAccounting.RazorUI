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
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError(ex, "{Message}", errMsg);

                return Result.Failure(new Error("CompanyService.DeleteCompanyAsync", errMsg));
            }
        }

        public async Task<Result<FiscalYearDto>> GetCompanyFiscalYearAsync(int companyCode)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"/api/v1/fiscalyears/{companyCode}");

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<FiscalYearDto>();
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string msg = $"A company with company code {companyCode} was not found.";

                _logger!.LogWarning("CompanyService.GetCompanyFiscalYearAsync: {message}", msg);

                return Result<FiscalYearDto>.Failure<FiscalYearDto>(
                    new Error("CompanyService.GetCompanyFiscalYearAsync", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.GetCompanyFiscalYearAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<FiscalYearDto>.Failure<FiscalYearDto>(
                    new Error("CompanyService.GetCompanyFiscalYearAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("CompanyService.GetCompanyFiscalYearAsync: {errMsg}", errMsg);

                return Result<FiscalYearDto>.Failure<FiscalYearDto>(
                    new Error("CompanyService.GetCompanyFiscalYearAsync", errMsg)
                );
            }
        }

        public async Task<Result<FiscalYearDto>> GetCompanyFiscalYearAsync(int companyCode, int fiscalYear)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"/api/v1/fiscalyears/{companyCode}/{fiscalYear}");

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<FiscalYearDto>();
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string msg = $"A company with company code {companyCode} was not found.";

                _logger!.LogWarning("CompanyService.GetCompanyFiscalYearAsync: {message}", msg);

                return Result<FiscalYearDto>.Failure<FiscalYearDto>(
                    new Error("CompanyService.GetCompanyFiscalYearAsync", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.GetCompanyFiscalYearAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<FiscalYearDto>.Failure<FiscalYearDto>(
                    new Error("CompanyService.GetCompanyFiscalYearAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("CompanyService.GetCompanyFiscalYearAsync: {errMsg}", errMsg);

                return Result<FiscalYearDto>.Failure<FiscalYearDto>(
                    new Error("CompanyService.GetCompanyFiscalYearAsync", errMsg)
                );
            }
        }

        public async Task<Result<FiscalYearDto>> CreateCompanyFiscalYearAsync(CreateFiscalYearCommand command)
        {
            try
            {
                var jsonFiscalYear = JsonSerializer.Serialize(command);
                var requestContent = new StringContent(jsonFiscalYear, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await _httpClient.PostAsync($"/api/v1/fiscalyears", requestContent);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return await response.Content.ReadFromJsonAsync<FiscalYearDto>();
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string msg = $"Failed to create new fiscal year {command.FiscalYear} with starting date {command.StartDate}.";

                _logger!.LogError("CompanyService.GetCompanyByIdAsync: {message}", msg);

                return Result<FiscalYearDto>.Failure<FiscalYearDto>(
                    new Error("CompanyService.CreateCompanyFiscalYearAsync", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("CompanyService.GetCompanyDtoWithoutFiscalYearAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<FiscalYearDto>.Failure<FiscalYearDto>(
                    new Error("CompanyService.CreateCompanyFiscalYearAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("CompanyService.CreateCompanyFiscalYearAsync: {errMsg}", errMsg);

                return Result<FiscalYearDto>.Failure<FiscalYearDto>(
                    new Error("CompanyService.CreateCompanyFiscalYearAsync", errMsg)
                );
            }
        }

        public async Task<Result> DeleteCompanyFiscalYearAsync(int companyCode, int fiscalYear)
        {
            try
            {
                string uri = $"/api/v1/fiscalyears";
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
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError(ex, "{Message}", errMsg);
                return Result.Failure(new Error("CompanyService.DeleteCompanyFiscalYearAsync", errMsg));
            }
        }
    }
}
