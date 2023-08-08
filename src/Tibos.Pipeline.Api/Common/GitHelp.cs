using LibGit2Sharp;

namespace Tibos.Pipeline.Api.Common
{
    public static class GitHelp
    {
        /// <summary>
        /// git拉取代码
        /// </summary>
        /// <param name="account">git账号</param>
        /// <param name="password">git密码</param>
        /// <param name="gitlabPath">gitlab项目地址</param>
        /// <param name="path">本地仓库地址</param>
        public static void GitPull(string account,string password,string gitlabPath,string path)
        {
            if (Directory.Exists(path)) 
            {
                Directory.Delete(path, true);
            }
            var option = new CloneOptions();
            option.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = account, Password = password };
            //克隆
            Repository.Clone(gitlabPath, path, option);

        }

        /// <summary>
        /// 创建分支推送到远程仓库
        /// </summary>
        public static void GitPush(string account, string password, string path, string brancheName = "develop")
        {
            var repOptions = new RepositoryOptions();


            using (var repo = new Repository(path))
            {
                var myBranche = repo.Branches.FirstOrDefault(m => m.FriendlyName == brancheName);
                if (myBranche == null)
                {
                    myBranche = repo.CreateBranch(brancheName);
                }
                Commands.Checkout(repo, myBranche);



                //添加到暂存区
                Commands.Stage(repo, $"{path}/*");
                Signature signature = repo.Config.BuildSignature(DateTimeOffset.Now);

                repo.Commit("updating files..", new Signature(account, account, DateTimeOffset.Now),
new Signature(account, account, DateTimeOffset.Now));


                var options = new PushOptions();
                options.CredentialsProvider = (_url, _user, _cred) =>
                    new UsernamePasswordCredentials { Username = account, Password = password };

                //推送
                Remote remote = repo.Network.Remotes["origin"];
                repo.Network.Push(remote, $"refs/heads/{brancheName}", options);
            }
        }
    }
}
