using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Optovka.Model;
using System.Security.Claims;
using System.Security.Principal;

namespace OptovkaTests
{
    [TestClass]
    public class UserPostsControllerTest
    {
        public required Mock<IUserPostsService> userPostsService { get; set; }
        public required Mock<IHttpContextAccessor> contextAccessor { get; set; }
        public required UserPostDto userPostDto { get; set; }
        public required ApplicationUser user { get; set; }
        [TestInitialize]
        public void Setup()
        {
            userPostsService = new Mock<IUserPostsService>();
            contextAccessor = new Mock<IHttpContextAccessor>();
            userPostDto = new UserPostDto() 
            { 
                Title = "title",
                Section = "section",
                Description = "description",
                RequiredQuantity = 1
            };
            user = new ApplicationUser()
            {
                Id = "1",
                Email = "user@gmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "user",
                PhoneNumber = "+998946899911",
                BirthDate = new DateTime(2000, 1, 1)
            };
        }

        //[TestMethod]
        //public void Create_UserIdNull_Return401()
        //{
        //    //Arrange
        //    var controller = new Mock<UserPostsController>(userPostsService.Object);

        //    var controllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { } };

        //    var claim = new Claim("userId", "1");
        //    var claims = new List<Claim>() { claim};
        //    var claimIdentity = new ClaimsIdentity(claims);
        //    controllerContext.HttpContext.User.AddIdentity(claimIdentity);

        //    controller.Setup(c => c.HttpContext).Returns(controllerContext.HttpContext);

        //    //Act
        //    var expectedResult = controller.Object.Create(userPostDto);

        //    //Assert
        //    Assert.IsNotNull(expectedResult);
        //}
        [TestMethod]
        public void Create_UserIdNull_Return401()
        {
            //Arrange
            var identity = new GenericIdentity("userId", "test");
            var contextUser = new ClaimsPrincipal(identity); //add claims as needed

            //...then set user and other required properties on the httpContext as needed
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
            var controller = new UserPostsController(userPostsService.Object);
            controller.ControllerContext.HttpContext = httpContext;
            //controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer TOKEN";
            //Act
            var expectedResult = controller.Create(userPostDto);

            //Assert
            Assert.IsNotNull(expectedResult);
        }
    }
}
