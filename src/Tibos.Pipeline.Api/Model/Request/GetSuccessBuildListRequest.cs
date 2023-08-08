namespace Tibos.Pipeline.Api.Model.Request
{
    public class GetSuccessBuildListRequest:BaseRequest
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        public long AppId { get; set; }

        /// <summary>
        /// 环境编号
        /// </summary>
        public long EnvId { get; set; }
    }
}
