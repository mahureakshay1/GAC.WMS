using AutoMapper;
using GAC.WMS.Application.Dtos;
using GAC.WMS.Application.Interfaces;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Enums;
using GAC.WMS.Domain.Exceptions;
using GAC.WMS.Infrastructure.Persistence;
using GAC.WMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GAC.WMS.UnitTests.Application.Tests
{
    [TestClass]
    public class ProductServiceTests
    {
        private AppDbContext _dbContext = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IValidatorService<ProductDto>> _validatorMock = null!;
        private Mock<ILogger<IProductService>> _loggerMock = null!;
        private IProductService _productService = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestProductDb")
                .Options;

            _dbContext = new AppDbContext(options);
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidatorService<ProductDto>>();
            _loggerMock = new Mock<ILogger<IProductService>>();
            _productService = new ProductService(_dbContext, _mapperMock.Object, _validatorMock.Object, _loggerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnListOfProducts()
        {
            var cancellationToken = CancellationToken.None;
            _dbContext.Set<Product>().AddRange(new List<Product>
            {
                new () { Id = 1, Title = "Product A", Code = "P001" ,Description = "description" },
                new () { Id = 2, Title = "Product B", Code = "P002" ,Description = "description"}
            });
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns((Product p) => new ProductDto { Id = p.Id, Title = p.Title, Code = p.Code });

            var result = await _productService.GetAllAsync(cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Product A", result.First().Title);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenIdExists()
        {
            var cancellationToken = CancellationToken.None;
            var product = new Product { Id = 3, Title = "Product A", Code = "P001", Description = "description" };
            _dbContext.Set<Product>().Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<ProductDto>(product))
                .Returns(new ProductDto { Id = 3, Title = "Product A", Code = "P001", Description = "description" });

            var result = await _productService.GetByIdAsync(3, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Id);
            Assert.AreEqual("Product A", result.Title);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldThrowException_WhenIdDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;
            await Assert.ThrowsExceptionAsync<ItemNotFoundException>(() => _productService.GetByIdAsync(99, cancellationToken));
        }

        [TestMethod]
        public async Task GetByCodeAsync_ShouldReturnProduct_WhenCodeExists()
        {
            var cancellationToken = CancellationToken.None;
            var product = new Product { Id = 4, Title = "Product A", Code = "P001", Description = "description" };
            _dbContext.Set<Product>().Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<ProductDto>(product))
                .Returns(new ProductDto { Id = 4, Title = "Product A", Code = "P001" });

            var result = await _productService.GetByCodeAsync("P001", cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual("P001", result.Code);
        }

        [TestMethod]
        public async Task GetByCodeAsync_ShouldThrowException_WhenCodeDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;

            await Assert.ThrowsExceptionAsync<ItemNotFoundException>(() => _productService.GetByCodeAsync("INVALID_CODE", cancellationToken));
        }

        [TestMethod]
        public async Task GetByNameAsync_ShouldReturnProduct_WhenNameExists()
        {
            var cancellationToken = CancellationToken.None;
            var product = new Product { Id = 5, Title = "Product A", Code = "P001", Description = "description" };
            _dbContext.Set<Product>().Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<ProductDto>(product))
                .Returns(new ProductDto { Id = 5, Title = "Product A", Code = "P001", Description = "description" });

            var result = await _productService.GetByNameAsync("Product A", cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual("Product A", result.Title);
        }

        [TestMethod]
        public async Task GetByNameAsync_ShouldThrowException_WhenNameDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;
            await Assert.ThrowsExceptionAsync<ItemNotFoundException>(() => _productService.GetByNameAsync("INVALID_NAME", cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_ShouldReturnCreatedProduct()
        {
            var cancellationToken = CancellationToken.None;
            var productDto = new ProductDto { Title = "Product A", Code = "P001", Description = "description" };
            var product = new Product { Title = "Product A", Code = "P001", Description = "description" };

            _validatorMock.Setup(v => v.ValidateAsync(productDto, cancellationToken)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<Product>(productDto)).Returns(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

            var result = await _productService.CreateAsync(productDto, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual("Product A", result.Title);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldReturnUpdatedProduct_WhenProductExists()
        {
            var cancellationToken = CancellationToken.None;
            var existingProduct = new Product
            {
                Id = 1,
                Title = "Product A",
                Code = "P001",
                Description = "Original Description",
                CreatedAt = DateTime.UtcNow
            };
            var updatedProductDto = new ProductDto
            {
                Id = 1,
                Title = "Updated Product A",
                Code = "P001",
                Description = "Updated Description"
            };

            _dbContext.Set<Product>().Add(existingProduct);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<Product>(updatedProductDto)).Returns(existingProduct);
            _mapperMock.Setup(m => m.Map<ProductDto>(existingProduct)).Returns(updatedProductDto);

            var result = await _productService.UpdateAsync(updatedProductDto, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Product A", result.Title);
            Assert.AreEqual("Updated Description", result.Description);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;
            var nonExistentProductDto = new ProductDto
            {
                Id = 99,
                Title = "Non-Existent Product",
                Code = "P999",
                Description = "Non-Existent Description",
                QuantityAvailable = 0,
                CreatedAt = DateTime.UtcNow,
                UnitOfDimension = DimensionUnit.Centimeter,
                UnitOfQuantity = QuantityUnit.Piece,
                Height = 1,
                Length = 1,
                Weight = 1,
                Width = 1
            };

            _mapperMock.Setup(m => m.Map<Product>(nonExistentProductDto))
               .Returns(new Product
               {
                   Id = nonExistentProductDto.Id,
                   Title = nonExistentProductDto.Title,
                   Code = nonExistentProductDto.Code,
                   Description = nonExistentProductDto.Description
               });


            await Assert.ThrowsExceptionAsync<ItemNotFoundException>(() => _productService.UpdateAsync(nonExistentProductDto, cancellationToken));
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldReturnTrue_WhenProductExists()
        {
            var cancellationToken = CancellationToken.None;
            var product = new Product { Id = 7, Title = "Product A", Code = "P001", Description = "description" };
            _dbContext.Set<Product>().Add(product);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var result = await _productService.DeleteAsync(7, cancellationToken);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;

            await Assert.ThrowsExceptionAsync<ItemNotFoundException>(() => _productService.DeleteAsync(99, cancellationToken));
        }
    }
}