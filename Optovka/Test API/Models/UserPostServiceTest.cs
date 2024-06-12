using Lw.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Moq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection.Metadata;

namespace OptovkaTests
{
    [TestClass]
    public class UserPostServiceTest
    {
        public ApplicationDbContext context { get; set; }
        public UserPostModel userPostModel;
        public string AuthorUserId;
        public UserPost userPost;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "test")
            .Options;
            context = new ApplicationDbContext(options);

            userPostModel = new UserPostModel("title", "section", "description", 1);
            AuthorUserId = "1";
            userPost = new UserPost(userPostModel, AuthorUserId);
        }

        [TestMethod]
        public async Task AddAsync_Succeed()
        {
            //Arrange
            var userPostService = new UserPostsService(context);

            //Act
            await userPostService.AddAsync(userPostModel, "1");

            //Assert
            var userPosts = context.UserPosts.ToList();
            Assert.IsNotNull(userPosts);
            var post = userPosts.First();
            Assert.IsNotNull(post);
            Assert.AreEqual(post.Title, userPostModel.Title);
            Assert.AreEqual(post.Section, userPostModel.Section);
            Assert.AreEqual(post.Description, userPostModel.Description);
            Assert.AreEqual(post.RequiredQuantity, userPostModel.RequiredQuantity);
            Assert.AreEqual(post.AuthorUserId, AuthorUserId);

            context.UserPosts.RemoveRange(userPosts);
            context.SaveChanges();
            var newUserPosts = context.UserPosts.ToList();
            Assert.AreEqual(newUserPosts.Count(), 0);
        }

        [TestMethod]
        public async Task TryUpdateAsync_Succeed()
        {
            //Arrange
            var userPostService = new UserPostsService(context);

            //Act
            await userPostService.TryUpdateAsync(userPostModel, userPost);

            //Assert
            var userPosts = context.UserPosts.ToList();
            Assert.IsNotNull(userPosts);
            var post = userPosts.First();
            Assert.IsNotNull(post);
            Assert.AreEqual(post.Title, userPost.Title);
            Assert.AreEqual(post.Section, userPost.Section);
            Assert.AreEqual(post.Description, userPost.Description);
            Assert.AreEqual(post.RequiredQuantity, userPost.RequiredQuantity);
            Assert.AreEqual(post.AuthorUserId, AuthorUserId);

            context.UserPosts.RemoveRange(userPosts);
            context.SaveChanges();
            var newUserPosts = context.UserPosts.ToList();
            Assert.AreEqual(newUserPosts.Count(), 0);
        }

        [TestMethod]
        public async Task TryGetByIdAsync_Succeed()
        {
            //Arrange
            var userPostService = new UserPostsService(context);
            await userPostService.AddAsync(userPostModel, AuthorUserId);
            context.SaveChanges();

            //Act
            var expectedResult = await userPostService.TryGetByIdAsync(1);

            //Assert
            var userPosts = context.UserPosts.ToList();
            Assert.IsNotNull(userPosts);
            var post = userPosts.First();
            Assert.IsNotNull(post);

            Assert.IsNotNull(expectedResult);
            Assert.AreEqual(post.Title, expectedResult.Title);
            Assert.AreEqual(post.Section, expectedResult.Section);
            Assert.AreEqual(post.Description, expectedResult.Description);
            Assert.AreEqual(post.RequiredQuantity, expectedResult.RequiredQuantity);
            Assert.AreEqual(post.AuthorUserId, expectedResult.AuthorUserId);

            context.UserPosts.RemoveRange(userPosts);
            context.SaveChanges();
            var newUserPosts = context.UserPosts.ToList();
            Assert.AreEqual(newUserPosts.Count(), 0);
        }

        [TestMethod]
        public void HasFreeQuantity_True()
        {
            //Arrange
            var userPostService = new UserPostsService(context);
            var desiredQuantity = 1;

            //Act
            var expectedResult = userPostService.HasFreeQuantity(userPost, desiredQuantity);

            //Assert
            Assert.IsTrue(expectedResult);
        }

        [TestMethod]
        public void HasFreeQuantity_False()
        {
            //Arrange
            var userPostService = new UserPostsService(context);
            var desiredQuantity = 2;

            //Act
            var expectedResult = userPostService.HasFreeQuantity(userPost, desiredQuantity);

            //Assert
            Assert.IsFalse(expectedResult);
        }

        [TestMethod]
        public async Task TakePartAsync_Succeed()
        {
            var userPostService = new UserPostsService(context);
            await userPostService.AddAsync(userPostModel, AuthorUserId);
            var userPost = await userPostService.TryGetByIdAsync(1);
            var xy = context.UserPosts.ToList();

            var desiredQuantity = 1;
            var userId = "1";

            //Act
            await userPostService.TakePartAsync(desiredQuantity, userPost, userId);

            //Assert
            var userPosts = context.UserPosts.ToList();
            Assert.IsNotNull(userPosts);
            Assert.AreEqual(userPosts.Count(), 1);
            var post = userPosts.First();
            Assert.IsNotNull(post);
            Assert.AreEqual(post.Title, userPost.Title);
            Assert.AreEqual(post.Section, userPost.Section);
            Assert.AreEqual(post.Description, userPost.Description);
            Assert.AreEqual(post.RequiredQuantity, userPost.RequiredQuantity);
            Assert.AreEqual(post.AuthorUserId, AuthorUserId);
            Assert.AreEqual(post.TakenQuantity, desiredQuantity);
        }
    }
}