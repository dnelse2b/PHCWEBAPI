using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SGOFWS.Persistence.Contexts;

public class AuthAppContext : IdentityDbContext<IdentityUser>
{
	public AuthAppContext(DbContextOptions<AuthAppContext> options)
		: base((DbContextOptions)options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
	}
}
