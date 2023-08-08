namespace Tibos.Pipeline.Api.Model.Config
{
    public class DockerOptions
    {
        /// <summary>
        /// docker镜像仓库地址
        /// </summary>
        public string DokcerRegistry { get; set; }

        /// <summary>
        /// 名称空间
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// docker镜像仓库账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// docker镜像仓库密码
        /// </summary>
        public string Password { get; set; }
    }
}
