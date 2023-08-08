namespace Tibos.Pipeline.Api.Model.Request
{
    public class CreateEnvInfoRequest
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 域名
        /// </summary>
        public string DomainSymbol { get; set; }

        /// <summary>
        /// 关联标签
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 标签类型(0:branch,1:tag)
        /// </summary>
        public int TagType { get; set; }
    }
}
