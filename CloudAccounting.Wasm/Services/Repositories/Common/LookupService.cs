
namespace CloudAccounting.Wasm.Services.Repositories.Common
{
    public class LookupService
        (
            IHttpClientFactory clientFactory,
            ILogger<LookupService>? logger
        ) : ILookupService
    {
        private readonly HttpClient _httpClient = clientFactory.CreateClient("CloudAccountingAPI");
        private const string RelativePath = "/api/v1/lookups";

        public async Task<Result<List<CompanyLookup>>> GetCompanyLookups()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{RelativePath}/companycodes");

                response.EnsureSuccessStatusCode();

                
                List<CompanyLookup>? lookups = await response.Content.ReadFromJsonAsync<List<CompanyLookup>>();

                return lookups;
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string msg = "Oops, unable to retrieve company code lookups.";

                // _logger!.LogWarning("CompanyService.GetCompanyByIdAsync: {message}", msg);

                return Result.Failure<List<CompanyLookup>>(
                    new Error("LookupService.GetCompanyLookups", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    // _logger!.LogError("LookupService.GetCompanyLookups: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result.Failure<List<CompanyLookup>>(
                    new Error("LookupService.GetCompanyLookups", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                logger!.LogError("LookupService.GetCompanyLookups: Request timed out or was canceled: {errMsg}", e.Message);

                return Result.Failure<List<CompanyLookup>>(
                    new Error("LookupService.GetCompanyLookups", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                logger!.LogError("LookupService.GetCompanyLookups: {errMsg}", errMsg);

                return Result.Failure<List<CompanyLookup>>(
                    new Error("LookupService.GetCompanyLookups", errMsg)
                );
            }
        }

        public async Task<Result<List<CostCenterLookupItem>>> GetCostCenterLookups(int companyCode)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{RelativePath}/costcenters/{companyCode}");

                response.EnsureSuccessStatusCode();

                List<CostCenterLookupItem>? lookups = await response.Content.ReadFromJsonAsync<List<CostCenterLookupItem>>();

                return Result.Success(lookups);
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string msg = "Oops, unable to retrieve cost center lookups.";
                logger!.LogWarning("LookupService.GetCostCenterLookups: {message}", msg);

                return Result.Failure<List<CostCenterLookupItem>>(
                    new Error("LookupService.GetCostCenterLookups", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    logger!.LogError("LookupService.GetCostCenterLookups: Status Code: {statusCode}", e.StatusCode.Value);
                }
                return Result.Failure<List<CostCenterLookupItem>>(
                    new Error("LookupService.GetCostCenterLookups", Helpers.GetExceptionMessage(e))
                );
            }
            catch (TaskCanceledException e)
            {
                logger!.LogError("LookupService.GetCostCenterLookups: Request timed out or was canceled: {errMsg}", e.Message);

                return Result.Failure<List<CostCenterLookupItem>>(
                    new Error("LookupService.GetCostCenterLookups", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                logger!.LogError("LookupService.GetCostCenterLookups: {errMsg}", errMsg);

                return Result.Failure<List<CostCenterLookupItem>>(
                    new Error("LookupService.GetCostCenterLookups", errMsg)
                );
            }
        }

        public async Task<Result<List<FiscalYearLookupItem>>> RetrieveFiscalYearsAsync(int companyCode)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{RelativePath}/fiscalyears/{companyCode}");

                response.EnsureSuccessStatusCode();

                List<FiscalYearLookupItem>? lookups = await response.Content.ReadFromJsonAsync<List<FiscalYearLookupItem>>();

                return Result.Success(lookups);
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string msg = "Oops, unable to retrieve fiscal year information.";
                logger!.LogWarning("LookupService.GetCostCenterLookups: {message}", msg);

                return Result.Failure<List<FiscalYearLookupItem>>(
                    new Error("LookupService.RetrieveFiscalYearsAsync", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    logger!.LogError("LookupService.RetrieveFiscalYearsAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }
                return Result.Failure<List<FiscalYearLookupItem>>(
                    new Error("LookupService.RetrieveFiscalYearsAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                logger!.LogError("LookupService.RetrieveFiscalYearsAsync: {errMsg}", errMsg);

                return Result.Failure<List<FiscalYearLookupItem>>(
                    new Error("LookupService.RetrieveFiscalYearsAsync", errMsg)
                );
            }
        }

        public async Task<Result<List<FiscalPeriodLookupItem>>> RetrieveFiscalPeriodsAsync(int companyCode, int companyYear)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{RelativePath}/fiscalperiods/{companyCode}/{companyYear}");

                response.EnsureSuccessStatusCode();

                List<FiscalPeriodLookupItem>? lookups = await response.Content.ReadFromJsonAsync<List<FiscalPeriodLookupItem>>();

                return lookups;
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string msg = "Oops, unable to retrieve fiscal period information.";
                logger!.LogWarning("LookupService.RetrieveFiscalPeriodsAsync: {message}", msg);

                return Result.Failure<List<FiscalPeriodLookupItem>>(
                    new Error("LookupService.RetrieveFiscalPeriodsAsync", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    logger!.LogError("LookupService.RetrieveFiscalPeriodsAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }
                return Result.Failure<List<FiscalPeriodLookupItem>>(
                    new Error("LookupService.RetrieveFiscalPeriodsAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                logger!.LogError("LookupService.RetrieveFiscalPeriodsAsync: {errMsg}", errMsg);

                return Result.Failure<List<FiscalPeriodLookupItem>>(
                    new Error("LookupService.RetrieveFiscalPeriodsAsync", errMsg)
                );
            }
        }
    }
}
