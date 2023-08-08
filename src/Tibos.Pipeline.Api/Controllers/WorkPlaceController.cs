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
    public class WorkPlaceController : ControllerBase
    {
        private readonly IWorkPlaceService _workPlaceService;
        private readonly IMapper _mapper;


        public WorkPlaceController(IWorkPlaceService workPlaceService, IMapper mapper)
        {
            _mapper = mapper;
            _workPlaceService = workPlaceService;
        }


        


        /// <summary>
        /// 查询工作台数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(JsonResponse<GetTopViewResponse>), 200)]
        public async Task<IActionResult> GetTopView([FromQuery]long envId)
        {
            var res = await _workPlaceService.GetTopView(envId);
            return Ok(res);
        }
    }
}