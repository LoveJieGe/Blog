﻿using HxBlogs.Model;
namespace HxBlogs.IDAL
{
	public partial interface IBlogDal:IBaseDal<Blog>
	{

	}
	public partial interface IBlogTagDal:IBaseDal<BlogTag>
	{

	}
	public partial interface IBlogTypeDal:IBaseDal<BlogType>
	{

	}
	public partial interface ICommentDal:IBaseDal<Comment>
	{

	}
	public partial interface IUserInfoDal:IBaseDal<UserInfo>
	{

	}
}