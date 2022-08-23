using Bogus;
using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;

namespace Gymbokning.Data
{
    public class SeedData
    {
        private static ApplicationDbContext db = default!;
        private static RoleManager<IdentityRole> roleManager = default!;
        private static UserManager<ApplicationUser> userManager = default!;

        public static async Task InitAsync(ApplicationDbContext context, IServiceProvider services, string adminPW)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            db = context;

            ArgumentNullException.ThrowIfNull(nameof(services));
            // if (services is null) throw new ArgumentNullException(nameof(services));

            //if (db.GymClasses.Any()) return;

            roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            ArgumentNullException.ThrowIfNull(roleManager);

            userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            ArgumentNullException.ThrowIfNull(userManager);

            var roleNames = new[] { "Member", "Admin" };
            var adminEmail = "admin@Gymclass.se";

            var gymClasses = GetGymClasses();
            await db.AddRangeAsync(gymClasses);

            await AddRolesAsync(roleNames);

            var admin = await AddAdminAsync(adminEmail, adminPW);

            await AddToRolesAsync(admin, roleNames);

        }

        private static async Task AddToRolesAsync(ApplicationUser admin, string[] roleNames)
        {
            foreach (var role in roleNames)
            {
                if (await userManager.IsInRoleAsync(admin, role)) continue;
                var result = await userManager.AddToRoleAsync(admin, role);
                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static async Task<ApplicationUser> AddAdminAsync(string adminEmail, string adminPW)
        {
            var found = await userManager.FindByEmailAsync(adminEmail);

            if (found != null) return null!;

            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };

            var result = await userManager.CreateAsync(admin, adminPW);
            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));

            return admin;
        }

        private static async Task AddRolesAsync(string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;
                var role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static IEnumerable<GymClass> GetGymClasses()
        {
            var faker = new Faker("sv");

            var gymClasses = new List<GymClass>();

            for (int i = 0; i < 20; i++)
            {
                var temp = new GymClass
                {
                    Name = faker.Company.CatchPhrase(),
                    Description = faker.Hacker.Verb(),
                    Duration = new TimeSpan(0, 55, 0),
                    StartTime = DateTime.Now.AddDays(faker.Random.Int(-5, 5))
                };

                gymClasses.Add(temp);
            }

            return gymClasses;
        }




    }
}
