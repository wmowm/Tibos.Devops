using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tibos.Pipeline.Api.Filters
{
    public class ExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        private readonly ILogger<ExceptionFilterAttribute> logger;
 
         public ExceptionFilterAttribute(ILoggerFactory loggerFactory)
         {
             logger = loggerFactory.CreateLogger<ExceptionFilterAttribute>();
         }
 
         public void OnException(ExceptionContext context)
         {
            //context.Exception.ToExceptionless().Submit();
            logger.LogError("Exception Execute! Message:" + context.Exception.Message);
            context.ExceptionHandled = true;
         }
    }
}
