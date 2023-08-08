namespace Tibos.Pipeline.Api.Model.Config
{
    public class GitlabOptions
    {
        /// <summary>
        /// 应用id(oauth)
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 应用密钥(oauth)
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 应用授权范围(oauth)
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// 回调地址,重定向(oauth)
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 仓库地址
        /// </summary>
        public string GitlabHost { get; set; }

        /// <summary>
        /// 管理员私钥
        /// </summary>
        public string AccountToken { get; set; }

        /// <summary>
        /// git 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// git 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 钩子通知地址
        /// </summary>
        public string WebHookHost { get; set; }

        /// <summary>
        /// 钩子密钥
        /// </summary>
        public string WebHookToken { get; set; }


        public string CallbackUrl
        {
            get { return $"{Host}/login"; }
        }

        public string AuthorizeUrl
        {
            get { return $"{GitlabHost}/oauth/authorize"; }
        }

        public string TokenUrl
        {
            get { return $"{GitlabHost}/oauth/token"; }
        }

        public string UserUrl
        {
            get { return $"{GitlabHost}/api/v4/user"; }
        }

        public string UserInfoUrl
        {
            get { return $"{GitlabHost}/oauth/userinfo"; }
        }

        public string GroupsUrl
        {
            get { return $"{GitlabHost}/api/v4/groups"; }
        }

        public string ProjectsUrl
        {
            get { return $"{GitlabHost}/api/v4/projects"; }
        }
        public string WebHookUrl
        {
            get { return GitlabHost + "/api/v4/projects/{0}/hooks"; }
        }
    }
}
