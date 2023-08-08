namespace Tibos.Pipeline.Api.Model.Request
{
    public class CreateAppInfoRequest
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        public long ProjectId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 模板编号
        /// </summary>
        public long TemplateId { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        public string Group { get; set; }
    }
}
