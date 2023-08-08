namespace Tibos.Pipeline.Api.Model.Request
{
    public class UpdateUserInfoRequest
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 角色组
        /// </summary>
        public List<string> Roles { get; set; }
    }
}
