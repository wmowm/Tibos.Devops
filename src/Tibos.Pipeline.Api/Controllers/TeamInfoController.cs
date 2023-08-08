using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    /// <summary>
    /// �Ŷӹ���
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TeamInfoController : ControllerBase
    {
        private readonly ITeamInfoService _teamInfoService;
        private readonly IGitlabService _gitlabService;


        public TeamInfoController(ITeamInfoService teamInfoService, IGitlabService gitlabService)
        {
            _teamInfoService = teamInfoService;
            _gitlabService = gitlabService;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Create(IFormCollection filesForm, [FromBody]CreateTeamInfoRequest request)
        {
            var res = await _teamInfoService.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// ��ѯ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<TeamInfoEntity>), 200)]
        public async Task<IActionResult> GetById([FromQuery] long id)
        {
            var res = await _teamInfoService.GetById(id);
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
        public async Task<IActionResult> Update([FromBody] UpdateTeamInfoRequest request)
        {
            var res = await _teamInfoService.Update(request);
            return Ok(res);
        }

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Delete([FromQuery] long id)
        {
            var res = await _teamInfoService.Delete(id);
            return Ok(res);
        }


        /// <summary>
        /// ��ѯ�б�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<TeamInfoEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] GetTeamInfoListRequest request)
        {
            var res = await _teamInfoService.GetList(request);
            return Ok(res);
        }

        /// <summary>
        /// ��ѯ��
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GitlabGetGroupsResponse>>), 200)]
        public async Task<IActionResult> GetGroups()
        {
            var res = await _gitlabService.GetGroups();
            return Ok(res);
        }



        /// <summary>
        /// �����Ŷ�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> JoinTeam([FromBody] JoinTeamRequest request)
        {
            var res = await _teamInfoService.JoinTeam(request);
            return Ok(res);
        }

        /// <summary>
        /// �鿴�Ŷ��û��б�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<UserInfoEntity>>), 200)]
        public async Task<IActionResult> GetTeamUserList([FromQuery] GetTeamUserListRequest request)
        {
            var res = await _teamInfoService.GetTeamUserList(request);
            return Ok(res);
        }


        /// <summary>
        /// ��ѯδ������Ŷӵ�ȫ���û�
        /// </summary>
        /// <param name="teamId">�Ŷӱ��</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GetTeamUserListResponse>>), 200)]
        public async Task<IActionResult> GetNotJoinUserList([FromQuery] long teamId)
        {
            var res = await _teamInfoService.GetNotJoinUserList(teamId);
            return Ok(res);
        }

        /// <summary>
        /// �Ƴ��Ŷ�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> RemoveTeamUser([FromQuery] RemoveTeamUserRequest request)
        {
            var res = await _teamInfoService.RemoveTeamUser(request);
            return Ok(res);
        }
    }
}