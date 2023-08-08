namespace Tibos.Pipeline.Api.Model.Request
{
    public class CreateTemplateInfoRequest
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 替换变量
        /// </summary>
        public string TempVal { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
    }

    public class CreateTemplateInfoDto: CreateTemplateInfoRequest 
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
    }
}
