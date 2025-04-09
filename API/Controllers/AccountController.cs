using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
  public class AccountController : BaseApiController
  {
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
    {
      _context = context;
      _tokenService = tokenService;
      _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
      var user = await _context.Users
        .Include(p=>p.Photos)
        .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
      if (user == null)
        return Unauthorized("Invalid username");

      var passwordSalt = Convert.FromBase64String(user.PasswordSalt);
      using var hmac = new HMACSHA256(passwordSalt);
      var computedHashByteArray = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
      var computedHash = Convert.ToBase64String(computedHashByteArray);
      if (computedHash != user.PasswordHash)
        return Unauthorized("Invalid password");
      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
        KnownAs=user.KnownAs
      };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
      if (await UserExists(registerDto.Username))
        return BadRequest("Username is taken");
      var user = _mapper.Map<AppUser>(registerDto);

      using var hmac = new HMACSHA256();
      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
      //var user = new AppUser
      //{
      //  UserName = registerDto.Username.ToLower(),
      //  PasswordHash = Convert.ToBase64String(computedHash),
      //  PasswordSalt = Convert.ToBase64String(hmac.Key),
      //};
      user.UserName = registerDto.Username.ToLower();
      user.PasswordHash = Convert.ToBase64String(computedHash);
      user.PasswordSalt = Convert.ToBase64String(hmac.Key);

      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        KnownAs = user.KnownAs
      };
    }

    private async Task<bool> UserExists(string username)
    {
      return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
  }
}
