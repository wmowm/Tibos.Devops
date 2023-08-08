namespace Tibos.Pipeline.Api.Model.Request
{
    public class UpdateProjectInfoRequest:CreateProjectInfoRequest
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long Id { get; set; }
    }
}
