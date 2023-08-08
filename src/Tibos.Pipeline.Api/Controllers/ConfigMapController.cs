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
    /// ���ù���
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ConfigMapController : ControllerBase
    {
        private readonly IConfigInfoService _configInfoService;


        public ConfigMapController(IConfigInfoService configInfoService)
        {
            _configInfoService = configInfoService;
        }

        /// <summary>
        /// ��ѯ�����ֵ��б�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<ConfigInfoEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] GetConfigInfoListRequest request)
        {
            var res = await _configInfoService.GetList(request);
            return Ok(res);
        }


        /// <summary>
        /// ��ѯ�����ֵ��޸ļ�¼
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<List<GetConfigRecordResponse>>), 200)]
        public async Task<IActionResult> GetConfigRecord([FromQuery] GetConfigRecordRequest request)
        {
            var res = await _configInfoService.GetConfigRecord(request);
            return Ok(res);
        }


        /// <summary>
        /// ��ѯ�����ֵ�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<ConfigInfoEntity>), 200)]
        public async Task<IActionResult> GetById([FromQuery] long id)
        {
            var res = await _configInfoService.GetById(id);
            return Ok(res);
        }


        /// <summary>
        /// ɾ�������ֵ�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<ConfigInfoEntity>), 200)]
        public async Task<IActionResult> Delete([FromQuery] long id)
        {
            var res = await _configInfoService.Delete(id);
            return Ok(res);
        }

        /// <summary>
        /// ���������ֵ�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Create([FromBody] CreateConfigInfoRequest request)
        {
            var res = await _configInfoService.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// �޸������ֵ�
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Update([FromBody] UpdateConfigInfoRequest request)
        {
            var res = await _configInfoService.Update(request);
            return Ok(res);
        }


        /// <summary>
        /// �޸���������
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> UpdateConfigContent([FromBody] UpdateConfigContentRequest request)
        {
            var res = await _configInfoService.UpdateConfigContent(request);
            return Ok(res);
        }

        /// <summary>
        /// �޸����ù���״̬
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> UpdateConfigStatus([FromBody] UpdateConfigStatusRequest request)
        {
            var res = await _configInfoService.UpdateConfigStatus(request);
            return Ok(res);
        }

        /// <summary>
        /// ˢ�¹�������
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(JsonResponse), 200)]
        public async Task<IActionResult> Redeployment([FromBody] RedeploymentRequest request)
        {
            var res = await _configInfoService.Redeployment(request.EnvId,request.AppId);
            return Ok(res);
        }

    }
}