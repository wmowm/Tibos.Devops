
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
        /// Autofacȫ�ֶ���
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
                    Description = "��ˮ�߽ӿ�",
                    Contact = new OpenApiContact() { Name = "Tibos", Email = "505613913@qq.com" }
                });

                #region jwt��֤
                // ������ȨС��
                option.OperationFilter<AddResponseHeadersFilter>();
                option.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                // ��header�����token�����ݵ���̨
                option.OperationFilter<SecurityRequirementsOperationFilter>();
                // Jwt Bearer ��֤
                option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Authorization:Bearer {your JWT token},ע������֮����һ���ո�",
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
                //����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //��ʹ���շ���ʽ��key
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //����ʱ���ʽ
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });




            //������Կ
            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

            var symmetricKeyAsBase64 = jwtOptions.Secret;
            var keyByteArray = Encoding.UTF8.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            //��֤����
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(o =>
                {
                    
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,//�Ƿ���֤ǩ��,����֤�Ļ����Դ۸����ݣ�����ȫ
                        IssuerSigningKey = signingKey,//���ܵ���Կ
                        ValidateIssuer = true,//�Ƿ���֤�����ˣ�������֤�غ��е�Iss�Ƿ��ӦValidIssuer����
                        ValidIssuer = jwtOptions.Iss,//������
                        ValidateAudience = true,//�Ƿ���֤�����ˣ�������֤�غ��е�Aud�Ƿ��ӦValidAudience����
                        ValidAudience = jwtOptions.Aud,//������
                        ValidateLifetime = true,//�Ƿ���֤����ʱ�䣬�����˾;ܾ�����
                        ClockSkew = TimeSpan.Zero,//����ǻ������ʱ�䣬Ҳ����˵����ʹ���������˹���ʱ�䣬����ҲҪ���ǽ�ȥ������ʱ��+���壬Ĭ�Ϻ�����7���ӣ������ֱ������Ϊ0
                        RequireExpirationTime = true,
                    };
                    o.Events = new JwtBearerEvents
                    {
                        //Ȩ����֤ʧ�ܺ�ִ��
                        OnChallenge = context =>
                        {
                            //��ֹĬ�ϵķ��ؽ��(������)
                            context.HandleResponse();
                            var result = JsonConvert.SerializeObject(new { code = "401", message = "��֤ʧ��" });
                            context.Response.ContentType = "application/json";
                            //��֤ʧ�ܷ���401
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.WriteAsync(result);
                            return Task.FromResult(0);
                        },
                        OnForbidden = context => 
                        {
                            var result = JsonConvert.SerializeObject(new { code = "403", message = "Ȩ�޲���" });
                            context.Response.ContentType = "application/json";
                            //��֤ʧ�ܷ���403
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
                    throw new Exception("δ�������ݿ�����");
                }
                optionsBuilder.UseMySql(connectionSetting, MySqlServerVersion.LatestSupportedServerVersion);
            });

            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<HttpClientHelper>();

            //��̨����
            builder.Services.AddHostedService<PublishTask>();
            builder.Services.AddHostedService<NodeMetricsTask>();

            //���ÿ�����
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

            //���AutoMapper
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