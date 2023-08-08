using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Tibos.Pipeline.Api.Model.Response
{
    public class GitlabUserInfoResponse
    {
        /// <summary>
        /// 编号
        /// </summary>
        [JsonProperty(PropertyName = "sub")]
        public string Sub { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        [JsonProperty(PropertyName = "sub_legacy")]
        public string SubLegacy { get; set; }



        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [JsonProperty(PropertyName = "nickname")]
        public string NickName { get; set; }


        [JsonProperty(PropertyName = "preferred_username")]
        public string PreferredUsername { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty(PropertyName = "profile")]
        public string Profile { get; set; }

        [JsonProperty(PropertyName = "picture")]
        public string Picture { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        [JsonProperty(PropertyName = "groups")]
        public List<string> Groups { get; set; }
    }
}
