using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
  public static class ApplicationServiceExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
      IConfiguration config)
    {
      services.AddDbContext<DataContext>(options =>
      {
        options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
      });
      services.AddCors();
      services.AddScoped<ITokenService, TokenService>();
      services.AddScoped<IPhotoService, PhotoService>();
      services.AddScoped<IUserRepository, UserRepository>();
      //services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
      return services;
    }
  }

}
