namespace Tibos.Pipeline.Api.Model.Request
{
    public class UpdateConfigStatusRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 状态(0:未挂载,1:已挂载)
        /// </summary>
        public bool Status { get; set; }
    }
}
