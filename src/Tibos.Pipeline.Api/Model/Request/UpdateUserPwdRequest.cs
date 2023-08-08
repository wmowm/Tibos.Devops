namespace Tibos.Pipeline.Api.Model.Request
{
    public class UpdateUserPwdRequest
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPwd { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string Pwd { get; set; }
    }

    public class UpdateUserPwdDto: UpdateUserPwdRequest
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
    }

}
