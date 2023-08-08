
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using LibGit2Sharp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using Tibos.Pipeline.Api.AutofacConfig;
using Tibos.Pipeline.Api.Common;
using Tibos.Pipeline.Api.Domain;
using Tibos.Pipeline.Api.Domain.IService;
using Tibos.Pipeline.Api.Domain.Work;
using Tibos.Pipeline.Api.Filters;
using Tibos.Pipeline.Api.Model.Config;


namespace Tibos.Pipeline.Api
{
    public class Program
    {
        /// <summary>
        /// Autofac全局对象
        /// </summary>
        public static IContainer container;
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).PropertiesAutowired().InstancePerDependency();
                builder.RegisterModule<DefaultModule>();
            });



            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Tibos.Pipeline",
                    Description = "流水线接口",
                    Contact = new OpenApiContact() { Name = "Tibos", Email = "505613913@qq.com" }
                });

                #region jwt认证
                // 开启加权小锁
                option.OperationFilter<AddResponseHeadersFilter>();
                option.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                // 在header中添加token，传递到后台
                option.OperationFilter<SecurityRequirementsOperationFilter>();
                // Jwt Bearer 认证
                option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Authorization:Bearer {your JWT token},注意两者之间是一个空格",
                });
                #endregion

                option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"), true);

            });


            builder.Services.AddKubernetes(o =>
            {
                var kubePath = builder.Configuration.GetSection("Kubernetes:KubePath").Get<string>();
                o.KubePath = kubePath;
            });

            builder.Services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ResourceFilterAttribute));
                options.Filters.Add(typeof(ActionFilterAttribute));
                options.Filters.Add(typeof(ExceptionFilterAttribute));
                options.Filters.Add(typeof(ResultFilterAttribute));

            })
                .AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });




            //生成密钥
            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

            var symmetricKeyAsBase64 = jwtOptions.Secret;
            var keyByteArray = Encoding.UTF8.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            //认证参数
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(o =>
                {
                    
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,//是否验证签名,不验证的画可以篡改数据，不安全
                        IssuerSigningKey = signingKey,//解密的密钥
                        ValidateIssuer = true,//是否验证发行人，就是验证载荷中的Iss是否对应ValidIssuer参数
                        ValidIssuer = jwtOptions.Iss,//发行人
                        ValidateAudience = true,//是否验证订阅人，就是验证载荷中的Aud是否对应ValidAudience参数
                        ValidAudience = jwtOptions.Aud,//订阅人
                        ValidateLifetime = true,//是否验证过期时间，过期了就拒绝访问
                        ClockSkew = TimeSpan.Zero,//这个是缓冲过期时间，也就是说，即使我们配置了过期时间，这里也要考虑进去，过期时间+缓冲，默认好像是7分钟，你可以直接设置为0
                        RequireExpirationTime = true,
                    };
                    o.Events = new JwtBearerEvents
                    {
                        //权限验证失败后执行
                        OnChallenge = context =>
                        {
                            //终止默认的返回结果(必须有)
                            context.HandleResponse();
                            var result = JsonConvert.SerializeObject(new { code = "401", message = "验证失败" });
                            context.Response.ContentType = "application/json";
                            //验证失败返回401
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.WriteAsync(result);
                            return Task.FromResult(0);
                        },
                        OnForbidden = context => 
                        {
                            var result = JsonConvert.SerializeObject(new { code = "403", message = "权限不足" });
                            context.Response.ContentType = "application/json";
                            //验证失败返回403
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.WriteAsync(result);
                            return Task.FromResult(0);
                        }
                    };
                });

            builder.Services.AddDbContext<PipelineDBContext>(optionsBuilder =>
            {
                var connectionSetting = builder.Configuration.GetConnectionString("Default");


                if (connectionSetting == null)
                {
                    throw new Exception("未配置数据库连接");
                }
                optionsBuilder.UseMySql(connectionSetting, MySqlServerVersion.LatestSupportedServerVersion);
            });

            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<HttpClientHelper>();

            //后台任务
            builder.Services.AddHostedService<PublishTask>();
            builder.Services.AddHostedService<NodeMetricsTask>();

            //配置跨域处理
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyMethod()
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            //添加AutoMapper
            builder.Services.AddAutoMapper(config =>
            {
                config.ValidateInlineMaps = false;

            });

            builder.Services.Configure<GitlabOptions>(builder.Configuration.GetSection("Gitlab"));
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
            builder.Services.Configure<DockerOptions>(builder.Configuration.GetSection("Docker"));

            var app = builder.Build();
        
            app.UseCors("any");

            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.Run();



        }
    }
}