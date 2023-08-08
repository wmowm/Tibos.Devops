namespace Tibos.Pipeline.Api.Model.Response
{
    public class GitlabCreaetProjectResponse
    {
        public long id { get; set; }

        public string description { get; set; }

        public string name { get; set; }
        public string created_at { get; set; }
        public string ssh_url_to_repo { get; set; }
        public string http_url_to_repo { get; set; }
        public string web_url { get; set; }

        public GitlabNamespace @namespace{ get;set;}
    }

    public class GitlabNamespace
    {
        public long id { get; set; }

        public string name { get; set; }
    }
}
