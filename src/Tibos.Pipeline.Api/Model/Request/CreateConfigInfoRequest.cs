namespace Tibos.Pipeline.Api.Model.Request
{
    public class CreateConfigInfoRequest
    {
        /// <summary>
        /// 环境编号
        /// </summary>
        public long EnvId { get; set; }

        /// <summary>
        /// 应用编号
        /// </summary>
        public long AppId { get; set; }


        /// <summary>
        /// 挂载路径
        /// </summary>
        public string MountPath { get; set; }

        /// <summary>
        /// 子路径
        /// </summary>
        public string SubPath { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Remark { get; set; }
    }
}
