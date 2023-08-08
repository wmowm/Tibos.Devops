namespace Tibos.Pipeline.Api.Model.Request
{
    public class GetProjectInfoListRequest:BaseRequest
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

    }
}
