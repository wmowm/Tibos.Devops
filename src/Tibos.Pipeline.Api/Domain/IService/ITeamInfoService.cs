using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface ITeamInfoService
    {
        Task<JsonResponse> Create(CreateTeamInfoRequest request);
        Task<JsonResponse> Update(UpdateTeamInfoRequest request);

        Task<JsonResponse> Delete(long id);

        Task<JsonResponse<TeamInfoEntity>> GetById(long id);

        Task<JsonResponse<List<TeamInfoEntity>>> GetList(GetTeamInfoListRequest request);

        Task<JsonResponse> JoinTeam(JoinTeamRequest request);

        Task<JsonResponse<List<GetTeamUserListResponse>>> GetTeamUserList(GetTeamUserListRequest request);

        Task<JsonResponse<List<UserInfoEntity>>> GetNotJoinUserList(long teamId);

        Task<JsonResponse> RemoveTeamUser(RemoveTeamUserRequest request);

        Task<JsonResponse<List<TeamInfoEntity>>> GetUserTeamList(string userId);
    }
}
