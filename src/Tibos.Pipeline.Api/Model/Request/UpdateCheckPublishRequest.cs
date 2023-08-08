namespace Tibos.Pipeline.Api.Model.Request
{
    public class UpdateCheckPublishRequest
    {
        /// <summary>
        /// 标识
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 审核发布
        /// </summary>
        public bool CheckPublish { get; set; }
    }
}
