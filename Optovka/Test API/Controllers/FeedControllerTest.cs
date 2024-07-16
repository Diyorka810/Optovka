using Moq;
using Microsoft.AspNetCore.Mvc;

namespace OptovkaTests.Controllers
{
    [TestClass]
    public class FeedControllerTest
    {
        public required Mock<IFeedService> feedService;
        public required UserPost userPost;
        public required FeedController controller;

        [TestInitialize]
        public void Setup()
        {
            feedService = new Mock<IFeedService>();
            userPost = new UserPost("1", "title", "section", "description", 1);
            var list = new List<UserPost>() { userPost };
            feedService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<SortingOrderType>())).ReturnsAsync(list);
            controller = new FeedController(feedService.Object);
        }

        [TestMethod]
        [DataRow(51)]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task Index_InvalidLimit_ErrorMessage(int limit)
        {
            //Arrange
            var status = "Error";
            var message = "From 1 to 50 posts at one request is allowed.";

            var orderType = SortingOrderType.Asc;
            var section = "section";
            var skip = 2;

            //Act
            var expectedResult = await controller.Index(orderType, section, skip, limit);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as ObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 400);
            var value = actionResult.Value as ApiResponseDto;
            Assert.IsNotNull(value);
            Assert.AreEqual(value.Status, status);
            Assert.AreEqual(value.Message, message);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task Index_InvalidSkip_ErrorMessage(int skip)
        {
            //Arrange
            var status = "Error";
            var message = "Skip value must be positive.";

            var orderType = SortingOrderType.Asc;
            var section = "section";
            var limit = 50;

            //Act
            var expectedResult = await controller.Index(orderType, section, skip, limit);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as ObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 400);
            var value = actionResult.Value as ApiResponseDto;
            Assert.IsNotNull(value);
            Assert.AreEqual(value.Status, status);
            Assert.AreEqual(value.Message, message);
        }

        [TestMethod]
        [DataRow(SortingOrderType.Asc, "section", 1)]
        [DataRow(SortingOrderType.Desc, "section", 2)]
        public async Task Index_StatusCode200(SortingOrderType orderType, string section, int skip)
        {
            //Act
            var expectedResult = await controller.Index(orderType, section, skip);

            //Assert
            Assert.IsNotNull(expectedResult);
            var actionResult = expectedResult as OkObjectResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult.StatusCode, 200);
            var userPosts = actionResult.Value as List<UserPost>;
            Assert.IsNotNull(userPosts);
            var expectedUserPost = userPosts.FirstOrDefault();
            Assert.IsNotNull(expectedUserPost);
            Assert.AreEqual(expectedUserPost.Section, userPost.Section);
            Assert.AreEqual(expectedUserPost.Title, userPost.Title);
            Assert.AreEqual(expectedUserPost.Description, userPost.Description);
            Assert.AreEqual(expectedUserPost.AuthorUser, userPost.AuthorUser);
            Assert.AreEqual(expectedUserPost.RequiredQuantity, userPost.RequiredQuantity);
        }
    }
}