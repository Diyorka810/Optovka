using Lw;
using Lw.Data;
using Lw.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Moq;
using Optovka.Model;
using System.Collections;
using System.Collections.Generic;
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
        public string authorUserId;
        public UserPost userPost;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "test")
            .Options;
            context = new ApplicationDbContext(options);

            userPostModel = new UserPostModel("title", "section", "description", 1);
            authorUserId = "1";
            userPost = new UserPost(userPostModel, authorUserId);
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
            Assert.AreEqual(post.AuthorUserId, authorUserId);

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
            Assert.AreEqual(post.AuthorUserId, authorUserId);

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
        public async Task GetAllAsync_Succeed()
        {
            var userPostService = new UserPostsService(context);
            await userPostService.AddAsync(userPostModel, authorUserId);

            //Act
            var expectedResult = await userPostService.GetAllAsync();

            //Assert
            Assert.IsNotNull(expectedResult);
            Assert.AreEqual(expectedResult.Count(), 1);
            var expectedPost = expectedResult.FirstOrDefault();
            Assert.IsNotNull(expectedPost);
            Assert.AreEqual(expectedPost.Title, userPost.Title);
            Assert.AreEqual(expectedPost.Section, userPost.Section);
            Assert.AreEqual(expectedPost.Description, userPost.Description);
            Assert.AreEqual(expectedPost.RequiredQuantity, userPost.RequiredQuantity);
            Assert.AreEqual(expectedPost.AuthorUserId, authorUserId);

            context.UserPosts.RemoveRange(expectedResult);
            context.SaveChanges();
            var newUserPosts = context.UserPosts.ToList();
            Assert.AreEqual(newUserPosts.Count(), 0);
        }

        [TestMethod]
        public async Task TakePartAsync_Succeed()
        {
            var userPostService = new UserPostsService(context);
            await userPostService.AddAsync(userPostModel, authorUserId);
            var userPost = context.UserPosts.FirstOrDefault();
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
            Assert.AreEqual(post.AuthorUserId, authorUserId);
            Assert.AreEqual(post.TakenQuantity, desiredQuantity);

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
            await userPostService.AddAsync(userPostModel, authorUserId);
            context.SaveChanges();
            var userPost = context.UserPosts.FirstOrDefault();
            var userPostId = userPost.Id;

            //Act
            var expectedResult = await userPostService.TryGetByIdAsync(userPostId);

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
        public async Task TryGetByTitleAsync_Succeed()
        {
            //Arrange
            var userPostService = new UserPostsService(context);
            await userPostService.AddAsync(userPostModel, authorUserId);

            //Act
            var expectedResult = await userPostService.TryGetByTitleAsync("title");

            //Assert
            Assert.IsNotNull(expectedResult);
            Assert.AreEqual(expectedResult.Title, userPost.Title);
            Assert.AreEqual(expectedResult.Section, userPost.Section);
            Assert.AreEqual(expectedResult.Description, userPost.Description);
            Assert.AreEqual(expectedResult.RequiredQuantity, userPost.RequiredQuantity);
            Assert.AreEqual(expectedResult.AuthorUserId, authorUserId);

            context.UserPosts.RemoveRange(expectedResult);
            context.SaveChanges();
            var newUserPosts = context.UserPosts.ToList();
            Assert.AreEqual(newUserPosts.Count(), 0);
        }


        
    }
    public class Solution
    {
        public int RomanToInt(string s)
        {
            var list = ConvertStringToIntList(s);
            var res = 0;
            for (int i = 0; i < list.Count(); i++)
            {
                if (i == 0 && list[i] < list[i + 1])
                {
                    res += list[i + 1] - list[i];
                    continue;
                }

                if (i == list.Count() - 1 && list[i - 1] < list[i])
                    continue;

                if (i < list.Count() - 1 && i != 0)
                {
                    if (list[i] < list[i + 1])
                        res += list[i + 1] - list[i];
                    else if (list[i - 1] < list[i])
                        continue;
                    else
                        res += list[i];
                }
                else
                    res += list[i];

            }
            return res;
        }
        public List<int> ConvertStringToIntList(string s)
        {
            var array = s.ToCharArray();
            var list = new List<int>();
            foreach (var item in array)
            {
                var y = item.ToString();
                switch (y)
                {
                    case "I":
                        list.Add(1);
                        break;
                    case "V":
                        list.Add(5);
                        break;
                    case "X":
                        list.Add(10);
                        break;
                    case "L":
                        list.Add(50);
                        break;
                    case "C":
                        list.Add(100);
                        break;
                    case "D":
                        list.Add(500);
                        break;
                    case "M":
                        list.Add(1000);
                        break;
                }
            }
            return list;
        }
    }
}