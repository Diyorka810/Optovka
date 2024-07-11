using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OptovkaTests
{
    [TestClass]
    public class UserControllerTest
    {
        public required Mock<UserManager<ApplicationUser>> userManager;
        public required Mock<IConfiguration> configuration;
        public required Mock<RoleManager<IdentityRole>> roleManager;
        public required UserController userController;
        public required ApplicationUser user;
        public required RegisterDto registerModel;

        [TestInitialize]
        public void Setup()
        {
            registerModel = new RegisterDto()
            {
                Email = "user@gmail.com",
                UserName = "user",
                PhoneNumber = "+998946899911",
                BirthDate = new DateTime(2000, 1, 1),
                Password = "A_123asd",
                CardNumber = 1234567890123456
            };
            user = new ApplicationUser()
            {
                Id = "1",
                Email = registerModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.UserName,
                PhoneNumber = registerModel.PhoneNumber,
                BirthDate = registerModel.BirthDate,
                CardNumber = registerModel.CardNumber,
            };

            userManager = MockServices.CreateUMSetupResultSuccess();
            roleManager = MockServices.CreateRoleManagerMock();

            configuration = new Mock<IConfiguration>();
            userController = new UserController(userManager.Object, configuration.Object, roleManager.Object);
        }

        [TestMethod]
        public async Task Get_StatusCode200AndUser()
        {
            //Arrange
            var createdUser = userManager.Setup(x => x.FindByIdAsync(user.Id))
                                  .Returns(Task.FromResult(user)!);
            var controller = new UserController(userManager.Object, configuration.Object, roleManager.Object);
            //Act
            var expectedResult = await controller.Get(user.Id);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as OkObjectResult;
            Assert.IsNotNull(actionResult);
            var expectedUser = actionResult.Value as UserInfoDto;
            Assert.IsNotNull(expectedUser);
            Assert.AreEqual(expectedUser.UserName, user.UserName);
            Assert.AreEqual(expectedUser.Email, user.Email);
            Assert.AreEqual(expectedUser.PhoneNumber, user.PhoneNumber);
            Assert.AreEqual(expectedUser.BirthDate, user.BirthDate);
            userManager.Verify(x => x.FindByIdAsync(user.Id), Times.Once);
            userManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            userManager.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task Get_UserNotExist_StatusCode404()
        {
            //Arrange
            ApplicationUser? nullRes = null;
            userManager.Setup(x => x.FindByIdAsync(user.Id))
                                  .Returns(Task.FromResult(nullRes));
            var controller = new UserController(userManager.Object, configuration.Object, roleManager.Object);
            var status = "Error";
            var message = "User not found";

            //Act
            var expectedResult = await controller.Get(user.Id);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as ObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 400);
            var expectedValue = actionResult.Value as ApiResponse;
            Assert.IsNotNull(expectedValue);
            Assert.AreEqual(expectedValue.Status, status);
            Assert.AreEqual(expectedValue.Message, message);
            userManager.Verify();
        }

        [TestMethod]
        public async Task Register_CreatedAndStatusCode201()
        {
            //Arrange
            var status = "Success";
            var message = "User created successfully!";

            //Act
            var expectedResult = await userController.Register(registerModel);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as ObjectResult;
            Assert.IsNotNull(actionResult);
            var expectedStatusCode = actionResult.StatusCode;
            Assert.AreEqual(expectedStatusCode, 201);
            var expectedValue = actionResult.Value as ApiResponse;
            Assert.IsNotNull(expectedValue);
            Assert.AreEqual(expectedValue.Status, status);
            Assert.AreEqual(expectedValue.Message, message);
            userManager.Verify();
        }

        [TestMethod]
        public async Task Register_UserNameExist_StatusCode400()
        {
            //Arrange
            var userManager = MockServices.CreateUMSetupResultSuccess();
            userManager.Setup(x => x.FindByNameAsync(user.UserName!))
                                .Returns(Task.FromResult(user)!);
            var controller = new UserController(userManager.Object, configuration.Object, roleManager.Object);
            var status = "Error";
            var message = "User already exists!";

            //Act
            var expectedResult = await controller.Register(registerModel);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as ObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 400);
            var expectedValue = actionResult.Value as ApiResponse;
            Assert.IsNotNull(expectedValue);
            Assert.AreEqual(expectedValue.Status, status);
            Assert.AreEqual(expectedValue.Message, message);
            userManager.Verify();
        }

        [TestMethod]
        public async Task Register_IncorrectEmailDateAndCardNumber_ValidationErrors()
        {
            //Arrange
            var incorrectDto = new RegisterDto()
            {
                Email = "us#$%^&ergmailcom",
                UserName = "user",
                PhoneNumber = "+998946899911",
                BirthDate = new DateTime(2010, 1, 1),
                Password = "123",
                CardNumber = 1,
            };
            var status = "Error";
            var message = "Errors: \nThe email can only contain letters, numbers and . \r\nEmail should contains one @\r\nCard number is incorrect\r\nYou are too young. Come back in 2 years \r\n";
            //Act
            var expectedResult = await userController.Register(incorrectDto);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as ObjectResult;
            Assert.IsNotNull(actionResult);
            var expectedStatusCode = actionResult.StatusCode;
            Assert.AreEqual(expectedStatusCode, 400);
            var expectedValue = actionResult.Value as ApiResponse;
            Assert.IsNotNull(expectedValue);
            Assert.AreEqual(expectedValue.Status, status);
            Assert.AreEqual(expectedValue.Message, message);
            userManager.Verify();
        }

        [TestMethod]
        public async Task Register_CreationFailed_StatusCode400()
        {
            //Arrange
            var userManager= MockServices.CreateUMSetupResultFailed();
            var controller = new UserController(userManager.Object, configuration.Object, roleManager.Object);

            var status = "Error";
            var message = "Username already exists";
            //Act
            var expectedResult = await controller.Register(registerModel);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as ObjectResult;
            Assert.IsNotNull(actionResult);
            var expectedStatusCode = actionResult.StatusCode;
            Assert.AreEqual(expectedStatusCode, 400);
            var expectedValue = actionResult.Value as ApiResponse;
            Assert.IsNotNull(expectedValue);
            Assert.AreEqual(expectedValue.Status, status);
            Assert.AreEqual(expectedValue.Message, message);
            userManager.Verify();
        }

        [TestMethod]
        public async Task Login_NameNotExist_Unauthorized()
        {
            //Arrange
            var userManager= MockServices.CreateUserManagerMock();
            ApplicationUser? nullUser = null;
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(nullUser);
            var controller = new UserController(userManager.Object, configuration.Object, roleManager.Object);

            //Act
            var expectedResult = await controller.Login(user.UserName!, registerModel.Password);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as UnauthorizedResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 401);
            userManager.VerifyAll();
        }

        [TestMethod]
        public async Task Login_PasswordIncorrect_Unauthorized()
        {
            //Arrange
            var userManager = MockServices.CreateUserManagerMock();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(false);
            var controller = new UserController(userManager.Object, configuration.Object, roleManager.Object);

            //Act
            var expectedResult = await controller.Login(user.UserName!, registerModel.Password);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as UnauthorizedResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 401);
            userManager.VerifyAll();
        }

        [TestMethod]
        public async Task Login_JWTToken()
        {
            //Arrange
            var userManager = MockServices.CreateUserManagerMock();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(true);
            var roleList = new List<string>() { "User" };
            userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roleList);
            roleManager.Setup(x => x.FindByNameAsync(roleList.First())).ReturnsAsync(It.IsAny<IdentityRole>());
            var authClaims = new List<Claim>
            {
                new Claim("User", user.Id)
            };
            roleManager.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityRole>())).ReturnsAsync(authClaims);
            var inMemorySettings = new Dictionary<string, string> 
            {
                {"JWT:Secret", "YourSecretKeyHere1233333333333333333333322222222111111111111111111111111111111111111111111123123123"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                        .AddInMemoryCollection(inMemorySettings!)
                        .Build();
            
            var controller = new UserController(userManager.Object, configuration, roleManager.Object);

            //Act
            var expectedResult = await controller.Login(user.UserName!, registerModel.Password);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as OkObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 200);
            var value = actionResult.Value;
            Assert.IsNotNull(value);
            userManager.VerifyAll();
        }
    }
}