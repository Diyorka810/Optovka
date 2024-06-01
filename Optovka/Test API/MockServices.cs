using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_API
{
    public class MockServices
    {
        public static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
            
            return userManagerMock;
        }

        public static Mock<UserManager<ApplicationUser>> CreateUMSetupResultSuccess()
        {
            var userManagerMock = CreateUserManagerMock();

            userManagerMock
                .Setup(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));
            userManagerMock
                .Setup(userManager => userManager.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));

            return userManagerMock;
        }

        public static Mock<UserManager<ApplicationUser>> CreateUMSetupResultFailed()
        {
            var userManagerMock = CreateUserManagerMock();

            var identityErrors = new IdentityError[]
            {
                new IdentityError()
                {
                    Code = "User already exists!",
                    Description = "Username already exists"
                }
            };
            userManagerMock
                .Setup(userManager => userManager.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync((IdentityResult.Failed(identityErrors)));
            userManagerMock
                .Setup(userManager => userManager.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()));

            return userManagerMock;
        }

        public static Mock<RoleManager<IdentityRole>> CreateRoleManagerMock()
        {
            var list = new List<IdentityRole>()
                {
                    new IdentityRole("User"),
                    new IdentityRole("Admin")
                }
                .AsQueryable();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object,
                new IRoleValidator<IdentityRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object);
            roleManagerMock
                .Setup(r => r.Roles).Returns(list);

            return roleManagerMock;
        }
    }
}
