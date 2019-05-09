﻿using Hx.Common.Logs;
using Hx.Framework;
using HxBlogs.Model;
using HxBlogs.WebApp.Models;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HxBlogs.WebApp.Filters
{
    public class HandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
    {
        public ILogger Logger
        {
            get
            {
                return ContainerManager.Resolve<ILogger>();
            }
        }
        public override void OnException(ExceptionContext context)
        {
            WriteLog(context);
            base.OnException(context);
            context.ExceptionHandled = true;
            AjaxResult result = new AjaxResult { Type = ResultType.Error, Message = context.Exception.Message };
            if (context.Exception is UserFriendlyException)
            {
                result.Errorcode = context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Result = new JsonResult() { Data = result };
            }
            else if (context.Exception is NoAuthorizeException)
            {
                result.Errorcode = context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                if (!context.HttpContext.Request.IsAjaxRequest())
                {
                    context.HttpContext.Response.RedirectToRoute("Default", new { controller = "error", action = "error401", errorUrl = context.HttpContext.Request.RawUrl });
                }
                else
                {
                    context.Result = new JsonResult() { Data = result };
                }
            }
            else
            {
                result.Errorcode = context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                ExceptionMessage error = new ExceptionMessage(context.Exception);
                var s = JsonConvert.SerializeObject(error);
                if (!context.HttpContext.Request.IsAjaxRequest())
                {
                    context.HttpContext.Response.RedirectToRoute("Default", new { controller = "error", action = "error500", data = HttpUtility.UrlEncode(s) });
                }
                else
                {
                    result.Message = error.Message;
                    context.Result = new JsonResult() { Data = result };
                }
            }
        }


        // <summary>
        /// 写入日志（log4net）
        /// </summary>
        /// <param name="context">提供使用</param>
        private void WriteLog(ExceptionContext context)
        {
            if (context == null)
                return;
            if (context.Exception is NoAuthorizeException || context.Exception is UserFriendlyException)
            {
                //友好错误提示,未授权错误提示，记录警告日志
                Logger.Warn(context.Exception.Message, context.Exception);
            }
            else
            {
                //异常错误，
                Logger.Error(context.Exception.Message, context.Exception);
                ////TODO :写入错误日志到数据库
            }
        }
    }
}