using Microsoft.Owin.Security;
using System.Net.Http;
using TaskManager.Common.UserContextModels;

namespace TaskManager.Data.Common
{
    public static class HttpRequestMessageExtensions
    {
        public static UserContext GetUserContext(this HttpRequestMessage request)
        {
            return new UserContext(1); //To do
            if (!request.GetOwinEnvironment().ContainsKey("oauth.Properties"))
            {
                return new UnathorizedUserContext();
            }

            AuthenticationProperties properties = request.GetOwinEnvironment()["oauth.Properties"] as AuthenticationProperties;
            var userId = int.Parse(properties.Dictionary["userId"]);

            return new UserContext(userId);
        }
    }
}
