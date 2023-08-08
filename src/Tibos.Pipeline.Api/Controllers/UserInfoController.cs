using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Domain;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;
using TTibos.Pipeline.Api.Domain.IService;

namespace Tibos.Pipeline.Api.Controllers
{
    /// <summary>
    /// �û�����
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class UserInfoController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;


        public UserInfoController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        /// <summary>
        /// �����û���¼����
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUserLogin([FromBody] CreateUserLoginRequest request)
        {
            var res = await _userInfoService.CreateUserLogin(request);
            return Ok(res);
        }

        /// <summary>
        /// ��ѯ����¼����
        /// </summary>
        /// <param name="userId">�û����</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<GetUserLoginResponse>), 200)]
        public async Task<IActionResult> GetUserLogin([FromQuery] string userId)
        {
            var res = await _userInfoService.GetUserLogin(userId);
            return Ok(res);
        }

        /// <summary>
        /// �޸�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoRequest request)
        {
            var res = await _userInfoService.UpdateUserInfo(request);
            return Ok(res);
        }

        /// <summary>
        /// �޸��û�״̬
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusRequest request)
        {
            var res = await _userInfoService.UpdateUserStatus(request);
            return Ok(res);
        }


        /// <summary>
        /// ��ѯ�û��б�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<UserInfoResponse>>), 200)]
        public async Task<IActionResult> GetUserInfoList([FromQuery] GetUserInfoListRequest request)
        {
            var res = await _userInfoService.GetUserInfoList(request);
            return Ok(res);
        }

        /// <summary>
        /// ��ȡ���н�ɫ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<EnumberEntity>>), 200)]
        public async Task<IActionResult> GetRoles()
        {
            var res = await _userInfoService.GetRoles();
            return Ok(res);
        }
    }
}