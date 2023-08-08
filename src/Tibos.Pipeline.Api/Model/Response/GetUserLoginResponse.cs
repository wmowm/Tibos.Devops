namespace Tibos.Pipeline.Api.Model.Response
{
    public class GetUserLoginResponse
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }

       
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
    

        /// <summary>
        /// 角色
        /// </summary>
        public string Roles { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
    }
}
