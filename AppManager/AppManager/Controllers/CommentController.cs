using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CommentController : ControllerBase
{
	private readonly CommentService commentService;
	private readonly UserServiceAccessor userServiceAccessor;
	public CommentController(CommentService commentService, UserServiceAccessor userServiceAccessor)
	{
		this.commentService = commentService;
		this.userServiceAccessor = userServiceAccessor;
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all/{id}")]
	public async Task<IActionResult> GetAll(int id)
	{
		try
		{
			return await commentService.FindAll(id);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			return await commentService.Delete(id);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromBody]CommentDTO request)
	{
		try
		{
			return await commentService.Create(request);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("reply")]
	public async Task<IActionResult> Reply([FromBody] CommentDTO request)
	{

		try
		{
			return await commentService.Reply(request);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}
}
