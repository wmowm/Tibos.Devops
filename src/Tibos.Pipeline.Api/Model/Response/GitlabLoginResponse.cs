namespace Tibos.Pipeline.Api.Model.Response
{
    public class GitlabLoginResponse
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public virtual string NickName { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        public virtual string Group { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public virtual string Phone { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// openId
        /// </summary>
        public string OpenId { get; set; }
    }
}
