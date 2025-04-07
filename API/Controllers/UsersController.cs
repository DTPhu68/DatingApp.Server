using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
  [Authorize]
  public class UsersController : BaseApiController
  {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
    {
      _userRepository = userRepository;
      _mapper = mapper;
      _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
      var users = await _userRepository.GetMembersAsync();
      return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      return await _userRepository.GetMemberAsync(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
      var username = User.GetUsername();
      var user = await _userRepository.GetUserByUsernameAsync(username);
      if (user == null) return NotFound();
      _mapper.Map(memberUpdateDto, user);
      _userRepository.Update(user);
      if (await _userRepository.SaveAllAsync()) return NoContent();
      return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
      var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
      if (user == null) return NotFound();

      var fileName = await _photoService.AddPhotoAsync(file);
      if (string.IsNullOrEmpty(fileName)) return BadRequest("Could not save photo to server");
      var imageUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/resources/images/"
        + fileName;
      var photo = new Photo
      {
        Url = imageUrl,
        UserId = user.Id,
        IsMain = user.Photos.Count == 0 ? true : false,
      };
      user.Photos.Add(photo);
      if (await _userRepository.SaveAllAsync())
      {
        //return _mapper.Map<PhotoDto>(photo);
        //return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
        return CreatedAtAction(nameof(GetUser),
          new { username = user.UserName },
          _mapper.Map<PhotoDto>(photo));
      }
      return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
      var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

      if (user == null) return NotFound();

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if (photo == null) return NotFound();

      if (photo.IsMain) return BadRequest("This is already your main photo");

      var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
      if (currentMain != null) currentMain.IsMain = false;
      photo.IsMain = true;

      if (await _userRepository.SaveAllAsync()) return NoContent();

      return BadRequest("Problem setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
      var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
      if (photo == null) return NotFound();
      if (photo.IsMain) return BadRequest("You cannot delete your main photo");
      var fileName = getFileNameFromUrl(photo.Url);
      if (fileName != null)
      {
        var result = _photoService.DeletePhoto(fileName);
        if (!result) return BadRequest("Failed to delete photo " + photo.Url);
      }
      user.Photos.Remove(photo);
      if (await _userRepository.SaveAllAsync()) return Ok();
      return BadRequest("Failed to delete photo");
    }

    private string getFileNameFromUrl(string url)
    {
      Uri uri = new(url);
      return System.IO.Path.GetFileName(uri.LocalPath);
    }
  }
}
