﻿using HxBlogs.Model;
using HxBlogs.IDAL;
namespace HxBlogs.DAL
{
	public partial class BlogDal:BaseDal<Blog>,IBlogDal
	{
	}
	public partial class BlogTagDal:BaseDal<BlogTag>,IBlogTagDal
	{
	}
	public partial class BlogTypeDal:BaseDal<BlogType>,IBlogTypeDal
	{
	}
	public partial class CommentDal:BaseDal<Comment>,ICommentDal
	{
	}
	public partial class UserInfoDal:BaseDal<UserInfo>,IUserInfoDal
	{
	}
}