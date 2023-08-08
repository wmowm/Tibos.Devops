namespace Tibos.Pipeline.Api.Model.Request
{
    public class UpdateAppInfoRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
    }
}
