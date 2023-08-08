using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Xml.Linq;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Response;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tibos.Pipeline.Api.Domain.Service
{
    public class GitlabService: IGitlabService
    {
        private readonly IOptions<GitlabOptions> _options;
        private readonly HttpClientHelper _httpClient;
        public GitlabService(IOptions<GitlabOptions> options, HttpClientHelper httpClient) 
        {
            _options = options;
            _httpClient = httpClient;
        }

        /// <summary>
        /// 获取认证地址
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResponse<string>> Authorize() 
        {
            var response = new JsonResponse<string>();
            try
            {
                var url = $"http://gitlab.wmowm.com:880/oauth/authorize?response_type=code&client_id={_options.Value.AppId}&redirect_uri={_options.Value.CallbackUrl}&scope={_options.Value.Scope}";
                response.data = url;
                return response;
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<JsonResponse<OAuthTokenResponse>> AuthToken(string code) 
        {
            var response = new JsonResponse<OAuthTokenResponse>();
            try
            {
                var dict = new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["code"] = code,
                    ["client_id"] = _options.Value.AppId,
                    ["client_secret"] = _options.Value.AppKey,
                    ["redirect_uri"] = _options.Value.CallbackUrl
                };
                var res = await _httpClient.PostData(_options.Value.TokenUrl, JsonConvert.SerializeObject(dict));
                var authorizeResult = JsonConvert.DeserializeObject<OAuthTokenResponse>(res);
                response.data = authorizeResult;
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<JsonResponse<GitlabUserResponse>> GetUser(string access_token) 
        {
            var response = new JsonResponse<GitlabUserResponse>();
            try
            {
                var url = _options.Value.UserUrl;
                var res = await _httpClient.GetData(url, new Dictionary<string, string>() { { "Authorization", $"Bearer {access_token}" } });
                response.data = JsonConvert.DeserializeObject<GitlabUserResponse>(res);
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<JsonResponse<GitlabUserInfoResponse>> GetUserInfo(string access_token)
        {
            var response = new JsonResponse<GitlabUserInfoResponse>();
            try
            {
                var url = $"{_options.Value.UserInfoUrl}?access_token={access_token}";
                var res = await _httpClient.GetData(url);
                response.data = JsonConvert.DeserializeObject<GitlabUserInfoResponse>(res);
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 查询所有组
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResponse<List<GitlabGetGroupsResponse>>> GetGroups() 
        {
            var response = new JsonResponse<List<GitlabGetGroupsResponse>>();
            try
            {
                var url = $"{_options.Value.GroupsUrl}?access_token={_options.Value.AccountToken}";
                var res = await _httpClient.GetData(url);
                response.data = JsonConvert.DeserializeObject<List<GitlabGetGroupsResponse>>(res);
            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// 创建项目
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResponse<GitlabCreaetProjectResponse>> CreaetProject(int groupId,string projectName)
        {
            var response = new JsonResponse<GitlabCreaetProjectResponse>();
            try
            {
                var url = $"{_options.Value.ProjectsUrl}?access_token={_options.Value.AccountToken}";
                var res = await _httpClient.GetData($"{url}&search={projectName}");
                var list = JsonConvert.DeserializeObject<List<GitlabCreaetProjectResponse>>(res);
                if(list != null && list.Any(m=>m.@namespace.id == groupId && m.name == projectName)) 
                {
                    response.data = list.FirstOrDefault(m => m.@namespace.id == groupId && m.name == projectName);
                }
                else 
                {
                    var data = JsonConvert.SerializeObject(new { namespace_id = groupId, name = projectName });
                    res = await _httpClient.PostData(url, data);
                    response.data = JsonConvert.DeserializeObject<GitlabCreaetProjectResponse>(res);
                }
                //创建webhook
                url = string.Format(_options.Value.WebHookUrl, response.data.id) + $"?access_token={_options.Value.AccountToken}";
                res = await _httpClient.PostData(url, JsonConvert.SerializeObject(new 
                {
                    enable_ssl_verification = false,
                    token=_options.Value.WebHookToken,
                    url= _options.Value.WebHookHost,
                    job_events = true
                }));

                //创建分支
                url = $"{_options.Value.ProjectsUrl}/{response.data.id}/repository/branches/?access_token={_options.Value.AccountToken}&branch=develop&ref=main";
                res = await _httpClient.PostData(url,"");

            }
            catch (Exception ex)
            {
                response.code = "-1";
                response.message = ex.Message;
            }
            return response;
        }
    }
}
