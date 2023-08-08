namespace Tibos.Pipeline.Api.Model.Request
{
    public class GetPodListRequest
    {
        /// <summary>
        /// 环境编号
        /// </summary>
        public long EnvId { get; set; }

        /// <summary>
        /// 应用编号
        /// </summary>
        public long AppId { get; set; }
    }
}
