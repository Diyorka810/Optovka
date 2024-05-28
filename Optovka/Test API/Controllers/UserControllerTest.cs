using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Optovka.Model;
using Optovka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Configuration;
using System.Runtime.InteropServices;

namespace Test_API
{
    [TestClass]
    public class UserControllerTest
    {
        public Mock<UserManager<ApplicationUser>> userManager;
        public Mock<IConfiguration> configuration;
        public Mock<RoleManager<IdentityRole>> roleManager;
        public UserController userController;
        public ApplicationUser user;
        public RegisterModel registerModel;

        [TestInitialize]
        public void Setup()
        {
            registerModel = new RegisterModel()
            {
                Email = "user@gmail.com",
                UserName = "user",
                PhoneNumber = "+998946899911",
                BirthDate = new DateTime(2000, 1, 1),
                Password = "A_123asd",
            };
            user = new ApplicationUser()
            {
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.UserName,
                PhoneNumber = registerModel.PhoneNumber,
                BirthDate = registerModel.BirthDate
            };

            var mockServices = new MockServices();
            userManager = mockServices.CreateUserManagerMock();
            roleManager = mockServices.CreateMockRoleManager();

            configuration = new Mock<IConfiguration>();
            userController = new UserController(userManager.Object, configuration.Object, roleManager.Object);
        }

        //[TestMethod]
        //public void Get_Return200AndUser()
        //{
        //    //Arrange
        //    //userManager.Setup(x => x.CreateAsync())

        //    //userManager.Setup(x => x.FindByIdAsync("1")).Returns();
        //    //Act

        //    //Assert
        //}

        [TestMethod]
        public async Task Register_CreatedAndReturn201()
        {
            //Arrange
            var message = "{ Status = Success, Message = User created successfully! }";
            var result = userManager.Setup(x => x.CreateAsync(user, registerModel.Password)).Returns(Task.FromResult(It.IsAny<IdentityResult>()));

            //Act
            var expectedResult = await userController.Register(registerModel);

            var actionResult = expectedResult as ObjectResult;
            var expectedValue = actionResult.Value.ToString();
            var expectedStatusCode = actionResult.StatusCode;

            //Assert
            Assert.AreEqual(expectedValue, message);
            Assert.AreEqual(expectedStatusCode, 201);
            userManager.Verify();
        }

        [TestMethod]
        public async Task Register_UserWithThisNameAlreadyExist_StatusCode400()
        {
            //Arrange
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
            var controller = new UserController(userManagerMock.Object, configuration.Object, roleManager.Object);

            var message = "User already exists!";
            //Act
            var expectedResult = await controller.Register(registerModel);

            //Assert

            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as ObjectResult;
            Assert.IsNotNull(actionResult);
            var value = actionResult.Value.ToString();
            Assert.AreEqual(actionResult.StatusCode, 400);
            //Assert.
        }

        [TestMethod]
        public async Task Register_IncorrectEmailAndDate_ValidationErrors()
        {
            //Arrange
            var incorrectDto = new RegisterModel()
            {
                Email = "us#$%^&ergmailcom",
                UserName = "user",
                PhoneNumber = "+998946899911",
                BirthDate = new DateTime(2010, 1, 1),
                Password = "123",
            };
            var message = "{ Status = Error, Message = Errors: \nThe email can only contain letters, numbers and . \r\nEmail should contains one @\r\nYou are too young. Come back in 2 years \r\n }";
            //Act
            var expectedResult = await userController.Register(incorrectDto);
            var actionResult = expectedResult as ObjectResult;
            var value = actionResult.Value.ToString();

            //Assert
            Assert.AreEqual(value, message);
        }
    }
}