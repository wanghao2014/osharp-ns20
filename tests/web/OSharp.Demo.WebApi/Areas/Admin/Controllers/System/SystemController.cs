﻿// -----------------------------------------------------------------------
//  <copyright file="SystemController.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2018 OSharp. All rights reserved.
//  </copyright>
//  <site>http://www.osharp.org</site>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2018-03-21 9:51</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OSharp.Collections;
using OSharp.Core.Modules;
using OSharp.Entity;
using OSharp.Reflection;


namespace OSharp.Demo.WebApi.Areas.Admin.Controllers
{
    [Description("管理-系统")]
    public class SystemController : AdminApiController
    {
        private readonly IServiceProvider _provider;

        public SystemController(IServiceProvider provider)
        {
            _provider = provider;
        }

        [Description("信息")]
        public IActionResult Info()
        {
            dynamic info = new ExpandoObject();

            //模块信息
            OSharpModuleManager moduleManager = _provider.GetService<OSharpModuleManager>();
            info.Modules = moduleManager.SourceModules.OrderBy(m => m.Level).ThenBy(m => m.Order).Select(m => new
            {
                m.GetType().Name,
                Class = m.GetType().FullName,
                Level = m.Level.ToString(),
                m.Order,
                m.IsEnabled
            }).ToList();

            string version = Assembly.GetExecutingAssembly().GetProductVersion();

            MvcOptions mvcOps = _provider.GetService<IOptions<MvcOptions>>().Value;

            info.Lines = new List<string>()
            {
                "WebApi 数据服务已启动",
                $"版本号：{version}",
                $"数据连接：{_provider.GetOSharpOptions().GetDbContextOptions(typeof(DefaultDbContext)).ConnectionString}",
                $"MvcFilters：\r\n{mvcOps.Filters.ExpandAndToString(m=>$"{m.ToString()}-{m.GetHashCode()}", "\r\n")}"
            };

            return Json(info);
        }
    }
}