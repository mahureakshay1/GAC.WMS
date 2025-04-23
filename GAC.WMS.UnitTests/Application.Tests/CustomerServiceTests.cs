using AutoMapper;
using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Exceptions;
using GAC.WMS.Infrastructure.Persistence;
using GAC.WMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GAC.WMS.UnitTests.Application.Tests
{
    [TestClass]
    public class CustomerServiceTests
    {
        private AppDbContext _dbContext;
        private Mock<IMapper> _mapperMock;
        private Mock<IValidatorService<CustomerDto>> _validatorMock;
        private CustomerService _customerService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestCustomerDb")
                .Options;

            _dbContext = new AppDbContext(options);
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidatorService<CustomerDto>>();
            _customerService = new CustomerService(_dbContext, _mapperMock.Object, _validatorMock.Object);
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
                new Customer { Id = 1, CompanyName = "Company A", ContactPersonName = "Akshay Mahure", Address="pune", Contact="12345678" },
                new Customer { Id = 2, CompanyName = "Company B", ContactPersonName = "Akshay Mahure", Address="pune", Contact="12345678" }
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