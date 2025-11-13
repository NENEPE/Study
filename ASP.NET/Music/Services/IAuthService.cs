using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using MusicPortal.Models;
using MusicPortal.Repositories;
using MusicPortal.Services;
using System.Security.Cryptography;
using System.Text;

namespace MusicPortal.Services
{
    public interface IAuthService
    {
        Task<User> LoginCheck(string username, string password);
        Task<bool> RegisterCheck(User user);
        Task<bool> UsernameCheck(string username);
        Task<bool> EmailCheck(string email);
        void HashPassword(string password, out string salt, out string hashedPassword);
        void CookieSet(string key, string value);
        void UserSet(User user);
    }
}

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly HttpContext _httpContext;

    public AuthService(IRepository<User> userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<User> LoginCheck(string username, string password)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u =>
            u.Username == username);

        if (user == null)
            return user;

        byte[] pswrd = Encoding.Unicode.GetBytes(user.Salt + password);

        byte[] byteHash = SHA256.HashData(pswrd);

        StringBuilder hash = new StringBuilder(byteHash.Length);
        for (int i = 0; i < byteHash.Length; i++)
            hash.Append(string.Format("{0:X2}", byteHash[i]));

        if (hash.ToString() != user.Password)
        {
            return null;
        }

        return user;
    }

    public async Task<bool> RegisterCheck(User user)
    {
        try
        {
            await _userRepository.AddAsync(user);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UsernameCheck(string username)
    {
        return await _userRepository.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> EmailCheck(string email)
    {
        return await _userRepository.AnyAsync(u => u.Email == email);
    }

    public void HashPassword(string password, out string salt, out string hashedPassword)
    {
        byte[] saltBuf = new byte[16];
        using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
        {
            randomNumberGenerator.GetBytes(saltBuf);
        }

        StringBuilder sb = new StringBuilder(16);
        for (int i = 0; i < 16; i++)
            sb.Append(string.Format("{0:X2}", saltBuf[i]));
        salt = sb.ToString();

        byte[] saltedPassword = Encoding.Unicode.GetBytes(salt + password);

        byte[] byteHash = SHA256.HashData(saltedPassword);

        StringBuilder hash = new StringBuilder(byteHash.Length);
        for (int i = 0; i < byteHash.Length; i++)
            hash.Append(string.Format("{0:X2}", byteHash[i]));

        hashedPassword = hash.ToString();
    }

    public void CookieSet(string key, string value)
    {
        if (_httpContext.Request.Cookies["login"] != null)
        {
            _httpContext.Response.Cookies.Delete("login");
        }

        CookieOptions option = new CookieOptions();
        option.Expires = DateTime.Now.AddDays(10);
        _httpContext.Response.Cookies.Append(key, value, option);
    }

    public void UserSet(User user)
    {
        _httpContext.Session.SetString("UserId", user.Id.ToString());
        _httpContext.Session.SetString("Username", user.Username);
        _httpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());
    }
}