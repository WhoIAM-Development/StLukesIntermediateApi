using System.Globalization;
using System.Resources;

namespace IntermediateAPI.Utilities
{
    public class MessagingUtility
    {
        private readonly bool _isDevEnvironment;

        public enum Messages
        {
            TotpGenerateError,
            TotpVerifyError,
            GeneralSignInError
        }

        private ResourceManager rm = new ResourceManager("IntermediateAPI.Resources.Strings", typeof(MessagingUtility).Assembly);

        public MessagingUtility(bool? isDevEnvironment)
        {
            _isDevEnvironment = isDevEnvironment ?? true;
        }

        public string GetLocalizedString(string locale, Messages message, string developerMessage = null)
        {
            string s = rm.GetString(message.ToString(), culture: new CultureInfo(locale));

            if (_isDevEnvironment && !string.IsNullOrEmpty(developerMessage))
            {
                s = $"{s} - {developerMessage}";
            }

            return s;
        }
    }
}
