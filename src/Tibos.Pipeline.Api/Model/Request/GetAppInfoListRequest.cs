namespace Tibos.Pipeline.Api.Model.Request
{
    public class GetAppInfoListRequest:BaseRequest
    {
        public long? Id { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public long? ProjectId { get; set; }
    }
}
