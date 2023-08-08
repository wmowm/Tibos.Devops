namespace Tibos.Pipeline.Api.Model.Request
{
    public class UpdateTeamInfoRequest:CreateTeamInfoRequest
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long Id { get; set; }
    }
}
