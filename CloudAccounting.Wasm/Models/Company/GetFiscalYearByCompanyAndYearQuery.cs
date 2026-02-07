namespace CloudAccounting.Wasm.Models.Company
{
    public record class GetFiscalYearByCompanyAndYearQuery(
        int CompanyCode,
        int FiscalYear
    );

}
