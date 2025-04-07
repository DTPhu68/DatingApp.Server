namespace API.Interfaces
{
  public interface IPhotoService
  {
    Task<string> AddPhotoAsync(IFormFile file);
    bool DeletePhoto(string fileName);
  }
}
