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
    /// 应用管理
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class AppInfoController : ControllerBase
    {
        private readonly IAppInfoService _appInfoService;
        private readonly IBuildRecordService _buildRecordService;


        public AppInfoController(IAppInfoService appInfoService, IBuildRecordService buildRecordService)
        {
            _appInfoService = appInfoService;
            _buildRecordService = buildRecordService;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Create([FromBody]CreateAppInfoRequest request)
        {
            var res = await _appInfoService.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<GetAppInfoResponse>), 200)]
        public async Task<IActionResult> GetById([FromQuery] long id)
        {
            var res = await _appInfoService.GetById(id);
            return Ok(res);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Update([FromBody] UpdateAppInfoRequest request)
        {
            var res = await _appInfoService.Update(request);
            return Ok(res);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Delete([FromQuery] long id)
        {
            var res = await _appInfoService.Delete(id);
            return Ok(res);
        }


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GetAppInfoListResponse>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] GetAppInfoListRequest request)
        {
            var res = await _appInfoService.GetList(request);
            return Ok(res);
        }

        /// <summary>
        /// 查询构建记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GetBuildListResponse>>), 200)]
        public async Task<IActionResult> GetBuildList([FromQuery] GetBuildListRequest request)
        {
            var res = await _buildRecordService.GetBuildList(request);
            return Ok(res);
        }


        /// <summary>
        /// 查询项目成功的构建记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GetBuildListResponse>>), 200)]
        public async Task<IActionResult> GetSuccessBuildList([FromQuery] GetSuccessBuildListRequest request)
        {
            var res = await _buildRecordService.GetSuccessBuildList(request);
            return Ok(res);
        }

        /// <summary>
        /// 应用收藏/取消
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> FavoriteApp([FromBody] FavoriteAppRequest request)
        {
            var res = await _appInfoService.FavoriteApp(request);
            return Ok(res);
        }
    }
}