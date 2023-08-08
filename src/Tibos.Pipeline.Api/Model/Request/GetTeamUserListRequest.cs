namespace Tibos.Pipeline.Api.Model.Request
{
    public class GetTeamUserListRequest:BaseRequest
    {

        public long? TeamId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
    }
}
