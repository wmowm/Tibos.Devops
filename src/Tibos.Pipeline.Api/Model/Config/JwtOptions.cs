namespace Tibos.Pipeline.Api.Model.Config
{
    public class JwtOptions
    {
        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
    }
}
