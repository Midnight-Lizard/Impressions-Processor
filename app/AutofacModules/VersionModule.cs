using Autofac;
using MidnightLizard.Impressions.Infrastructure;
using MidnightLizard.Impressions.Infrastructure.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Impressions.Processor.AutofacModules
{
    public class VersionModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(SchemaVersion.Latest);
        }
    }
}
