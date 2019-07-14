﻿using Hx.Framework;
using HxBlogs.IBLL;
using HxBlogs.Model;
using HxBlogs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace HxBlogs.WebApp.Areas.Admin.Controllers
{
    /// <summary>
    /// 用户中心
    /// </summary>
    public class UserCenterController : Controller
    {
        private IUserService _userService;
        public UserCenterController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Admin/UserCenter
        public ActionResult Profiles()
        {
            Models.BasicInfoDTO basicInfo = MapperManager.Map<Models.BasicInfoDTO>(UserContext.LoginUser.BasicInfo);
            basicInfo.NickName = UserContext.LoginUser.NickName;
            Models.JobInfoDTO jobInfo = MapperManager.Map<Models.JobInfoDTO>(UserContext.LoginUser.JobInfo);
            ViewBag.JobInfo = jobInfo;
            ViewBag.AvatarUrl = UserContext.LoginUser.AvatarUrl;
            return View(basicInfo);
        }

        public ActionResult SaveBasicInfo(Models.BasicInfoDTO infoDTO)
        {
            AjaxResult result = new AjaxResult();
            if (ModelState.IsValid)
            {
                TransactionManager.Excute(delegate{
                    IBasicInfoService basicService = ContainerManager.Resolve<IBasicInfoService>();
                    UserInfo logUser = UserContext.LoginUser;
                    HxBlogs.Model.UserInfo user = MapperManager.Map<UserInfo>(logUser);
                    BasicInfo basicInfo = MapperManager.Map<BasicInfo>(infoDTO);
                    basicInfo.Id = user.BasicId;
                    user.BasicInfo = basicInfo;
                    user.NickName = infoDTO.NickName;
                    List<string> fields = new List<string>();
                    fields.AddRange(new string[] { "RealName", "QQ", "WeChat", "Mobile", "Birthday", "Address", "Description" });
                    if (string.IsNullOrEmpty(logUser.BasicInfo.Gender))
                    {
                        fields.Add("Gender");
                    }
                    _userService.UpdateEntityFields(user, "NickName");
                    basicService.UpdateEntityFields(basicInfo, fields);
                    UserContext.UpdateUser(user);
                });
            }
            else
            {
                result = GetErrorResult();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveJobInfo(Models.JobInfoDTO infoDTO)
        {
            AjaxResult result = new AjaxResult();
            if (ModelState.IsValid)
            {
                TransactionManager.Excute(delegate {
                    IJobInfoService jobService = ContainerManager.Resolve<IJobInfoService>();
                    UserInfo logUser = UserContext.LoginUser;
                    HxBlogs.Model.UserInfo user = MapperManager.Map<UserInfo>(logUser);
                    JobInfo jobInfo = MapperManager.Map<JobInfo>(infoDTO);
                    jobInfo.Id = user.JobId;
                    user.JobInfo = jobInfo;
                    List<string> fields = new List<string>();
                    fields.AddRange(new string[] { "Position", "Industry", "WorkUnit", "WorkYear", "Status", "Skills", "GoodAreas" });
                    jobService.UpdateEntityFields(jobInfo, fields);
                    _userService.UpdateEntityFields(user);
                    UserContext.UpdateUser(user);
                });
            }
            else
            {
                result = GetErrorResult();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private AjaxResult GetErrorResult()
        {
            AjaxResult result = new AjaxResult
            {
                Success = false
            };
            foreach (var key in ModelState.Keys)
            {
                var modelstate = ModelState[key];
                if (modelstate.Errors.Any())
                {
                    result.Message = modelstate.Errors.FirstOrDefault().ErrorMessage;
                    break;
                }
            }
            return result;
        }
    }
}