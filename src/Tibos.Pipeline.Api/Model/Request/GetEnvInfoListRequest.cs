namespace Tibos.Pipeline.Api.Model.Request
{
    public class GetEnvInfoListRequest:BaseRequest
    {
        /// <summary>
        /// 环境名称
        /// </summary>
        public string Name { get; set; }
    }
}
