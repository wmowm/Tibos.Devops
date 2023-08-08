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
    /// 容器管理
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ContainerController : ControllerBase
    {
        private readonly IContainerService _cloudService;


        public ContainerController(IContainerService cloudService)
        {
            _cloudService = cloudService;
        }

        /// <summary>
        /// 获取pod列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GetPodListResponse>>), 200)]
        public async Task<IActionResult> GetPodList([FromQuery] GetPodListRequest request)
        {
            var res = await _cloudService.GetPodList(request);
            return Ok(res);
        }


        /// <summary>
        /// 重启容器
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> RestartPod([FromBody] GetPodListRequest request)
        {
            var res = await _cloudService.RestartPod(request);
            return Ok(res);
        }

        /// <summary>
        /// 伸缩容器
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> ScalePod([FromBody] ScalePodRequest request)
        {
            var res = await _cloudService.ScalePod(request);
            return Ok(res);
        }

        /// <summary>
        /// 伸缩容器日志
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<string>>), 200)]
        public async Task<IActionResult> GetPodLog([FromQuery] GetPodLogRequest request)
        {
            var res = await _cloudService.GetPodLog(request);
            return Ok(res);
        }
    }
}