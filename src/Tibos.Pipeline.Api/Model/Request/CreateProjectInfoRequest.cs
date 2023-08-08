namespace Tibos.Pipeline.Api.Model.Request
{
    public class CreateProjectInfoRequest
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
        /// 团队编号
        /// </summary>
        public long TeamId { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
    }
}
