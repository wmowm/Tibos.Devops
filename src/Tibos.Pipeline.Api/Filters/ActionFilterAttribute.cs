using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Tibos.Pipeline.Api.Model.Config;
using Tibos.Pipeline.Api.Model.Response;
using Tibos.Pipeline.Api.Controllers;

namespace Tibos.Pipeline.Api.Filters
{
    public class ActionFilterAttribute : Attribute, IActionFilter
    {

        private readonly ILogger<ActionFilterAttribute> logger;
        private readonly IOptions<GitlabOptions> _gitlabOptions;

        public ActionFilterAttribute(ILoggerFactory loggerFactory, IOptions<GitlabOptions> authOptions
        )
        {
            logger = loggerFactory.CreateLogger<ActionFilterAttribute>();
            _gitlabOptions = authOptions;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var controllerName = context.RouteData.Values["controller"] as string;
            var actionName = context.RouteData.Values["action"] as string;
            var token = request.Headers["X-Gitlab-Token"];
            #region 根据注解允许匿名访问
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            //controller
            var controllerAttributes = actionDescriptor.MethodInfo.DeclaringType.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);
            if (controllerAttributes != null && controllerAttributes.Length > 0)
            {
                return;
            }
            //action
            var actionAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true);
            if (actionAttributes != null && actionAttributes.Length > 0)
            {
                return;
            }
            #endregion

            if (controllerName.ToLower() == typeof(WebHookController).Name.ToLower())
            {
                if (token != _gitlabOptions.Value.WebHookToken)
                {
                    context.Result = new JsonResult(new JsonResponse { code = "401", message = "未授权" });
                    return;
                }
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
