using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;

namespace Project_2_Web_Api.Service;

public interface UserServiceAccessor
{

	Task<bool> CheckPermission(string permission);
	Task<bool> IsGuest();
	Task<bool> IsDistributor();
	Task<bool> IsSystem();
	Task<bool> IsSales();
	Task<dynamic> GetByMe();
	Task<Guid?> GetById();
	Task<string?> GetByName(Guid? id);
	string GetByName2(Guid? id);
}
