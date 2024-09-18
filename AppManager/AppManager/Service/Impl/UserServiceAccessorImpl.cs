using AutoMapper;
using Project_2_Web_API.Models;
using System.Diagnostics;
using System.Security;
using System.Security.Claims;

namespace Project_2_Web_Api.Service.Impl;

public class UserServiceAccessorImpl : UserServiceAccessor
{

	private DatabaseContext db;
	private readonly IHttpContextAccessor _httpContextAccessor;
	public UserServiceAccessorImpl(DatabaseContext db, IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
		this.db = db;
	}
	public async Task<bool> CheckPermission(string permission)
	{
		try
		{
			if(_httpContextAccessor.HttpContext != null)
			{
				var id = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
				var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
				if (role.ToLower() == "other department" || role.ToLower() == "guest")
				{
					var database = await db.Users.FindAsync(id);
					foreach(var permissionCheck in database.GrantPermissions)
					{
						if(permissionCheck.Permission.ToLower() == permission.ToLower())
						{
							return true;
						}
					}
					return false;
				}
				else if(role.ToLower() == "distributor - om/tl")
				{
					var database = await db.Distributors.FindAsync(id);
					foreach (var permissionCheck in database.GrantPermissions)
					{
						if (permissionCheck.Permission.ToLower() == permission.ToLower())
						{
							return true;
						}
					}
					return false;
				}
				else if(role.ToLower() == "Administrator".ToLower() || role.ToLower() == "Owner".ToLower())
				{
					return true;
				}
				else
				{
					var database = await db.StaffUsers.FindAsync(id);
					foreach (var permissionCheck in database.GrantPermissions)
					{
						if (permissionCheck.Permission.ToLower() == permission.ToLower())
						{
							return true;
						}
					}
					return false;
				}
			}
			else
			{
				return false;
			}
		}catch(Exception ex)
		{
			Debug.WriteLine(ex.Message); 
			return false;
		}
	}

