using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Docker.Example.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public DataSeeder(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SeedData()
    {
        await _context.Database.MigrateAsync();
        await _context.Database.EnsureCreatedAsync();

        await AddUser();
    }

    private async Task AddUser()
    {
        var result = await _userManager.CreateAsync(new IdentityUser
        {
            UserName = "indyisgreat@gmail.com",
            Email = "indyisgreat@gmail.com"
        }, "HotChocolate123!");

        if (!result.Succeeded)
        {
            throw new Exception();
        }
    }
}