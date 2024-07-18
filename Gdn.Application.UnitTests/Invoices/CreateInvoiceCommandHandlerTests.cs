using AutoMapper;
using Gdn.Application.Invoices.Commands.CreateInvoice;
using Gdn.Application.Invoices.Dtos;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Moq;

namespace Gdn.Application.UnitTests.Invoices;

[TestClass]
public class CreateInvoiceCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IInvoiceRepository> _mockInvoiceRepository;
    private readonly Mock<IMapper> _mockMapper;

    public CreateInvoiceCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockInvoiceRepository = new Mock<IInvoiceRepository>();

        _mockUnitOfWork.Setup(m => m.GetRepository<IInvoiceRepository>()).Returns(_mockInvoiceRepository.Object);

        _mockMapper = new Mock<IMapper>();
    }

    [TestMethod]
    public async Task Handle_GivenValidRequest_ShouldAddInvoice()
    {
        // Arrange
        var handler = new CreateInvoiceCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        var invoiceInput = new InvoiceInput() { Number = "1" }; // Assuming InvoiceInput is the correct type for the parameter
        var command = new CreateInvoiceCommand(invoiceInput);
        var invoice = new Invoice();

        _mockUnitOfWork.Setup(u => u.GetRepository<IInvoiceRepository>()).Returns(_mockInvoiceRepository.Object);
        _mockMapper.Setup(m => m.Map<Invoice>(It.IsAny<InvoiceInput>())).Returns(invoice);
        _mockInvoiceRepository.Setup(r => r.AddAsync(It.IsAny<Invoice>())).Returns(Task.FromResult(invoice));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _mockInvoiceRepository.Verify(r => r.AddAsync(It.IsAny<Invoice>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(Result<Invoice>)); // Assuming Result<T> is the expected return type
    }
}