	public Task<bool> IsGuest()
	{
		try
		{

			
			if (_httpContextAccessor.HttpContext!=null)
			{
				var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
				var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
				if (role.ToLower() == "other department" || role.ToLower() == "guest")
				{
					return Task.FromResult(true);
				}
				else if (role.ToLower() == "distributor - om/tl")
				{
					return Task.FromResult(false);
				}
				else
				{
					return Task.FromResult(false);
				}
			}
			else
			{
				return Task.FromResult(false);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			return Task.FromResult(false);
		}
	}

	public Task<bool> IsDistributor()
	{
		try
		{
			

			if (_httpContextAccessor.HttpContext != null)
			{
				var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
				if (role.ToLower() == "other department" || role.ToLower() == "guest")
				{
					return Task.FromResult(false);
				}
				else if (role.ToLower() == "distributor - om/tl")
				{
					return Task.FromResult(true);
				}
				else
				{
					return Task.FromResult(false);
				}
			}
			else
			{
				return Task.FromResult(false);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			return Task.FromResult(false);
		}
	}

	public Task<bool> IsSystem()
	{
		try
		{
			if (_httpContextAccessor.HttpContext != null)
			{
				var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
				if (role.ToLower() == "other department" || role.ToLower() == "guest")
				{
					return Task.FromResult(false);
				}
				else if (role.ToLower() == "distributor - om/tl")
				{
					return Task.FromResult(false);
				}
				else if(role.ToLower() == "Administrator".ToLower()  || role.ToLower() == "Owner".ToLower())
				{
					return Task.FromResult(true) ;
				}
				else
				{
					return Task.FromResult(false);
				}
			}
			else
			{
				return Task.FromResult(false);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			return Task.FromResult(false);
		}
	}

	public Task<bool> IsSales()
	{
		try{
			if (_httpContextAccessor.HttpContext != null)
			{

				var role = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
				if (role.ToLower() == "other department" || role.ToLower() == "guest")
				{
					return Task.FromResult(false);
				}
				else if (role.ToLower() == "distributor - om/tl")
				{
					return Task.FromResult(false);
				}
				else if (role.ToLower() == "Administrator".ToLower() || role.ToLower() == "Owner".ToLower())
				{
					return Task.FromResult(false);
				}
				else
				{
					return Task.FromResult(true);
				}
			}
			else
			{
				return Task.FromResult(false);
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex.Message);
			return Task.FromResult(false);
		}
	}

	public async Task<dynamic> GetByMe()
	{
		try
		{
			if(_httpContextAccessor.HttpContext != null)
			{
				var idClaim = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
				if (await IsGuest())
				{
					return db.Users.Where(x => x.Id == Guid.Parse(idClaim)).Select(x => new
					{
						id = x.Id,
						fullname = x.FullName,
						positionId = x.PositionId,
						positionName = x.Position.Name,
						area = x.Area == null ? null : new {
							id = x.Area.Id,
							code = x.Area.Code,
							name = x.Area.Name
							},
						createDate = x.CreatedDate
					});
				}else if ( await IsDistributor())
				{
					return db.Distributors.Where(x => x.Id == Guid.Parse(idClaim)).Select(x => new
					{
						id = x.Id,
						name = x.Name,
						positionId = x.PositionId,
						positionName = x.Position.Name,
						area = x.Area == null ? null : new
						{
							id = x.Area.Id,
							code = x.Area.Code,
							name = x.Area.Name
						},
						createDate = x.CreatedDate
					});
				}else if(await IsSales())
				{
					return db.StaffUsers.Where(x => x.Id == Guid.Parse(idClaim)).Select(x => new
					{
						id = x.Id,
						fullname = x.Fullname,
						positionId = x.PositionId,
						positionName = x.Position.Name,
						area = x.Area == null ? null : new
						{
							id = x.Area.Id,
							code = x.Area.Code,
							name = x.Area.Name
						},
						createDate = x.CreatedDate
					});
				}
				else
				{
					return db.StaffUsers.Where(x => x.Id == Guid.Parse(idClaim)).Select(x => new
					{
						id = x.Id,
						fullname = x.Fullname,
						positionId = x.PositionId,
						positionName = x.Position.Name,
						area = x.Area == null ? null : new
						{
							id = x.Area.Id,
							code = x.Area.Code,
							name = x.Area.Name
						},
						createDate = x.CreatedDate
					});
				}
				
			}else {
				return new { error = "Unthorization 401" };
				}
		}catch(Exception ex)
		{
			return new { error = ex.Message };
		}
		
	}

	public Task<Guid?> GetById()
	{
		try
		{
			if (_httpContextAccessor.HttpContext != null)
			{
				return Task.FromResult<Guid?>(Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name)));
			}
			else
			{
				return Task.FromResult<Guid?>(null);
			}
		}
		catch (Exception ex)
		{
			return Task.FromResult<Guid?>(null);
		}
	}
	public async Task<string?> GetByName(Guid? id)
	{
		try
		{
			if (_httpContextAccessor.HttpContext != null)
			{
				var data = await db.StaffUsers.FindAsync(id);
				if(data != null)
				{
					return data.Fullname;
				}
				var data2 = await db.Users.FindAsync(id);
				if (data2 != null)
				{
					return data2.FullName;
				}
				var data3 = await db.Distributors.FindAsync(id);
				if (data3 != null)
				{
					return data3.Name;
				}
				else
				{
					return null;
				}

			}
			else
			{
				return null;
			}
		}
		catch (Exception ex)
		{
			return null;
		}
	}

	public string GetByName2(Guid? id)
	{
		try
		{
			if (_httpContextAccessor.HttpContext != null)
			{
				var data =  db.StaffUsers.Find(id);
				if (data != null)
				{
					return data.Fullname;
				}
				var data2 =  db.Users.Find(id);
				if (data2 != null)
				{
					return data2.FullName;
				}
				var data3 =  db.Distributors.Find(id);
				if (data3 != null)
				{
					return data3.Name;
				}
				else
				{
					return null;
				}

			}
			else
			{
				return null;
			}
		}
		catch (Exception ex)
		{
			return null;
		}
	}

}
