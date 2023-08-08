using System;
using System.Collections.Generic;

namespace Tibos.Pipeline.Api.Model.Request
{



    public class HookRequest
    {
        public string object_kind { get; set; }
        public string @ref { get; set; }
        public bool tag { get; set; }
        public string before_sha { get; set; }
        public string sha { get; set; }
        public int build_id { get; set; }
        public string build_name { get; set; }
        public string build_stage { get; set; }
        public string build_status { get; set; }
        public string build_created_at { get; set; }
        public string build_started_at { get; set; }
        public string build_finished_at { get; set; }
        public decimal? build_duration { get; set; }
        public decimal? build_queued_duration { get; set; }
        public bool build_allow_failure { get; set; }
        public string build_failure_reason { get; set; }
        public int pipeline_id { get; set; }
        public Runner runner { get; set; }
        public int project_id { get; set; }
        public string project_name { get; set; }
        public User user { get; set; }
        public Commit commit { get; set; }
        public Repository repository { get; set; }
        public object environment { get; set; }
    }

    public class Runner
    {
        public int id { get; set; }
        public string description { get; set; }
        public string runner_type { get; set; }
        public bool active { get; set; }
        public bool is_shared { get; set; }
        public string[] tags { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string avatar_url { get; set; }
        public string email { get; set; }
    }

    public class Commit
    {
        public int id { get; set; }
        public string sha { get; set; }
        public string message { get; set; }
        public string author_name { get; set; }
        public string author_email { get; set; }
        public string author_url { get; set; }
        public string status { get; set; }
        public decimal? duration { get; set; }
        public string started_at { get; set; }
        public string finished_at { get; set; }
    }

    public class Repository
    {
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string homepage { get; set; }
        public string git_http_url { get; set; }
        public string git_ssh_url { get; set; }
        public int? visibility_level { get; set; }
    }


}
