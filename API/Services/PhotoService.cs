using API.Interfaces;
using System.Net.Http.Headers;

namespace API.Services
{
  public class PhotoService : IPhotoService
  {
    public async Task<string> AddPhotoAsync(IFormFile file)
    {
      var folderName = Path.Combine("Resources", "Images");
      var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
      if (file.Length > 0)
      {
        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        var fullPath = Path.Combine(pathToSave, fileName);
        var dbPath = Path.Combine(folderName, fileName);
        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);
        //return dbPath;
        return fileName;
      }
      else
      {
        return string.Empty;
      }
    }

    public bool DeletePhoto(string fileName)
    {
      var folderName = Path.Combine("Resources", "Images");
      var pathToDelete = Path.Combine(Directory.GetCurrentDirectory(), folderName);
      var imagePath = Path.Combine(pathToDelete, fileName);
      if (File.Exists(imagePath))
        File.Delete(imagePath);
      return true;
    }

  }
}
