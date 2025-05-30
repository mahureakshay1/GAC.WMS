﻿using AutoMapper;
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
using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.UnitTests.Application.Tests
{
    [TestClass]
    public class PurchaseOrderServiceTests
    {
        private AppDbContext _dbContext = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IValidatorService<PurchaseOrderDto>> _orderValidatorMock = null!;
        private Mock<IValidatorService<PurchaseOrderLineDto>> _orderLinevalidatorMock = null!;
        private IPurchaseOrderService _purchaseOrderService = null!;
        private Mock<ILogger<IPurchaseOrderService>> _loggerMock = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestPurchaseOrderDb")
                .Options;

            _dbContext = new AppDbContext(options);
            _mapperMock = new Mock<IMapper>();
            _orderValidatorMock = new Mock<IValidatorService<PurchaseOrderDto>>();
            _orderLinevalidatorMock = new Mock<IValidatorService<PurchaseOrderLineDto>>();
            _loggerMock = new Mock<ILogger<IPurchaseOrderService>>();
            _purchaseOrderService = new PurchaseOrderService(_dbContext, _mapperMock.Object, _orderValidatorMock.Object, _orderLinevalidatorMock.Object, _loggerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnListOfPurchaseOrders()
        {
            var cancellationToken = CancellationToken.None;
            _dbContext.Set<PurchaseOrder>().AddRange(new List<PurchaseOrder>
            {
                new PurchaseOrder
                {
                    Id = 1,
                    CustomerId = 1,
                    Customer= new Customer()
                {
                Address = "Pune",
                    CompanyName = "Customer A",
                    Contact = "12345678",
                    ContactPersonName = "Akshay Mahure"
                }
                },
                new PurchaseOrder {
                    Id = 2,
                    CustomerId = 2,
                Customer= new Customer()
                {
                Address = "Pune",
                    CompanyName = "Customer A",
                    Contact = "12345678",
                    ContactPersonName = "Akshay Mahure"
                }}
            });
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>()))
                .Returns((PurchaseOrder p) => new PurchaseOrderDto
                {
                    Id = p.Id,
                    CustomerId = p.CustomerId,
                    PurchaseOrderLines = new List<PurchaseOrderLineDto>()
                    {
                        new PurchaseOrderLineDto()
                        {
                            Id = 1,
                            ProductId = 1,
                            Quantity = 10
                        }
                    }
                });

            var result = await _purchaseOrderService.GetAllAsync(cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.First().CustomerId);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnPurchaseOrder_WhenIdExists()
        {
            var cancellationToken = CancellationToken.None;
            var purchaseOrder = new PurchaseOrder
            {
                Id = 1,
                CustomerId = 1,
                Customer = new Customer()
                {
                    Address = "Pune",
                    CompanyName = "Customer A",
                    Contact = "12345678",
                    ContactPersonName = "Akshay Mahure"
                }

            };
            _dbContext.Set<PurchaseOrder>().Add(purchaseOrder);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(purchaseOrder))
                .Returns(new PurchaseOrderDto
                {
                    Id = 1,
                    CustomerId = 1,
                    PurchaseOrderLines = new List<PurchaseOrderLineDto>()
                    {
                        new PurchaseOrderLineDto()
                        {
                            Id = 1,
                            ProductId = 1,
                            Quantity = 10
                        }
                    }
                });

            var result = await _purchaseOrderService.GetByIdAsync(1, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(1, result.CustomerId);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public async Task GetByIdAsync_ShouldThrowException_WhenIdDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;

            await _purchaseOrderService.GetByIdAsync(99, cancellationToken);
        }

        [TestMethod]
        public async Task GetByCustomerNameAsync_ShouldReturnPurchaseOrders_WhenCustomerNameExists()
        {
            var cancellationToken = CancellationToken.None;
            var customer = new Customer { Id = 1, CompanyName = "Customer A", Address = "Pune", ContactPersonName = "Akshay Mahure", Contact = "12345678" };
            var purchaseOrder = new PurchaseOrder { Id = 1, CustomerId = 1, Customer = customer };
            _dbContext.Set<Customer>().Add(customer);
            _dbContext.Set<PurchaseOrder>().Add(purchaseOrder);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(purchaseOrder))
                .Returns(new PurchaseOrderDto
                {
                    Id = 1,
                    CustomerId = 1,
                    PurchaseOrderLines = new List<PurchaseOrderLineDto>()
                    {
                        new ()
                        {
                            Id = 1,
                            ProductId = 1,
                            Quantity = 10
                        }
                    }

                });

            // Act
            var result = await _purchaseOrderService.GetByCustomerNameAsync("Customer A", cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().CustomerId);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public async Task GetByCustomerNameAsync_ShouldThrowException_WhenCustomerNameDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;

            await _purchaseOrderService.GetByCustomerNameAsync("Invalid Customer", cancellationToken);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldReturnCreatedPurchaseOrder()
        {
            var cancellationToken = CancellationToken.None;
            var purchaseOrderDto = new PurchaseOrderDto
            {
                CustomerId = 1,
                PurchaseOrderLines = new List<PurchaseOrderLineDto>()
                    {
                        new ()
                        {
                            Id = 1,
                            ProductId = 1,
                            Quantity = 10
                        }
                    }
            };
            var purchaseOrder = new PurchaseOrder
            {
                Id = 1,
                CustomerId = 1,
                Customer = new Customer()
                {
                    Address = "Pune",
                    CompanyName = "Customer A",
                    Contact = "12345678",
                    ContactPersonName = "Akshay Mahure"
                }
            };

            _orderValidatorMock.Setup(v => v.ValidateAsync(purchaseOrderDto, cancellationToken)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<PurchaseOrder>(purchaseOrderDto)).Returns(purchaseOrder);
            _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(purchaseOrder)).Returns(purchaseOrderDto);

            var result = await _purchaseOrderService.CreateAsync(purchaseOrderDto, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CustomerId);
        }


        [TestMethod]
        public async Task UpdateAsync_ShouldReturnUpdatedPurchaseOrder_WhenPurchaseOrderExists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var existingPurchaseOrder = new PurchaseOrder
            {
                Id = 1,
                CustomerId = 1,
                Status = OrderStatus.Created,
                Customer = new()
                {
                    Id = 1,
                    CompanyName = "Customer A",
                    Address = "Pune",
                    ContactPersonName = "Akshay Mahure",
                    Contact = "12345678"
                },
                PurchaseOrderLines = new List<PurchaseOrderLine>
                 {
                     new() { Id = 1, ProductId = 1, Quantity = 10, UnitPrice = 100 }
                 }
            };
            var updatedPurchaseOrderDto = new PurchaseOrderDto
            {
                Id = 1,
                CustomerId = 1,
                Status = OrderStatus.Completed,
                PurchaseOrderLines = new List<PurchaseOrderLineDto>
                 {
                     new() { Id = 1, ProductId = 2, Quantity = 20, UnitPrice = 200 },
                     new() { Id = 1, ProductId = 3, Quantity = 20, UnitPrice = 200 }
                 }
            };

            _dbContext.Set<PurchaseOrder>().Add(existingPurchaseOrder);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<PurchaseOrder>(updatedPurchaseOrderDto)).Returns(existingPurchaseOrder);
            _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(existingPurchaseOrder)).Returns(updatedPurchaseOrderDto);

            var result = await _purchaseOrderService.UpdateAsync(updatedPurchaseOrderDto, cancellationToken);

            Assert.IsNotNull(result);
            Assert.AreEqual(OrderStatus.Completed, result.Status);
            Assert.AreEqual(20, result.PurchaseOrderLines.First().Quantity);
            Assert.AreEqual(2, result.PurchaseOrderLines.Count());
            Assert.AreEqual(200, result.PurchaseOrderLines.First().UnitPrice);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldThrowException_WhenPurchaseOrderDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;

            var nonExistentPurchaseOrderDto = new PurchaseOrderDto
            {
                Id = 99,
                CustomerId = 1,
                Status = OrderStatus.Created,
                PurchaseOrderLines = new List<PurchaseOrderLineDto>
            {
                new() { Id = 1, ProductId = 1, Quantity = 10, UnitPrice = 100 }
            }
            };

            _mapperMock.Setup(m => m.Map<PurchaseOrder>(nonExistentPurchaseOrderDto))
                .Returns(new PurchaseOrder
                {
                    Id = nonExistentPurchaseOrderDto.Id,
                    CustomerId = nonExistentPurchaseOrderDto.CustomerId,
                    Status = nonExistentPurchaseOrderDto.Status,
                    Customer = new Customer
                    {
                        Id = nonExistentPurchaseOrderDto.CustomerId,
                        CompanyName = "Customer A",
                        Address = "Pune",
                        ContactPersonName = "Akshay Mahure",
                        Contact = "12345678"
                    },
                });

            await Assert.ThrowsExceptionAsync<ItemNotFoundException>(() => _purchaseOrderService.UpdateAsync(nonExistentPurchaseOrderDto, cancellationToken));
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldReturnTrue_WhenPurchaseOrderExists()
        {
            var cancellationToken = CancellationToken.None;
            var purchaseOrder = new PurchaseOrder
            {
                Id = 1,
                CustomerId = 1,
                Customer = new()
                {
                    Address = "Pune",
                    CompanyName = "Customer A",
                    Contact = "12345678",
                    ContactPersonName = "Akshay Mahure"

                }
            };
            _dbContext.Set<PurchaseOrder>().Add(purchaseOrder);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var result = await _purchaseOrderService.DeleteAsync(1, cancellationToken);

            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public async Task DeleteAsync_ShouldThrowException_WhenPurchaseOrderDoesNotExist()
        {
            var cancellationToken = CancellationToken.None;

            await _purchaseOrderService.DeleteAsync(99, cancellationToken);
        }
    }
}
