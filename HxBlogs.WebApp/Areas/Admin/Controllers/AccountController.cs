﻿using Common.Email;
using Common.Helper;
using Common.Logs;
using Common.Mapper;
using Common.Cache;
using Common.Web;
using HxBlogs.Framework;
using HxBlogs.IBLL;
using HxBlogs.Model;
using HxBlogs.WebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HxBlogs.WebApp.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private IUserInfoService _userService;
        public ILogger Logger
        {
            get {
                return ContainerManager.Resolve<ILogger>();
            }
        }
        public AccountController(IUserInfoService userService)
        {
            this._userService = userService;
        }
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        // GET: Admin/Account
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 注册页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }
        #region 用户登录注册
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> RegisterUser(RegisterViewModel info)
        {
            ReturnResult result = new ReturnResult();
            UserInfo userInfo = null;
            if (ModelState.IsValid)
            {
                userInfo = MapperHelper.Map<UserInfo>(info);
                userInfo = this._userService.Insert(userInfo,out result);
            }
            if (result.IsSuccess)
            {
                await SendEmail(info.UserName, info.Email);
                result.Message = "已发送激活链接到邮箱，请尽快激活。";
                //成功以后直接到主页，即在登录状态
                string sessionId = Guid.NewGuid().ToString();
                Response.Cookies[CookieInfo.SessionID].Value = sessionId.ToString();
                string jsonData = JsonConvert.SerializeObject(userInfo);
                MemcachedHelper.Set(sessionId, jsonData, DateTime.Now.AddMinutes(20));
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 发送邮件，激活账号
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="toEmail"></param>
        /// <returns></returns>
        public async Task<bool> SendEmail(string userName, string toEmail)
        {
            Guid key = Guid.NewGuid();
            MemcachedHelper.Set(key.ToString(), userName, DateTime.Now.AddMinutes(30));
            var checkUrl = Request.Url.Scheme + "://" + Request.Url.Host + ":" +
                Request.Url.Port + "/Admin/Account/Activation?key=" + key;
            await Task.Run(() =>
            {
                EmailHelper email = new EmailHelper()
                {
                    MailPwd = "tao58568470jie",
                    MailFrom = "stjworkemail@163.com",
                    MailSubject = "欢迎您注册 海星·博客",
                    MailBody = EmailHelper.TempBody(userName, "请复制打开链接(或者右键新标签中打开)，激活账号",
                      "<a style='word-wrap: break-word;word-break: break-all;' href='" + checkUrl + "'>" + checkUrl + "</a>"),
                    MailToArray = new string[] { toEmail }
                };
                email.SendAsync((s, ex) => {
                    if (!s && ex != null)
                    {
                        Logger.Error("邮箱发送失败", ex);
                    }
                });
            });
            return true;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login()
        {
            string validateCode = Request["ValidateCode"];
            string code = Session[CookieInfo.VCode].ToString();
            ReturnResult result = new ReturnResult();
            if (!Helper.AreEqual(code, validateCode))
            {
                result.IsSuccess = false;
                result.Message = "验证码不正确";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            string userName = Request["UserName"];
            string pwd = Common.Security.SafeHelper.MD5TwoEncrypt(Request["PassWord"]);
            UserInfo userInfo = this._userService.QueryEntity(u => (u.UserName == userName || u.Email == userName) && u.PassWord == pwd);
            if (userInfo == null)
            {
                result.IsSuccess = false;
                result.Message = "用户名或密码错误!";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            result.IsSuccess = true;
            string returnUrl = Request["ReturnUrl"];
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "/";
            returnUrl = returnUrl.Trim();
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }
            result.ReturnUrl = returnUrl;
            string sessionId = Guid.NewGuid().ToString();
            Response.Cookies[CookieInfo.SessionID].Value = sessionId.ToString();
            string jsonData = JsonConvert.SerializeObject(userInfo);
            MemcachedHelper.Set(sessionId, jsonData, DateTime.Now.AddMinutes(20));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 显示验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowValidateCode()
        {
            ValidateCode validate = new ValidateCode()
            {
                MultValue = 2,
                NPhase = Math.PI
            };
            string code = validate.GetRandomNumberString(4);
            Session[CookieInfo.VCode] = code;
            byte[] bytes = validate.CreateValidateGraphic(code);
            return File(bytes, "image/jpeg");
        }

        public ActionResult Logout()
        {
            if (Request.Cookies[CookieInfo.SessionID] != null)
            {
                string sessionId = Request.Cookies[CookieInfo.SessionID].Value;
                MemcachedHelper.Delete(sessionId);
            }
            return Redirect("/login");
        }
        #endregion
        #region 验证用户输入的信息
        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckUserName(string userName)
        {
            bool result = _userService.Exist(userName);
            if (result)
            {
                return Json(string.Format("用户名{0}已存在!", userName));
            }
            return Json(true);
        }
        /// <summary>
        /// 检查邮箱是否被注册了
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckEmail(string email)
        {
            bool result = _userService.Exist(u => u.Email == email);
            if (result)
            {
                return Json("此邮箱已被注册!");
            }
            return Json(true);
        }
        /// <summary>
        /// 检查验证码是否正确
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckCode(string validateCode)
        {
            string code = Session[CookieInfo.VCode].ToString();
            // ReturnResult result = new ReturnResult();
            if (Helper.AreEqual(code, validateCode))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 激活用户
        [HttpGet]
        public ActionResult Activation()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ActiveUser(string key)
        {
            ReturnResult result = new ReturnResult();
            if (MemcachedHelper.Get(key) != null)
            {
                string userName = MemcachedHelper.Get(key).ToString();
                UserInfo userInfo = this._userService.QueryEntity(u => u.UserName == userName);
                if (userInfo == null)
                {
                    result.IsSuccess = false;
                    result.Message = "此用户没有注册!";
                }
                else
                {
                    userInfo.IsActivate = "Y";
                    this._userService.Update(userInfo);
                    result.IsSuccess = true;
                }
                MemcachedHelper.Delete(key);
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "此激活链接已失效!";
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}