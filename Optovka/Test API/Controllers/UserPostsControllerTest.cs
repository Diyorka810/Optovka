using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace OptovkaTests
{
    [TestClass]
    public class UserPostsControllerTest
    {
        public required Mock<IUserPostsService> userPostsService;
        public required Mock<IHttpContextAccessor> contextAccessor;
        public required UserPostDto userPostDto;
        public required UserPost userPost;
        public required ApplicationUser user;
        public required DefaultHttpContext httpContext;
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
            userPost = new UserPost(userPostDto.ToUserPostModel(), "1");
            user = new ApplicationUser()
            {
                Id = "1",
                Email = "user@gmail.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "user",
                PhoneNumber = "+998946899911",
                BirthDate = new DateTime(2000, 1, 1)
            };

            var contextUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new("userId", "1")
            }, "TestAuthentication"));

            httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
        }

        [TestMethod]
        public void Create_Created201()
        {
            //Arrange
            var controller = new UserPostsController(userPostsService.Object);
            controller.ControllerContext.HttpContext = httpContext;

            //Act
            var expectedResult = controller.Create(userPostDto);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult.Result as StatusCodeResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 201);
        }

        [TestMethod]
        public void Edit_CantFindPost_StatusCode404()
        {
            //Arrange 
            UserPost? userPost = null;
            userPostsService.Setup(x => x.TryGetByIdAsync(It.IsAny<int>())).ReturnsAsync(userPost);
            var controller = new UserPostsController(userPostsService.Object);
            controller.ControllerContext.HttpContext = httpContext;

            //Act
            var expectedResult = controller.Edit(userPostDto, 1);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult.Result as StatusCodeResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 404);
            userPostsService.Verify();
        }

        [TestMethod]
        public void Edit_AuthorUserPostIdIsNotEqual_StatusCode401()
        {
            //Arrange 
            var userPost = new UserPost(userPostDto.ToUserPostModel(), "asdf");
            userPostsService.Setup(x => x.TryGetByIdAsync(It.IsAny<int>())).ReturnsAsync(userPost);
            var controller = new UserPostsController(userPostsService.Object);
            controller.ControllerContext.HttpContext = httpContext;
            var status = "Error";
            var message = "You can change only your posts";

            //Act
            var expectedResult = controller.Edit(userPostDto, 1);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult.Result as ObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 401);
            var resultValue = actionResult.Value as ApiResponseDto;
            Assert.IsNotNull(resultValue);
            Assert.AreEqual(resultValue.Status, status);
            Assert.AreEqual(resultValue.Message, message);
            userPostsService.Verify();
        }

        [TestMethod]
        public void Edit_StatusCode201()
        {
            //Arrange 
            userPostsService.Setup(x => x.TryGetByIdAsync(It.IsAny<int>())).ReturnsAsync(userPost);
            var controller = new UserPostsController(userPostsService.Object);
            controller.ControllerContext.HttpContext = httpContext;

            //Act
            var expectedResult = controller.Edit(userPostDto, 1);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult.Result as StatusCodeResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 201);
            userPostsService.Verify();
        }

        [TestMethod]
        public void TakePart_CantFindPost_StatusCode404()
        {
            //Arrange
            UserPost? userPost = null;
            userPostsService.Setup(x => x.TryGetByIdAsync(It.IsAny<int>())).ReturnsAsync(userPost);
            var controller = new UserPostsController(userPostsService.Object);
            controller.ControllerContext.HttpContext = httpContext;

            //Act
            var expectedResult = controller.TakePart(1, 1);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult.Result as StatusCodeResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 404);
            userPostsService.Verify();
        }

        [TestMethod]
        public void TakePart_HasNotFreeQuantity_StatusCode404()
        {
            //Arrange
            userPostsService.Setup(x => x.TryGetByIdAsync(It.IsAny<int>())).ReturnsAsync(userPost);
            userPostsService.Setup(x => x.HasFreeQuantity(userPost, It.IsAny<int>())).Returns(false);
            var controller = new UserPostsController(userPostsService.Object);
            controller.ControllerContext.HttpContext = httpContext;
            var status = "Error";
            var message = "You are trying to order more than the available quantity";

            //Act
            var expectedResult = controller.TakePart(1, 1);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult.Result as ObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 400);
            var resultValue = actionResult.Value as ApiResponseDto;
            Assert.IsNotNull(resultValue);
            Assert.AreEqual(resultValue.Status, status);
            Assert.AreEqual(resultValue.Message, message);
            userPostsService.Verify();
        }

        [TestMethod]
        public void TakePart_StatusCode205()
        {
            //Arrange
            userPostsService.Setup(x => x.TryGetByIdAsync(It.IsAny<int>())).ReturnsAsync(userPost);
            userPostsService.Setup(x => x.HasFreeQuantity(userPost, It.IsAny<int>())).Returns(true);
            var controller = new UserPostsController(userPostsService.Object);
            controller.ControllerContext.HttpContext = httpContext;

            //Act
            var expectedResult = controller.TakePart(1, 1);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult.Result as StatusCodeResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 205);
            userPostsService.Verify();
        }

        [TestMethod]
        public void FindByTitle_StatusCode200AndUserPost()
        {
            //Arrange
            userPostsService.Setup(x => x.TryGetByTitleAsync(It.IsAny<string>())).ReturnsAsync(userPost);
            var controller = new UserPostsController(userPostsService.Object);
            controller.ControllerContext.HttpContext = httpContext;

            //Act
            var expectedResult = controller.FindByTitle(It.IsAny<string>());

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult.Result as OkObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 200);
            var post = actionResult.Value as UserPost;
            Assert.IsNotNull(post);
            Assert.AreEqual(userPost, post);
            Assert.AreEqual(userPost.Id, post.Id);
            Assert.AreEqual(userPost.AuthorUser, post.AuthorUser);
            Assert.AreEqual(userPost.Description, post.Description);
            Assert.AreEqual(userPost.Section, post.Section);
            Assert.AreEqual(userPost.Title, post.Title);
            Assert.AreEqual(userPost.RequiredQuantity, post.RequiredQuantity);
            Assert.AreEqual(userPost.TakenQuantity, post.TakenQuantity);
            userPostsService.Verify();
        }

        [TestMethod]
        public void GetAll_StatusCode200AndUserPosts()
        {
            //Arrange
            var userPosts = new List<UserPost>() { userPost };
            userPostsService.Setup(x => x.GetAllAsync()).ReturnsAsync(userPosts);
            var controller = new UserPostsController(userPostsService.Object);
            controller.ControllerContext.HttpContext = httpContext;

            //Act
            var expectedResult = controller.GetAll();

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult.Result as OkObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 200);
            var posts = actionResult.Value as List<UserPost>;
            Assert.IsNotNull(posts);
            var post = posts.FirstOrDefault();
            Assert.IsNotNull(post);
            Assert.AreEqual(userPost, post);
            Assert.AreEqual(userPost.Id, post.Id);
            Assert.AreEqual(userPost.AuthorUser, post.AuthorUser);
            Assert.AreEqual(userPost.Description, post.Description);
            Assert.AreEqual(userPost.Section, post.Section);
            Assert.AreEqual(userPost.Title, post.Title);
            Assert.AreEqual(userPost.RequiredQuantity, post.RequiredQuantity);
            Assert.AreEqual(userPost.TakenQuantity, post.TakenQuantity);
            userPostsService.Verify();
        }
    }
}