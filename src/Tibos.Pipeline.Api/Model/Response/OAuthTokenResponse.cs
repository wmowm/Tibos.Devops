namespace Tibos.Pipeline.Api.Model.Response
{
    public class OAuthTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public int created_at { get; set; }
        public string id_token { get; set; }
    }
}

