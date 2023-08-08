using AutoMapper;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using Tibos.Pipeline.Api.Common;
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
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<WebHookController> _logger;
        private readonly IBuildRecordService _buildRecordService;
        private readonly IPublishRecordService _publishRecordService;

        private readonly PipelineDBContext _context;
        private readonly IMapper _mapper;
        private readonly IGitlabService _gitlabService;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IAppInfoService _appInfoService;
        private readonly IOptions<DockerOptions> _dockerOptions;
        private readonly IKubenetesService _kubenetesService;
        private readonly IContainerService _cloudService;
        private readonly IWorkPlaceService _workPlaceService;
        public TestController(IKubenetesService kubenetesService, 
            PipelineDBContext context, 
            IMapper mapper, 
            IGitlabService gitlabService, 
            IOptions<JwtOptions> jwtOptions, 
            IAppInfoService appInfoService,
            IOptions<DockerOptions> dockerOptions,
            IContainerService cloudService,
            IPublishRecordService publishRecordService,
            IWorkPlaceService workPlaceService
            )
        {
            _context = context;
            _mapper = mapper;
            _gitlabService = gitlabService;
            _jwtOptions = jwtOptions;
            _appInfoService = appInfoService;
            _kubenetesService = kubenetesService;
            _publishRecordService = publishRecordService;
            _dockerOptions = dockerOptions;
            _cloudService = cloudService;
            _workPlaceService = workPlaceService;
        }

        /// <summary>
        /// ≤‚ ‘
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Test()
        {
            return new JsonResult(200);
        }

    }
}