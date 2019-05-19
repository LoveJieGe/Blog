﻿using HxBlogs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HxBlogs.IDAL
{
    public partial interface IUserDal : IBaseDal<User>
    {
        User QueryUserByName(string username);
    }
}
