using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using Tibos.Pipeline.Api.Domain;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;
using TTibos.Pipeline.Api.Domain.IService;

namespace Tibos.Pipeline.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class MyInfoController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;
        private readonly ITeamInfoService _teamInfoService;
        private readonly IMapper _mapper;
        private readonly IProjectInfoService _projectInfoService;
        private readonly IAppInfoService _appInfoService;


        public MyInfoController(IUserInfoService userInfoService, IMapper mapper, ITeamInfoService teamInfoService, IProjectInfoService projectInfoService, IAppInfoService appInfoService)
        {
            _mapper = mapper;
            _userInfoService = userInfoService;
            _teamInfoService = teamInfoService;
            _projectInfoService = projectInfoService;
            _appInfoService = appInfoService;
        }


        /// <summary>
        /// 账号密码登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonResponse<LoginResponse>), 200)]
        public async Task<IActionResult> Login([FromBody] AccountLoginRequest request)
        {
            var res = await _userInfoService.Login(request);
            return Ok(res);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> UpdateUserPwd([FromBody] UpdateUserPwdRequest request)
        {
            var dto = _mapper.Map<UpdateUserPwdDto>(request);
            dto.UserId = HttpContext.User.Claims.ToList().FirstOrDefault(m => m.Type.Contains("UserId")).Value;
            var res = await _userInfoService.UpdateUserPwd(dto);
            return Ok(res);
        }


        /// <summary>
        /// 查询用户加入的团队
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<TeamInfoEntity>>), 200)]
        public async Task<IActionResult> GetUserTeamList()
        {
            var userId = HttpContext.User.Claims.ToList().FirstOrDefault(m => m.Type.Contains("UserId")).Value;
            var res = await _teamInfoService.GetUserTeamList(userId);
            return Ok(res);
        }

        /// <summary>
        /// 查询用户参与的项目
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GetProjectInfoListResponse>>), 200)]
        public async Task<IActionResult> GetUserProjectList()
        {
            var res = await _projectInfoService.GetUserProjectList();
            return Ok(res);
        }

        /// <summary>
        /// 查询用户收藏的应用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<AppInfoEntity>>), 200)]
        public async Task<IActionResult> GetFavoriteAppList()
        {
            var res = await _appInfoService.GetFavoriteAppList();
            return Ok(res);
        }
    }
}