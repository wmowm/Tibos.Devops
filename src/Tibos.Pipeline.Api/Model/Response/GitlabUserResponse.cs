using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Tibos.Pipeline.Api.Model.Response
{
    public class GitlabUserResponse
    {
        [JsonProperty(PropertyName ="name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName ="avatar_url")]
        public string Avatar { get; set; }

        [JsonProperty(PropertyName ="email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName ="blog")]
        public string Blog { get; set; }

        [JsonProperty(PropertyName ="bio")]
        public string Bio { get; set; }

        [JsonProperty(PropertyName ="created_at")]
        public DateTime CreateAt { get; set; }

        [JsonProperty(PropertyName ="updated_at")]
        public DateTime UpdateAt { get; set; }

        [JsonProperty(PropertyName ="public_repos")]
        public int PublicRepos { get; set; }

        [JsonProperty(PropertyName ="public_gists")]
        public int PublicGists { get; set; }

        [JsonProperty(PropertyName ="followers")]
        public int Followers { get; set; }

        [JsonProperty(PropertyName ="following")]
        public int Following { get; set; }

        [JsonProperty(PropertyName ="message")]
        public string ErrorMessage { get; set; }
    }
}
