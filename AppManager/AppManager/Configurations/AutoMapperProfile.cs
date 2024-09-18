using AutoMapper;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Models;
using Project_2_Web_API.Models;

namespace Project_2_Web_Api.Configurations;

public class AutoMapperProfile : Profile
{

	public AutoMapperProfile()
	{
		CreateMap<GrantPermission, GrantPermissionDTO>();
		CreateMap<GrantPermissionDTO, GrantPermission>();
		CreateMap<Position, PositionDTO>();
		CreateMap<PositionDTO, Position>();
		CreateMap<PositionGroup, PositionGroupDTO>();
		CreateMap<PositionGroupDTO, PositionGroup>();
		CreateMap<Area, AreaDTO>();
		CreateMap<AreaDTO, Area>();
		CreateMap<StaffUser, StaffUserDTO>();
		CreateMap<StaffUserDTO, StaffUser>();
		CreateMap<User, UserDTO>();
		CreateMap<UserDTO, User>();
		CreateMap<DistributorDTO, Distributor>();
		CreateMap<Distributor, DistributorDTO>();
		CreateMap<VisitDTO, Visit>();
		CreateMap<Visit, VisitDTO>();
		CreateMap<TaskForVisitDTO, TaskForVisit>();
		CreateMap<TaskForVisit, TaskForVisitDTO>();
		CreateMap<CommentDTO, Comment>();
		CreateMap<Comment, CommentDTO>();
		CreateMap<PostDTO, Post>();
		CreateMap<Post, PostDTO>();
		CreateMap<NotificationDTO, Notification>();
		CreateMap<Notification, NotificationDTO>();
	}

}
