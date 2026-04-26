using CloudAccounting.Wasm.Models.Authentication;

namespace CloudAccounting.Wasm.Services.ApplicationState
{
    public interface IAuthenticatedUserState
    {

        event Action OnChange;
        void SetUserId(string userId);
        void SetUser(ApplicationUser user);
        ApplicationUser GetUser();
        void SetCompany(int companyCode, string companyName);
         void SetCompanyYear(Int16 companyYear);
         void SetCompanyMonth(byte companyMonthId, string companyMonthName);

        void SetGroup(Int16 groupId, string groupTitle);
    }
}
