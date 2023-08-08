using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;

namespace Tibos.Pipeline.Api.Domain.IService
{
    public interface IUserInfoService
    {
        Task<JsonResponse<LoginResponse>> GitlabLogin(string code);

        Task<JsonResponse<LoginResponse>> Login(AccountLoginRequest request);

        Task<JsonResponse> CreateUserLogin(CreateUserLoginRequest request);

        Task<JsonResponse> UpdateUserInfo(UpdateUserInfoRequest request);

        Task<JsonResponse> UpdateUserStatus(UpdateUserStatusRequest request);

        Task<JsonResponse<GetUserLoginResponse>> GetUserLogin(string userId);

        Task<JsonResponse<List<UserInfoResponse>>> GetUserInfoList(GetUserInfoListRequest request);

        Task<JsonResponse<List<EnumberEntity>>> GetRoles();

        Task<JsonResponse> UpdateUserPwd(UpdateUserPwdDto request);
    }
}
