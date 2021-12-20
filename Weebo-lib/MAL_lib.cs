using RestSharp;
using System.Web;
using RestSharp.Serializers.SystemTextJson;
using System.Collections.Specialized;
using System.Text.Json;
namespace Weebo_lib
{
    public static class MAL_lib
    {
        static string Client_ID = "SECRET";
        static string Client_Secret = "SECRET";
        static (string code_verifier, string code_challenge) _PKCE = PKCE.GeneratePKCE();
        static string code;
        static string state;
        public static void GetAuth()
        {
            const string redirectURL = "";
            string URL = $"https://myanimelist.net/v1/oauth2/authorize";
            RestClient client = new RestClient(URL);
            client.UseSystemTextJson();
            RestRequest request = (RestRequest) new RestRequest(Method.GET)
                .AddParameter("response_type", "code")
                .AddParameter("redirect_uri", redirectURL)
                .AddParameter("code_challenge", _PKCE.code_challenge)
                .AddParameter("code_challenge_method", "S256");
            IRestResponse response = client.Get(request);
            NameValueCollection values = HttpUtility.ParseQueryString(response.ResponseUri.ToString());
            code = values.Get("code");
            state = values.Get("state");

            client.BaseUrl = new Uri("https://myanimelist.net/v1/oauth2/token");
            RestRequest GetToken = (RestRequest)new RestRequest(Method.GET)
                .AddParameter("client_id", Client_ID)
                //not sure we need this
                .AddParameter("client_secret", Client_Secret)
                .AddParameter("code", code)
                .AddParameter("code_verifier", _PKCE.code_verifier)
                .AddParameter("grant_type", "authorization_code")
                .AddParameter("redirect_uri", redirectURL);
            IRestResponse responseToken = client.Get(request);
            //TODO: JsonSerializer.Deserialize<JsonTokenResponse>(responseToken.Content);
        }
        static class JsonTokenResponse
        {
            static string token_type;
            static int expires_in;
            static string access_token;
            static string refresh_token;
        }
    }
    

}
