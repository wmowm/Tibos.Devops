using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface IGitlabService
    {
        Task<JsonResponse<string>> Authorize();

        Task<JsonResponse<OAuthTokenResponse>> AuthToken(string code);

        Task<JsonResponse<GitlabUserResponse>> GetUser(string access_token);

        Task<JsonResponse<GitlabUserInfoResponse>> GetUserInfo(string access_token);

        Task<JsonResponse<List<GitlabGetGroupsResponse>>> GetGroups();

        Task<JsonResponse<GitlabCreaetProjectResponse>> CreaetProject(int groupId, string projectName);
    }
}
