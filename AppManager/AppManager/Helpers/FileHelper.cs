namespace Project_2_Web_Api.Helpers;

public class FileHelper
{
	public static string generateFileName(string fileName)
	{
		var name = Guid.NewGuid().ToString().Replace("-", "");
		var lastIndex = fileName.LastIndexOf('.');
		var extend = fileName.Substring(lastIndex);
		return name + extend;
		//return name+ "." + extend;
	}
	public static bool checkFile(IFormFile file)
	{
		string[] fileExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
		var check = fileExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant());
		var checkSize = file.Length;
		if (check && checkSize <= 50 * 1024 * 1024)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public static bool checkFileMedia(IFormFile file)
	{
		string[] fileExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif",".mp4", ".DivX", ".MPEG-4", ".AVI", ".WMV" };
		var check = fileExtensions.Contains(Path.GetExtension(file.FileName.ToLowerInvariant()).ToLowerInvariant());
		var checkSize = file.Length;
		if (check && checkSize <= 200 * 1024 * 1024)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
