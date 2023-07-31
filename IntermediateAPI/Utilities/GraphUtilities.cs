using B2CIntermediateAPI.Models.Graph;
using Microsoft.IdentityModel.Tokens;

namespace B2CIntermediateAPI.Utilities
{
    public static class GraphUtilities
    {
        public static IEnumerable<string> GetIds(this SoftwareOAuthMethods? methods)
        {
            if (methods?.Value?.IsNullOrEmpty() ?? true)
            {
                return Enumerable.Empty<string>();
            }
            return from method in methods?.Value
                select method.Id;
        }
    }
}