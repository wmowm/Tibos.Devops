using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;
using Tibos.Pipeline.Api.Domain;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Domain.Service;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Entity;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;
using TTibos.Pipeline.Api.Domain.IService;

namespace Tibos.Pipeline.Api.Controllers
{
    /// <summary>
    /// 环境管理
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class EnvInfoController : ControllerBase
    {
        private readonly IEnvInfoService _envInfoService;
        private readonly IMapper _mapper;


        public EnvInfoController(IEnvInfoService envInfoService, IMapper mapper)
        {
            _envInfoService = envInfoService;
            _mapper = mapper;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Create([FromBody] CreateEnvInfoRequest request)
        {
            var res = await _envInfoService.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<EnvInfoEntity>), 200)]
        public async Task<IActionResult> GetById([FromQuery] long id)
        {
            var res = await _envInfoService.GetById(id);
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
        public async Task<IActionResult> Update([FromBody] UpdateEnvInfoRequest request)
        {
            var res = await _envInfoService.Update(request);
            return Ok(res);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Delete([FromQuery] long id)
        {
            var res = await _envInfoService.Delete(id);
            return Ok(res);
        }


        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<EnvInfoEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] GetEnvInfoListRequest request)
        {
            var res = await _envInfoService.GetList(request);
            return Ok(res);
        }



        /// <summary>
        /// 修改默认映射配置
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> UpdateMappingConfig([FromBody] UpdateMappingConfigRequest request)
        {
            var res = await _envInfoService.UpdateMappingConfig(request);
            return Ok(res);
        }

        /// <summary>
        /// 修改审核发布
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> UpdateCheckPublish([FromBody] UpdateCheckPublishRequest request)
        {
            var res = await _envInfoService.UpdateCheckPublish(request);
            return Ok(res);
        }
    }
}