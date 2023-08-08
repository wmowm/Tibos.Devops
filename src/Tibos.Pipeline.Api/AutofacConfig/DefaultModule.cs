using Autofac;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Reflection;

namespace Tibos.Pipeline.Api.AutofacConfig
{
    public class DefaultModule : Autofac.Module
    {
        //Autofac容器
        protected override void Load(ContainerBuilder builder)
        {
            //程序集注入
            var Domain = Assembly.Load("Tibos.Pipeline.Api");

            //根据名称约定，实现领域
            builder.RegisterAssemblyTypes(Domain, Domain)
              .Where(t => t.Name.EndsWith("Service"))
              .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterBuildCallback(scope =>
            {
                Program.container = (IContainer)scope;
            });

        }
    }
}
