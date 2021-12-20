using RestSharp;
using System.Web;
using RestSharp.Serializers.SystemTextJson;
using System.Collections.Specialized;
using System.Text.Json;
namespace Weebo_lib
{
    public static class MAL_lib
    {
        //create new "Secrets" class with public Client_ID and Client_Secret (from MAL)
        static string Client_ID = Secrets.Client_ID;
        static string Client_Secret = Secrets.Client_Secret;
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
                //not sure we need client_secret
                .AddParameter("client_secret", Client_Secret)
                .AddParameter("code", code)
                .AddParameter("code_verifier", _PKCE.code_verifier)
                .AddParameter("grant_type", "authorization_code")
                .AddParameter("redirect_uri", redirectURL);
            IRestResponse responseToken = client.Get(request);
            JsonSerializer.Deserialize<TokenResponse>(responseToken.Content);
        }

        public static void GetUserAnimeList()
        {
            RestClient client = new RestClient()
            {
                BaseUrl = new Uri("https://api.myanimelist.net/v2")
            };
            RestRequest request = (RestRequest)new RestRequest($"users/@me/animelist")
                .AddHeader("Authorization", TokenResponse.access_token)
                .AddQueryParameter("status", Enum.GetName(status.watching))
                .AddQueryParameter("sort", Enum.GetName(sorts.anime_title));
            IRestResponse response = client.Get(request);
            Anime[] UserAnimeList = JsonSerializer.Deserialize<Anime[]>(response.Content);
        }

        [System.Serializable]
        public record TokenResponse
        {
            public static string token_type;
            public static int expires_in;
            public static string access_token;
            public static string refresh_token;
        }
        enum sorts
        {
            list_score,
            list_updated_at,
            anime_title,
            anime_start_date
            //anime Id under development
        }
        enum status
        {
            watching,
            completed,
            on_hold,
            dropped,
            plan_to_watch
        }
    }
}
