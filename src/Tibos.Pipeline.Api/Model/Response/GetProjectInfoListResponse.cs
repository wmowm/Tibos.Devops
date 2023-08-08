namespace Tibos.Pipeline.Api.Model.Response
{
    public class GetProjectInfoListResponse
    {
        /// <summary>
        /// 主键,自增
        /// </summary>
        public long Id { get; set; }


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
        /// 团队编号
        /// </summary>
        public long? TeamId { get; set; }

        /// <summary>
        /// 团队名称
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// 团队logo
        /// </summary>
        public string TeamLogo { get; set; }

        /// <summary>
        /// 应用数量
        /// </summary>
        public int AppCount { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
    }
}
