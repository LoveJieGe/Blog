﻿using HxBlogs.Model;
using HxBlogs.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;

namespace HxBlogs.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>();
            //foreach (var s in assemblies)
            //{
            //    foreach (Type type in s.GetTypes())
            //    {
            //        Console.WriteLine(type.Namespace+"名称："+type.Name);
            //    }
            //}
            //BlogContext c = new BlogContext();
            //c.Blog.Add(new Blog()
            //{
            //    Title="这是一篇测试",
            //    Content="测试"
            //});
            //c.SaveChanges();
            //Console.WriteLine("成功");
            Assembly assembly = Assembly.GetAssembly(typeof(BaseModel));
            //Assembly ass = Assembly.GetExecutingAssembly();
            //Assembly.
            //Type[] types = assembly.GetExportedTypes();
            //foreach (Type type in types)
            //{
            //    if (typeof(BaseModel).IsAssignableFrom(type) && !type.IsAbstract)
            //    {
            //        Console.WriteLine(type.FullName);
            //    }
            //}
            Console.WriteLine(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            Console.ReadLine();
        }
    }
}
