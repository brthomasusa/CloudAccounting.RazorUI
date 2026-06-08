using CloudAccounting.Wasm.Models.Authentication;

namespace CloudAccounting.Wasm.Services.ApplicationState
{
    public class AuthenticatedUserState : IAuthenticatedUserState
    {
        private ApplicationUser _applicationUser = new ();

        public event Action? OnChange ;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public ApplicationUser GetUser() => _applicationUser;

        public void SetUser(ApplicationUser user)
        {
            _applicationUser = user;
            NotifyStateChanged();
        }

        public void SetUserId(string userId)
        {
            _applicationUser.UserId = userId;
            NotifyStateChanged();
        }

        public void SetCompany(int companyCode, string companyName)
        {
            _applicationUser.CompanyCode = companyCode;
            _applicationUser.CompanyName = companyName;
            NotifyStateChanged();
        }

        public void SetCompanyYear(Int16 companyYear)
        {
            _applicationUser.CompanyYear = companyYear;
            NotifyStateChanged();
        }

        public string GetCompanyName() => _applicationUser.CompanyName;
        public string GetCompanyYear() => _applicationUser.CompanyYear.ToString();
        public string GetCompanyMonthName() => _applicationUser.CompanyMonthName;

        public void SetCompanyMonth(byte companyMonthId, string companyMonthName)
        {
            _applicationUser.CompanyMonthId = companyMonthId;
            _applicationUser.CompanyMonthName = companyMonthName;
            NotifyStateChanged();
        }

        public void SetGroup(Int16 groupId, string groupTitle)
        {
            _applicationUser.GroupId = groupId;
            _applicationUser.GroupTitle = groupTitle;
            NotifyStateChanged();
        }        
    }
}