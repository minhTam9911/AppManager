using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_2_Web_Api.DTO;
using Project_2_Web_Api.Service;

namespace Project_2_Web_Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PostController : ControllerBase
{
	private readonly PostService postService;
	private readonly UserServiceAccessor userServiceAccessor;
	public PostController(PostService postService, UserServiceAccessor userServiceAccessor)
	{
		this.postService = postService;
		this.userServiceAccessor = userServiceAccessor;
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPost("create")]
	public async Task<IActionResult> Create([FromBody] PostDTO request)
	{
		try
		{
			return await postService.Create(request);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("multipart/form-data")]
	[HttpPut("upload/{id}")]
	public async Task<IActionResult> Upload(int id,IFormFile filePath)
	{
		try
		{
			return await postService.Upload(id, filePath);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("multipart/form-data")]
	[HttpPut("update/{id}")]
	public async Task<IActionResult> Update(int id,[FromBody] PostDTO request)
	{
		try
		{
			return await postService.Update(id,request);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpPut("update-status/{id}/{status}")]
	public async Task<IActionResult> Update(int id, bool status)
	{
		try
		{
			return await postService.UpdateStatus(id, status);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("search")]
	public async Task<IActionResult> Search([FromQuery(Name = "keyword")] string keyword)
	{
		try
		{
			return await postService.Search(keyword);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpGet("find-all")]
	public async Task<IActionResult> GetAll()
	{
		try
		{
			return await postService.FindAll();
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
			return await postService.Delete(id);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}
	[Produces("application/json")]
	[Consumes("application/json")]
	[HttpDelete("delete-range")]
	public async Task<IActionResult> DeletRange([FromBody] int[] id)
	{
		try
		{
			return await postService.DeleteRange(id);
		}
		catch (Exception ex)
		{
			return BadRequest(new { error = ex.Message });
		}
	}

}
