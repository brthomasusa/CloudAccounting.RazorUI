namespace CloudAccounting.Wasm.Models.Authentication;

public class UpdateUserRoleCommand
{
    public required string Email { get; init; }
    public string RoleName { get; set; } = string.Empty;
}