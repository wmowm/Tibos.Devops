namespace Tibos.Pipeline.Api.Model.Request
{
    public class UpdateMappingConfigRequest
    {
        /// <summary>
        /// 标识
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 映射配置
        /// </summary>
        public bool MappingConfig { get; set; }
    }
}
