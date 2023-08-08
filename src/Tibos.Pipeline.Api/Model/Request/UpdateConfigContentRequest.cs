namespace Tibos.Pipeline.Api.Model.Request
{
    public class UpdateConfigContentRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 重启容器
        /// </summary>
        public bool RestartContainer { get; set; }
    }
}
