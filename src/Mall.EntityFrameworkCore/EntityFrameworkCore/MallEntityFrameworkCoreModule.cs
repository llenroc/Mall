﻿using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Mall.EntityFrameworkCore
{
    [DependsOn(
        typeof(MallCoreModule), 
        typeof(AbpEntityFrameworkCoreModule))]
    public class MallEntityFrameworkCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MallEntityFrameworkCoreModule).GetAssembly());
        }
    }
}