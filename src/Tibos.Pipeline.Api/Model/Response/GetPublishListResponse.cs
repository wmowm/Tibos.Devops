namespace Tibos.Pipeline.Api.Model.Response
{
    public class GetPublishListResponse
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 发布描述
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 发布状态(-1:发布失败,0:待审核,1:审核通过,2:发布中,3:发布完成)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 应用编号
        /// </summary>
        public long AppId { get; set; }

        /// <summary>
        /// 环境编号
        /// </summary>
        public long EnvId { get; set; }

        /// <summary>
        /// 构建记录编号
        /// </summary>
        public long BuildRecordId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 环境名称
        /// </summary>
        public string EnvName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string HomePage { get; set; }

        /// <summary>
        /// 构建编号
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string SHA { get; set; }

        /// <summary>
        /// 分支
        /// </summary>
        public string Branch { get; set; }
    }
}
