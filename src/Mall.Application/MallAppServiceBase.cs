﻿using System;
using Abp.Application.Services;
using Abp.Authorization;

namespace Mall
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    [AbpAuthorize]
    public abstract class MallAppServiceBase : ApplicationService
    {
        protected MallAppServiceBase()
        {
            LocalizationSourceName = MallConsts.LocalizationSourceName;
        }

        /// <summary>
        /// 获取当前登陆人的UserId
        /// </summary>
        protected int UserId
        {
            get
            {
                return Convert.ToInt16(AbpSession.UserId);
            }
        }
    }
}