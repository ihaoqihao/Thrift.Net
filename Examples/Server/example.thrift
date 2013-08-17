namespace csharp Example.Service.Thrift
namespace java com.example.service.thrift

/**
 * 非法用户id异常
 */
exception IllegalityUserIdException
{
	1:i32 UserId;				//用户Id
	2:string Reason;			//原由
}

struct User
{
	1:i32 UserId;				//用户Id
	2:string Name;				//名字
}

/**
 * thrift service1
 */
service Service1
{
	/**
	 * sum
	 */
	i32 Sum(1:i32 x, 2:i32 y);
	/**
	 * get user by id.
	 */
	User GetUser(1:i32 userId) throws (1:IllegalityUserIdException ex)
}