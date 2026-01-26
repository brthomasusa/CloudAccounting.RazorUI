using PhoneNumbers;

namespace CloudAccounting.Wasm.Utilities
{
    public static class PhoneNumberUtility
    {
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return false;

            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var telephone = phoneNumberUtil.Parse(phoneNumber, "US");
            return phoneNumberUtil.IsValidNumber(telephone);
        }

        public static string GetFormattedPhoneNumber(string phoneNumber) 
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var telephone = phoneNumberUtil.Parse(phoneNumber, "US");

            return phoneNumberUtil.Format(telephone, PhoneNumberFormat.NATIONAL);
        }
    }
}
