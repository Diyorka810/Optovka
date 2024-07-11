using System.Text;

namespace OptovkaTests.Models
{
    [TestClass]
    public class ApplicationUserTest
    {
        public ApplicationUser user;
        public StringBuilder errorMessage;
        [TestInitialize]
        public void Setup()
        {
            user = new ApplicationUser()
            {
                Id = "1",
                Email = "user@gmail.com",
                UserName = "user",
                PhoneNumber = "+998946899911",
                BirthDate = new DateTime(2000, 1, 1),
                CardNumber = 1234567890123456
            };
            errorMessage = new StringBuilder("Errors: \n");
        }

        [TestMethod]
        public void IsValid_EmailIsNull_ErrorMessage()
        {
            //Arrange
            string email = null;
            user.Email = email;
            var isValid = false;
            errorMessage.AppendLine("The email can not be null");

            //Act
            var expectedResult = user.IsValid();

            //Assert
            Assert.IsNotNull(expectedResult);
            var expectedValue = expectedResult.Item1;
            var expectedMessage = expectedResult.Item2.ToString();
            Assert.AreEqual(expectedValue, isValid);
            Assert.AreEqual(expectedMessage, errorMessage.ToString());
        }

        [TestMethod]
        public void IsValid_EmailContainsSymbols_ErrorMessage()
        {
            //Arrange
            string email = "!#$%^&*()-=_+,?/|\\`:;\'\"{}[]@.com";
            user.Email = email;
            var isValid = false;
            errorMessage.AppendLine("The email can only contain letters, numbers and . ");

            //Act
            var expectedResult = user.IsValid();

            //Assert
            Assert.IsNotNull(expectedResult);
            var expectedValue = expectedResult.Item1;
            var expectedMessage = expectedResult.Item2.ToString();
            Assert.AreEqual(expectedValue, isValid);
            Assert.AreEqual(expectedMessage, errorMessage.ToString());
        }

        [TestMethod]
        [DataRow("asdfasdf.asdf")]
        [DataRow("asdf@asdf@asdf.asdf")]
        public void IsValid_IncorrectNumOfAt_ErrorMessage(string value)
        {
            //Arrange
            user.Email = value;
            var isValid = false;
            errorMessage.AppendLine("Email should contains one @");

            //Act
            var expectedResult = user.IsValid();

            //Assert
            Assert.IsNotNull(expectedResult);
            var expectedValue = expectedResult.Item1;
            var expectedMessage = expectedResult.Item2.ToString();
            Assert.AreEqual(expectedValue, isValid);
            Assert.AreEqual(expectedMessage, errorMessage.ToString());
        }

        [TestMethod]
        [DataRow("asdf.asdf@asdf")]
        [DataRow("asdf@asdf")]
        public void IsValid_MissingPoint_ErrorMessage(string value)
        {
            //Arrange
            user.Email = value;
            var isValid = false;
            errorMessage.AppendLine("Email should contains 1 . after @");

            //Act
            var expectedResult = user.IsValid();

            //Assert
            Assert.IsNotNull(expectedResult);
            var expectedValue = expectedResult.Item1;
            var expectedMessage = expectedResult.Item2.ToString();
            Assert.AreEqual(expectedValue, isValid);
            Assert.AreEqual(expectedMessage, errorMessage.ToString());
        }

        [TestMethod]
        [DataRow(12345678901234567)]
        [DataRow(1)]
        public void IsValid_CardNumberIsIncorrect(long value)
        {
            //Arrange
            user.CardNumber = value;
            var isValid = false;
            errorMessage.AppendLine("Card number is incorrect");

            //Act
            var expectedResult= user.IsValid();

            //Assert
            Assert.IsNotNull(expectedResult);
            var expectedValue = expectedResult.Item1;
            var expectedMessage = expectedResult.Item2.ToString();
            Assert.AreEqual(expectedValue, isValid);
            Assert.AreEqual(expectedMessage, errorMessage.ToString());
        }

        [TestMethod]
        public void IsValid_AgeLimit_ErrorMessage()
        {
            user.BirthDate = DateTime.Now;
            var isValid = false;
            errorMessage.AppendLine("You are too young. Come back in 16 years ");

            //Assert
            var expectedResult= user.IsValid();

            //Assert
            Assert.IsNotNull(expectedResult);
            var expectedValue = expectedResult.Item1;
            var expectedMessage = expectedResult.Item2.ToString();
            Assert.AreEqual(expectedValue, isValid);
            Assert.AreEqual(expectedMessage, errorMessage.ToString());
        }
    }
}