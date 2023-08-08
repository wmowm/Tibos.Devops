namespace Tibos.Pipeline.Api.Model.Response
{
    public class GitlabCreateAppResponse
    {
        /// <summary>
        /// 项目id
        /// </summary>
        public long ProjectId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目地址
        /// </summary>
        public string WebUrl { get; set; }
    }
}
