using CloudAccounting.Wasm.Services.Repositories.Company;
using Microsoft.Extensions.Logging;

namespace CloudAccounting.Wasm.Services.Repositories.Common
{
    public class LookupService
        (
            IHttpClientFactory ClientFactory,
            ILogger<LookupService>? logger
        ) : ILookupService
    {
        private readonly HttpClient _httpClient = ClientFactory.CreateClient("CloudAccountingAPI");
        private readonly ILogger<LookupService>? _logger = logger;
        private const string relativePath = "/api/v1/lookups";

        public async Task<Result<List<CompanyLookup>>> GetCompanyLookups()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{relativePath}/companycodes");

                response.EnsureSuccessStatusCode();

                List<CompanyLookup>? lookups = await response.Content.ReadFromJsonAsync<List<CompanyLookup>>();

                return lookups;
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string msg = $"Oops, unable to retrieve company code lookups.";

                _logger!.LogWarning("CompanyService.GetCompanyByIdAsync: {message}", msg);

                return Result<List<CompanyLookup>>.Failure<List<CompanyLookup>>(
                    new Error("LookupService.GetCompanyLookups", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("LookupService.GetCompanyLookups: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<List<CompanyLookup>>.Failure<List<CompanyLookup>>(
                    new Error("LookupService.GetCompanyLookups", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                _logger!.LogError("LookupService.GetCompanyLookups: Request timed out or was canceled: {errMsg}", e.Message);

                return Result<List<CompanyLookup>>.Failure<List<CompanyLookup>>(
                    new Error("LookupService.GetCompanyLookups", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("LookupService.GetCompanyLookups: {errMsg}", errMsg);

                return Result<List<CompanyLookup>>.Failure<List<CompanyLookup>>(
                    new Error("LookupService.GetCompanyLookups", errMsg)
                );
            }
        }
    }
}
