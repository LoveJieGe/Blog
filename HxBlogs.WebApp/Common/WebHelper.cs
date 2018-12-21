﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace HxBlogs.WebApp
{
    public class WebHelper
    {
        /// <summary>
        /// 根据时间获取要显示的日期格式形式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <param name="showTime"></param>
        /// <returns></returns>
        public static string GetDispayDate(DateTime dateTime,string format = "M", bool showTime = false)
        {
            string result = string.Empty;
            if (dateTime == null) return result;
            DateTime nowTime = DateTime.Now;
            TimeSpan ts = nowTime.Subtract(dateTime);
            if (ts.Days <= 1)
            {
                result = ts.Hours+"小时前";
            }
            else if (ts.Days <= 2)
            {
                result = "1天前";
            }
            else if (ts.Days <= 3)
            {
                result = "2天前";
            }
            else if (ts.Days <= 4)
            {
                result = "3天前";
            }
            else if (ts.Days <= 5)
            {
                result = "4天前";
            }
            else if (ts.Days <= 6)
            {
                result = "5天前";
            }
            else if (ts.Days <= 7)
            {
                result = "6天前";
            }
            else if (ts.Days <= 8)
            {
                result = "7天前";
            }
            else
            {
                result = dateTime.ToString(format);
            }
            if (showTime)
            {
                result += dateTime.ToString("t");
            }
            return result;
        }
        public static string GetDispayDate(DateTime? dateTime, string format = "M", bool showTime = false)
        {
            string result = string.Empty;
            if (dateTime == null || !dateTime.HasValue) return result;
            return GetDispayDate(dateTime.Value, format, showTime);
        }
        /// <summary>
        /// 设置cookie的值
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="value"></param>
        /// <param name="expires">过期时间</param>
        public static void SetCookieValue(string cookieName, string value, DateTime? expires = null)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Value = value;
                if (expires.HasValue)
                {
                    cookie.Expires = expires.Value;
                }
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            else
            {
                cookie = new HttpCookie(cookieName,value);
                if (expires.HasValue)
                {
                    cookie.Expires = expires.Value;
                }
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        /// <summary>
        /// 获取cookie的值
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static string GetCookieValue(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
                return string.Empty;
            else
                return cookie.Value;
        }
        /// <summary>
        /// 删除cookie的值
        /// </summary>
        /// <param name="cookieName"></param>
        public static void RemoveCookie(string cookieName)
        {
            SetCookieValue(cookieName, "", DateTime.Now.AddDays(-1));
        }
        /// <summary>
        /// 根据图片的全路径判断是否是图片文件,
        /// 如果根据后缀名判断是图片，在判断是否能转换成图片对象，能转换则是图片
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool IsImage(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath) && !Path.HasExtension(fullPath)) return false;
            Image img = null;
            try
            {
                string ext = Path.GetExtension(fullPath).ToLower();
                if (!IsImage(ext, true)) return false;
                img = Image.FromFile(fullPath);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                if (img != null)
                    img.Dispose();
            }
        }
        /// <summary>
        /// 根据扩展名判断是否是图片
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool IsImage(string ext,bool hasPoint)
        {
            string[] extList = new string[] { ".gif", ".jpg", ".jpeg", ".png", ".bmp", ".icon" };
            bool result = false;
            if (hasPoint)
            {
                if (extList.Contains(ext)) result = true;
            }
            else
            {
                if (extList.Contains("."+ext)) result = true;
            }
            
            return result;
        }
        /// <summary>
        /// 判断当前流是否是图片
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool IsImage(Stream stream)
        {
            Image img = null;
            try
            {
                img = Image.FromStream(stream);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                if (img != null)
                    img.Dispose();
            }
        }
        /// <summary>
        /// 把绝对路径转换成相对路径
        /// </summary>
        /// <param name="imagesurl1"></param>
        /// <returns></returns>
        public static string ToRelativePath(string imagesurl1)
        {
            string tmpRootDir = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string imagesurl2 = imagesurl1.Replace(tmpRootDir, "/"); //转换成相对路径
            imagesurl2 = imagesurl2.Replace(@"/", @"/");
            return imagesurl2;
        }
    }
}