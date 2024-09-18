using Microsoft.EntityFrameworkCore;
using Project_2_Web_Api.Models;
namespace Project_2_Web_API.Models;

public class DatabaseContext : DbContext
{

	public DatabaseContext(DbContextOptions options) : base(options)
	{

	}

	public DbSet<Area> Areas { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<PositionGroup> PositionGroups { get; set; }
	public DbSet<Comment> Comments { set; get; }
	public DbSet<Distributor> Distributors { get; set; }
	public DbSet<GrantPermission> GrantPermissions { get;set; }
	public DbSet<Media> Medias { get; set; }
	public DbSet<Position> Positions {  get; set; }
	public DbSet<Post> Posts { get; set; }
	public DbSet<StaffUser> StaffUsers { get; set; }
	public DbSet<TaskForVisit> TaskForVisit { get; set; }
	public DbSet<Visit> Visits { get; set; }
	public DbSet<ApiToken> ApiTokens { get; set; }
	public DbSet<Notification> Notifications { get; set; }
}
