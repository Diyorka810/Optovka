using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Optovka.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptovkaTests.Models
{
    [TestClass]
    public class FeedServiceTest
    {
        public ApplicationDbContext context;
        public Mock<IInMemoryCache> cache;
        public UserPost userPost;
        public List<UserPost> list;

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "test")
            .Options;
            context = new ApplicationDbContext(options);
            cache = new Mock<IInMemoryCache>();
            userPost = new UserPost("1", "title", "section", "description", 1);
            list = new List<UserPost>() { userPost };
        }

        [TestMethod]
        public async Task GetAsync_FromCache()
        {
            //Arrange
            cache.Setup(x => x.Contains(It.IsAny<string>())).Returns(true);
            cache.Setup(x => x.Get<IEnumerable<UserPost>>(It.IsAny<string>())).Returns(list);
            var feedService = new FeedService(context, cache.Object);
            var orderType = SortingOrderType.Asc;
            var limit = 50;
            var skip = 2;
            var section = "section";

            //Act
            var expectedResult = await feedService.GetAsync(section, skip, limit, orderType);

            //Assert
            Assert.IsNotNull(expectedResult);
            var expectedUserPost = expectedResult.FirstOrDefault();
            Assert.IsNotNull(expectedUserPost);
            Assert.AreEqual(expectedUserPost.Section, userPost.Section);
            Assert.AreEqual(expectedUserPost.Title, userPost.Title);
            Assert.AreEqual(expectedUserPost.Description, userPost.Description);
            Assert.AreEqual(expectedUserPost.AuthorUser, userPost.AuthorUser);
            Assert.AreEqual(expectedUserPost.RequiredQuantity, userPost.RequiredQuantity);
        }

        [TestMethod]
        public async Task GetAsync_FromDB()
        {
            //Arrange
            cache.Setup(x => x.Contains(It.IsAny<string>())).Returns(false);
            context.UserPosts.Add(userPost);
            context.SaveChanges();
            var feedService = new FeedService(context, cache.Object);
            var orderType = SortingOrderType.Asc;
            var limit = 50;
            var skip = 0;
            var section = "section";

            //Act
            var expectedResult = await feedService.GetAsync(section, skip, limit, orderType);

            //Assert
            Assert.IsNotNull(expectedResult);
            var expectedUserPost = expectedResult.FirstOrDefault();
            Assert.IsNotNull(expectedUserPost);
            Assert.AreEqual(expectedUserPost.Section, userPost.Section);
            Assert.AreEqual(expectedUserPost.Title, userPost.Title);
            Assert.AreEqual(expectedUserPost.Description, userPost.Description);
            Assert.AreEqual(expectedUserPost.AuthorUser, userPost.AuthorUser);
            Assert.AreEqual(expectedUserPost.RequiredQuantity, userPost.RequiredQuantity);
        }
    }
}
