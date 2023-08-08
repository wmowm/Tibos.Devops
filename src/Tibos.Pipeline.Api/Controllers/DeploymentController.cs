using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    /// 部署管理
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class DeploymentController : ControllerBase
    {
        private readonly IPublishRecordService _publishRecordService;


        public DeploymentController(IPublishRecordService publishRecordService)
        {
            _publishRecordService = publishRecordService;
        }

        /// <summary>
        /// 查询发布记录列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GetPublishListResponse>>), 200)]
        public async Task<IActionResult> GetPublishList([FromQuery] GetPublishListRequest request)
        {
            var res = await _publishRecordService.GetPublishList(request);
            return Ok(res);
        }


        /// <summary>
        /// 创建部署
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> CreatePublish([FromBody] CreatePublishRequest request)
        {
            var res = await _publishRecordService.CreatePublish(request);
            return Ok(res);
        }

        /// <summary>
        /// 回滚部署
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> RollBackPublish([FromBody] RollBackPublishRequest request)
        {
            var res = await _publishRecordService.RollBackPublish(request);
            return Ok(res);
        }
    }
}