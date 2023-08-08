namespace Tibos.Pipeline.Api.Model.Request
{
    public class UpdateUserStatusRequest
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 状态(0:禁用,1:启用)
        /// </summary>
        public int Status { get; set; }
    }
}
