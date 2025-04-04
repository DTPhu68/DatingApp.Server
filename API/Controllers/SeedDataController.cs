using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Controllers
{
  public class SeedDataController : BaseApiController
  {
    private readonly DataContext _context;

    public SeedDataController(DataContext context)
    {
      _context = context;
    }

    [HttpPost("batch_insert")]
    public async Task<ActionResult<bool>> BatchInsertUsers()
    {
      if (await _context.Users.AnyAsync())
        return false;
      var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
      var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
      var imageBaseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/resources/images/";
      foreach (var user in users)
      {
        using var hmac = new HMACSHA256();
        user.UserName = user.UserName.ToLower();
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.UserName.ToLower()));
        var new_user = new AppUser
        {
          UserName = user.UserName.ToLower(),
          PasswordHash = Convert.ToBase64String(computedHash),
          PasswordSalt = Convert.ToBase64String(hmac.Key),
          Gender = user.Gender,
          DateOfBirth = user.DateOfBirth,
          KnownAs = user.KnownAs,
          Created = user.Created,
          LastActive = user.LastActive,
          Introduction = user.Introduction,
          LookingFor = user.LookingFor,
          Interests = user.Interests,
          City = user.City,
          Country = user.Country,
          Photos = new List<Photo>
          {
            new Photo
            {
              //UserId=user.Id,
              Url =imageBaseUrl+ user.UserName.ToLower() + ".jpg",
              IsMain = true,
            }
          }

        };
        _context.Users.Add(new_user);
      }
      try
      {
        await _context.SaveChangesAsync();
        return true;
      }
      catch (DbUpdateException dbEx)
      {
        Console.WriteLine(dbEx.ToString());
        throw;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        throw;
      }

    }

    [HttpPost("insert_photos")]
    public async Task<ActionResult<bool>> BatchInsertPhotos()
    {

      var users = await _context.Users.ToListAsync();
      foreach (var user in users)
      {
        var photo = new Photo
        {
          Url = user.UserName.ToLower() + ".jpg",
          IsMain = true,
          UserId = user.Id
        };
        _context.Photos.Add(photo);
      }
      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateException dbEx)
      {
        Console.WriteLine(dbEx.ToString());
        throw;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        throw;
      }
      return true;
    }

  }
}
