using AutoMapper;
using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Exceptions;
using GAC.WMS.Infrastructure.Persistence;
using GAC.WMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GAC.WMS.UnitTests.Application.Tests
{
    [TestClass]
    public class CustomerServiceTests
    {
        private AppDbContext _dbContext = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IValidatorService<CustomerDto>> _validatorMock = null!;
        private Mock<ILogger<ICustomerService>> _loggerMock = null!;
        private ICustomerService _customerService = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestCustomerDb")
                .Options;

            _dbContext = new AppDbContext(options);
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidatorService<CustomerDto>>();
            _loggerMock = new Mock<ILogger<ICustomerService>>();
            _customerService = new CustomerService(_dbContext, _mapperMock.Object, _validatorMock.Object, _loggerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnListOfCustomers()
        {
            var cancellationToken = CancellationToken.None;
            _dbContext.Set<Customer>().AddRange(new List<Customer>
            {
                new () { Id = 1, CompanyName = "Company A", ContactPersonName = "Akshay Mahure", Address="pune", Contact="12345678" },
                new () { Id = 2, CompanyName = "Company B", ContactPersonName = "Akshay Mahure", Address="pune", Contact="12345678" }
            });
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<CustomerDto>(It.IsAny<Customer>()))
                .Returns((Customer c) => new CustomerDto { Id = c.Id, CompanyName = c.CompanyName, ContactPersonName = c.ContactPersonName });

            var result = await _customerService.GetAllAsync(cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Company A", result.First().CompanyName);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnCustomer_WhenIdExists()
        {
            var cancellationToken = CancellationToken.None;
            var customer = new Customer { Id = 1, CompanyName = "Company A", ContactPersonName = "Akshay Mahure", Address = "pune", Contact = "12345678" };
            _dbContext.Set<Customer>().Add(customer);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<CustomerDto>(customer))
                .Returns(new CustomerDto { Id = 1, CompanyName = "Company A", ContactPersonName = "Akshay Mahure", Address = "pune", Contact = "12345678" });

            var result = await _customerService.GetByIdAsync(1, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Company A", result.CompanyName);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public async Task GetByIdAsync_ShouldThrowException_WhenIdDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;

            await _customerService.GetByIdAsync(99, cancellationToken);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldReturnUpdatedCustomer_WhenCustomerExists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var existingCustomer = new Customer
            {
                Id = 1,
                CompanyName = "Company A",
                ContactPersonName = "Akshay Mahure",
                Address = "Pune",
                Contact = "12345678",
                CreatedAt = DateTime.UtcNow
            };
            var updatedCustomerDto = new CustomerDto
            {
                Id = 1,
                CompanyName = "Updated Company A",
                ContactPersonName = "Updated Name",
                Address = "Updated Pune",
                Contact = "87654321"
            };

            _dbContext.Set<Customer>().Add(existingCustomer);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<Customer>(updatedCustomerDto)).Returns(existingCustomer);
            _mapperMock.Setup(m => m.Map<CustomerDto>(existingCustomer)).Returns(updatedCustomerDto);

            // Act
            var result = await _customerService.UpdateAsync(updatedCustomerDto, cancellationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Company A", result.CompanyName);
            Assert.AreEqual("Updated Name", result.ContactPersonName);
            Assert.AreEqual("Updated Pune", result.Address);
            Assert.AreEqual("87654321", result.Contact);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowException_WhenCustomerDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;
            var nonExistentCustomerDto = new CustomerDto
            {
                Id = 99,
                CompanyName = "Non-Existent Company",
                ContactPersonName = "Non-Existent Person",
                Address = "Non-Existent Address",
                Contact = "00000000"
            };

            _validatorMock.Setup(v => v.ValidateAsync(nonExistentCustomerDto, cancellationToken)).Returns(Task.CompletedTask);

            await Assert.ThrowsExceptionAsync<ItemNotFoundException>(() => _customerService.UpdateAsync(nonExistentCustomerDto, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_ShouldReturnCreatedCustomer()
        {
            var cancellationToken = CancellationToken.None;
            var customerDto = new CustomerDto { CompanyName = "Company A", ContactPersonName = "Akshay Mahure", Address = "pune", Contact = "12345678" };
            var customer = new Customer { Id = 1, CompanyName = "Company A", ContactPersonName = "Akshay Mahure", Address = "pune", Contact = "12345678" };

            _validatorMock.Setup(v => v.ValidateAsync(customerDto, cancellationToken)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<Customer>(customerDto)).Returns(customer);
            _mapperMock.Setup(m => m.Map<CustomerDto>(customer)).Returns(customerDto);

            var result = await _customerService.CreateAsync(customerDto, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual("Company A", result.CompanyName);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldReturnTrue_WhenCustomerExists()
        {
            var cancellationToken = CancellationToken.None;
            var customer = new Customer { Id = 1, CompanyName = "Company A", ContactPersonName = "Akshay Mahure", Address = "pune", Contact = "12345678" };
            _dbContext.Set<Customer>().Add(customer);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var result = await _customerService.DeleteAsync(1, cancellationToken);

            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public async Task DeleteAsync_ShouldThrowException_WhenCustomerDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;

            await _customerService.DeleteAsync(99, cancellationToken);
        }
    }
}