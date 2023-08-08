namespace Tibos.Pipeline.Api.Model.Request
{
    public class JoinTeamRequest
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public List<string> UserIds { get; set; }

        /// <summary>
        /// 团队编号
        /// </summary>
        public long TeamId { get; set; }
    }
}
