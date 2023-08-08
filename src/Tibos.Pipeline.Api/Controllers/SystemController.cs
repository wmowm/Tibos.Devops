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
using Tibos.Pipeline.Api.Model.Enum;
using Tibos.Pipeline.Api.Model.Request;
using Tibos.Pipeline.Api.Model.Response;
using TTibos.Pipeline.Api.Domain.IService;

namespace Tibos.Pipeline.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SystemController : ControllerBase
    {
        private readonly ILogger<WebHookController> _logger;
        private readonly IBuildRecordService _buildRecordService;


        private readonly PipelineDBContext _context;
        private readonly IMapper _mapper;
        private readonly IGitlabService _gitlabService;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public SystemController(PipelineDBContext context, IMapper mapper, IGitlabService gitlabService, IOptions<JwtOptions> jwtOptions)
        {
            _context = context;
            _mapper = mapper;
            _gitlabService = gitlabService;
            _jwtOptions = jwtOptions;
        }


        /// <summary>
        /// 获取路由
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResponse<List<ChildrenResponse>>> Routes()
        {
            JsonResponse<List<ChildrenResponse>> response = new JsonResponse<List<ChildrenResponse>>();
            response.data = new List<ChildrenResponse>();


            var route = new ChildrenResponse()
            {
                router = "root",
                children = new List<ChildrenResponse>()
                {
                    new ChildrenResponse()
                    {
                        router = "dashboard",
                        icon="dashboard",
                        children = new List<ChildrenResponse>()
                        {
                            new ChildrenResponse(){ router = "workplace"}
                        }
                    },
                    new ChildrenResponse()
                     {
                        router = "integration",
                        name = "持续集成",
                        icon = "ci",
                        children = new List<ChildrenResponse>()
                        {
                            new ChildrenResponse(){ router = "project"},
                            new ChildrenResponse(){ router = "app"},
                            new ChildrenResponse(){ router = "build"}
                        }
                     },
                    new ChildrenResponse()
                     {
                        router = "deployment",
                        name = "部署管理",
                        icon = "cloud",
                        children = new List<ChildrenResponse>()
                        {
                             new ChildrenResponse(){ router = "publish"},
                             new ChildrenResponse(){ router = "container"},
                             new ChildrenResponse(){ router = "configmap"},
                             new ChildrenResponse(){ router = "configrecord"}
                        }
                     }
                }
            };
            var isAdmin = HttpContext.User.IsInRole("Admin");
            if (isAdmin) 
            {
                route.children.Add(new ChildrenResponse()
                {
                    router = "system",
                    name = "系统设置",
                    icon = "setting",
                    children = new List<ChildrenResponse>()
                        {
                            new ChildrenResponse(){ router = "user"},
                            new ChildrenResponse(){ router = "team"},
                            new ChildrenResponse(){ router = "template"},
                            new ChildrenResponse(){ router = "env"}
                        }
                });
            }
            response.data.Add(route);





            return response;
        }

    }
}