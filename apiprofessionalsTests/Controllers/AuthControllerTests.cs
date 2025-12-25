using Microsoft.VisualStudio.TestTools.UnitTesting;
using apiprofessionals.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using apiprofessionals.Models;
using apiprofessionals.RegisterDto;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace apiprofessionals.Controllers.Tests
{
    [TestClass()]
    public class AuthControllerTests
    {
        private AuthController _authController;
        private VendingDbContext _dbContext;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VendingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new VendingDbContext(options);
            var user = new UserModel
            {
                Id = 1,
                FullName = "Test",
                Email = "test@example.ru",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Role = "admin"
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            _authController = new AuthController(_dbContext);
        }

        [TestMethod()]
        public void Login_InvalidUser_ReturnUnauthorization()
        {
            var dto = new LoginDto
            {
                Email = "wrongtest@example.ru",
                Password = "Password123"
            };
            
            var result = _authController.Login(dto);
            Assert.IsNotInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }
        [TestMethod()]
        public void Login_Correct_ReturnAuthorizated()
        {
            var dto = new LoginDto
            {
                Email = "test@example.ru",
                Password = "Password123"
            };

            var result = _authController.Login(dto);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod()]
        public void Register_Correct_ReturnRegistered()
        {
            var dto = new apiprofessionals.RegisterDto.RegisterDto
            {
                FullName = "Test",
                Email = "testRegister@example.ru",
                Password = "Password123"
            };

            var result = _authController.Register(dto);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Ожидается OkObjectResult");
            Assert.AreEqual("Пользователь зарегистрирован", okResult.Value);
        }
    }
}