using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Model.Request
{
    public class CreateTeamInfoRequest
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }

        public string Logo { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        public List<string> Groups { get; set; }


        /// <summary>
        /// 域名
        /// </summary>
        public List<string> Domains { get; set; }
    }
}
