using Autofac;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibos.Pipeline.Api;
using Tibos.Pipeline.Api.Model.Enum;

namespace Tibos.Pipeline.Api.Domain.Service
{
    public class UserInfoExtensions
    {
        private readonly IHttpContextAccessor _httpContext;
        public UserInfoExtensions()
        {
            _httpContext = Program.container.Resolve<IHttpContextAccessor>();
        }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId
        {
            get
            {
                return _httpContext.HttpContext.User.Claims.ToList().FirstOrDefault(m => m.Type.Contains("UserId")).Value;
            }
        }

        /// <summary>
        /// 是否为管理员
        /// </summary>
        public bool IsAdmin
        {
            get 
            {
                return _httpContext.HttpContext.User.IsInRole("Admin");
            }
        }
    }
}
