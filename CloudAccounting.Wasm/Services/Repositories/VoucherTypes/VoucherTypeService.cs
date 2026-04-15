using CloudAccounting.Wasm.Models.VoucherTypes;

namespace CloudAccounting.Wasm.Services.Repositories.VoucherTypes
{
    public class VoucherTypeService
    (
        IHttpClientFactory ClientFactory,
        ILogger<VoucherTypeService> logger
    ) : IVoucherTypeService
    {
        private readonly HttpClient _httpClient = ClientFactory.CreateClient("CloudAccountingAPI");
        private readonly ILogger<VoucherTypeService>? _logger = logger;
        private const string relativePath = "/api/v1/vouchertypes";

        public async Task<Result<List<VoucherTypeDto>>> RetrieveAllAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(relativePath);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<List<VoucherTypeDto>>();
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("VoucherTypeService.RetrieveAllAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<List<VoucherTypeDto>>.Failure<List<VoucherTypeDto>>(
                    new Error("VoucherTypeService.RetrieveAllAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("VoucherTypeService.RetrieveAllAsync: {errMsg}", errMsg);

                return Result<List<VoucherTypeDto>>.Failure<List<VoucherTypeDto>>(
                    new Error("VoucherTypeService.RetrieveAllAsync", errMsg)
                );
            }
        }

        public async Task<Result<VoucherTypeDto>> RetrieveAsync(int voucherCode)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{relativePath}/{voucherCode}");

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<VoucherTypeDto>();
            }
            catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                string msg = $"A voucher type with voucher code {voucherCode} was not found.";

                _logger!.LogWarning("VoucherTypeService.RetrieveAsync: {message}", msg);

                return Result<VoucherTypeDto>.Failure<VoucherTypeDto>(
                    new Error("VoucherTypeService.RetrieveAsync", msg)
                );
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("VoucherTypeService.RetrieveAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<VoucherTypeDto>.Failure<VoucherTypeDto>(
                    new Error("VoucherTypeService.RetrieveAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError("VoucherTypeService.RetrieveAsync: {errMsg}", errMsg);

                return Result<VoucherTypeDto>.Failure<VoucherTypeDto>(
                    new Error("VoucherTypeService.RetrieveAsync", errMsg)
                );
            }
        }

        public async Task<Result<VoucherTypeCommand>> CreateAsync(VoucherTypeCommand command)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(command);
                StringContent content = new(jsonString, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await _httpClient.PostAsync(relativePath, content);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<VoucherTypeCommand>();
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("VoucherTypeService.CreateAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<VoucherTypeCommand>.Failure<VoucherTypeCommand>(
                    new Error("VoucherTypeService.CreateAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError(ex, "{Message}", errMsg);

                return Result<VoucherTypeCommand>.Failure<VoucherTypeCommand>(
                    new Error("VoucherTypeService.CreateAsync", errMsg)
                );
            }
        }

        public async Task<Result<VoucherTypeCommand>> UpdateAsync(VoucherTypeCommand command)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(command);
                StringContent content = new(jsonString, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await _httpClient.PutAsync(relativePath, content);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<VoucherTypeCommand>();
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode.HasValue)
                {
                    _logger!.LogError("VoucherTypeService.UpdateAsync: Status Code: {statusCode}", e.StatusCode.Value);
                }

                return Result<VoucherTypeCommand>.Failure<VoucherTypeCommand>(
                    new Error("VoucherTypeService.UpdateAsync", Helpers.GetExceptionMessage(e))
                );
            }
            catch (Exception ex)
            {
                string errMsg = Helpers.GetExceptionMessage(ex);
                _logger!.LogError(ex, "{Message}", errMsg);

                return Result<VoucherTypeCommand>.Failure<VoucherTypeCommand>(
                    new Error("VoucherTypeService.UpdateAsync", errMsg)
                );
            }
        }

        public async Task<Result> DeleteAsync(int voucherCode)
        {
            try
            {
                DeleteVoucherTypeCommand command = new(voucherCode);

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
    }
}
