namespace Tibos.Pipeline.Api.Model.Response
{
    public class GetTeamUserListResponse
    {
        /// <summary>
        /// 主键,唯一
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 团队id
        /// </summary>
        public long TeamId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 团队名称
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 团队logo
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 域名资源
        /// </summary>
        public string Domains { get; set; }
    }
}
