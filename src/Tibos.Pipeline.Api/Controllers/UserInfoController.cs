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
    /// 用户管理
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
        /// 创建用户登录类型
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
        /// 查询户登录类型
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<GetUserLoginResponse>), 200)]
        public async Task<IActionResult> GetUserLogin([FromQuery] string userId)
        {
            var res = await _userInfoService.GetUserLogin(userId);
            return Ok(res);
        }

        /// <summary>
        /// 修改
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
        /// 修改用户状态
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
        /// 查询用户列表
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
        /// 获取所有角色
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