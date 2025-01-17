using AutoMapper;
using Gdn.Application.Invoices.Commands.CreateInvoice;
using Gdn.Application.Invoices.Dtos;
using Gdn.Application.Mappings;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Moq;

namespace Gdn.Application.UnitTests.Invoices;

[TestClass]
public class CreateInvoiceCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly IMapper _mapper;

    public CreateInvoiceCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();

        _unitOfWorkMock.Setup(m => m.GetRepository<IInvoiceRepository>()).Returns(_invoiceRepositoryMock.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InvoiceProfile>());
        _mapper = new Mapper(config);
    }

    [TestMethod]
    public async Task Handle_WhenValidRequest_ShouldAddInvoice()
    {
        // Arrange
        var handler = new CreateInvoiceCommandHandler(_unitOfWorkMock.Object, _mapper);
        var invoiceInput = new InvoiceInput() { Number = "1" };
        var command = new CreateInvoiceCommand(invoiceInput);
        var invoice = new Invoice();

        _unitOfWorkMock.Setup(u => u.GetRepository<IInvoiceRepository>()).Returns(_invoiceRepositoryMock.Object);
        _invoiceRepositoryMock.Setup(r => r.Add(It.IsAny<Invoice>())).Returns(Task.FromResult(invoice));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _invoiceRepositoryMock.Verify(r => r.Add(It.IsAny<Invoice>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(Result<Invoice>)); // Assuming Result<T> is the expected return type
    }
}
