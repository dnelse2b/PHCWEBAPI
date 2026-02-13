using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SGOFWS.Domains.Models;

namespace SGOFWS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
	private readonly UserManager<IdentityUser> _userManager;

	private readonly RoleManager<IdentityRole> _roleManager;

	private readonly IConfiguration _configuration;

	public AuthenticateController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_configuration = configuration;
	}

	[HttpPost]
	[Route("login")]
	public async Task<IActionResult> Login([FromBody] LoginModel model)
	{
		_ = 2;
		try
		{
			IdentityUser user = await _userManager.FindByNameAsync(model.Username);
			bool flag = user != null;
			bool flag2 = flag;
			if (flag2)
			{
				flag2 = await _userManager.CheckPasswordAsync(user, model.Password);
			}
			if (flag2)
			{
				IList<string> userRoles = await _userManager.GetRolesAsync(user);
				List<Claim> authClaims = new List<Claim>
				{
					new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.UserName),
					new Claim("jti", Guid.NewGuid().ToString())
				};
				foreach (string userRole in userRoles)
				{
					authClaims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", userRole));
				}
				JwtSecurityToken token = GetToken(authClaims);
				return Ok(new
				{
					token = new JwtSecurityTokenHandler().WriteToken(token),
					expiration = token.ValidTo,
					allowed = true,
					output_response = "AUTHENTICATED"
				});
			}
			return Ok(new
			{
				token = "",
				expiration = "",
				allowed = false,
				output_response = "BAD_CREDENTIALS"
			});
		}
		catch (Exception)
		{
			return Ok(new
			{
				token = "",
				expiration = "",
				allowed = false,
				output_response = "INTERNAL_ERROR"
			});
		}
	}

	private JwtSecurityToken GetToken(List<Claim> authClaims)
	{
		SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
		string issuer = _configuration["JWT:ValidIssuer"];
		string audience = _configuration["JWT:ValidAudience"];
		DateTime? expires = DateTime.Now.AddHours(3.0);
		return new JwtSecurityToken(claims: authClaims, signingCredentials: new SigningCredentials(authSigningKey, "HS256"), issuer: issuer, audience: audience, notBefore: null, expires: expires);
	}
}
