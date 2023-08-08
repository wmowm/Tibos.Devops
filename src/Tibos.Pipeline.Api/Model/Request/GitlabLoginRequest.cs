using System.Text.Json.Serialization;

namespace Tibos.Pipeline.Api.Model.Request
{
    public class GitlabLoginRequest
    {

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }


        /// <summary>
        /// openId
        /// </summary>
        public string OpenId { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }


        /// <summary>
        /// 组
        /// </summary>
        [JsonPropertyName("groups")]
        public List<string> Groups { get; set; }
    }
}
