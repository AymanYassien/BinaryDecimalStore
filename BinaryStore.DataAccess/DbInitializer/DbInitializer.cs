using Binary.Utilities;
using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using BinaryDecimalStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BinaryStore.DataAccess.DbInitializer;

public class DbInitializer : IDbInitializer
{
    public readonly UserManager<ExtendIdentity> _userManager;
    public readonly RoleManager<IdentityRole> _roleManager;
    public readonly BinaryStoreDbContext _db;

    public DbInitializer(
        UserManager<ExtendIdentity> userManager,
        RoleManager<IdentityRole> roleManager,
        BinaryStoreDbContext db
        )
    {
        _userManager = userManager;
        _db = db;
        _roleManager = roleManager;
    }
    
    
    public void initialize()
    {
        addMigration();
        addRoles();
    }

    private void addMigration()
    {
        try
        {
            if (_db.Database.GetPendingMigrations().Count() > 0)
                _db.Database.Migrate();

        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error occurred while applying migrations: {ex.Message}");
            // You can log the full stack trace to a log file or a logging service for debugging
            Console.Error.WriteLine(ex.StackTrace);
        }
    }

    private void addRoles()
    {
        if (! _roleManager.RoleExistsAsync(StaticData.Role_Admin).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole((StaticData.Role_Admin))).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole((StaticData.Role_Company))).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole((StaticData.Role_Customer))).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole((StaticData.Role_Employee))).GetAwaiter().GetResult();

            addAdmin();
            assignRoleAdminToFirstUser();
        }
    }

    private void addAdmin()
    {
        _userManager.CreateAsync(new ExtendIdentity
        {
           UserName = "admin@BinaryStore.com",
           Email = "admin@BinaryStore.com",
           name = "Ayman Mohammed Yassin",
           PhoneNumber = "0559908650",
           address = "Riyadh",
           state = "Riy",
           Code = "112200",
           city = "Riyadh",
        }, "Admin!234").GetAwaiter().GetResult();
    }

    void assignRoleAdminToFirstUser()
    {
        ExtendIdentity user = _db.AppUsers.FirstOrDefault(u => u.Email == "admin@BinaryStore.com");
        _userManager.AddToRoleAsync(user, StaticData.Role_Admin).GetAwaiter().GetResult();
    }
}
