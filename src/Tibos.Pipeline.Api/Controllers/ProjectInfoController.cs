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
    /// ��Ŀ����
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProjectInfoController : ControllerBase
    {
        private readonly IProjectInfoService _projectInfoService;


        public ProjectInfoController(IProjectInfoService projectInfoService)
        {
            _projectInfoService = projectInfoService;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Create([FromBody]CreateProjectInfoRequest request)
        {
            var res = await _projectInfoService.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// ��ѯ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<ProjectInfoEntity>), 200)]
        public async Task<IActionResult> GetById([FromQuery] long id)
        {
            var res = await _projectInfoService.GetById(id);
            return Ok(res);
        }

        /// <summary>
        /// �޸�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Update([FromBody] UpdateProjectInfoRequest request)
        {
            var res = await _projectInfoService.Update(request);
            return Ok(res);
        }

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Delete([FromQuery] long id)
        {
            var res = await _projectInfoService.Delete(id);
            return Ok(res);
        }


        /// <summary>
        /// ��ѯ�б�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GetProjectInfoListResponse>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] GetProjectInfoListRequest request)
        {
            var res = await _projectInfoService.GetList(request);
            return Ok(res);
        }
    }
}