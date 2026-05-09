
namespace CloudAccounting.Wasm.Models.Authentication;

public class CreateUserWithRoleCommand
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int CompanyCode { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsCompanyAdmin { get; set; }
}