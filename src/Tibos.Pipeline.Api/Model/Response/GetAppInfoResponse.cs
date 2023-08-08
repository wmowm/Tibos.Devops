namespace Tibos.Pipeline.Api.Model.Response
{
    public class GetAppInfoResponse
    {
        public long Id { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public long ProjectId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 模板编号
        /// </summary>
        public long TemplateId { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 应用地址
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// 域名列表
        /// </summary>
        public Dictionary <string,List<string>> DomainMap { get; set; }
    }
}
