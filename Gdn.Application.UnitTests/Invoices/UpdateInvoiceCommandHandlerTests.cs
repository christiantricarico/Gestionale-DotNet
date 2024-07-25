using AutoMapper;
using Gdn.Application.Invoices.Commands.UpdateInvoice;
using Gdn.Application.Invoices.Dtos;
using Gdn.Application.Mappings;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Moq;

namespace Gdn.Application.UnitTests.Invoices;

[TestClass]
public class UpdateInvoiceCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IMapper _mapper;

    public UpdateInvoiceCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();

        _unitOfWorkMock.Setup(m => m.GetRepository<IInvoiceRepository>()).Returns(_invoiceRepositoryMock.Object);

        _mapperMock = new Mock<IMapper>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InvoiceProfile>());
        _mapper = new Mapper(config);

    }

    [TestMethod]
    public async Task Handle_WhenValidRequest_ShouldUpdateInvoice()
    {
        int invoiceId = 1;

        // Arrange
        var handler = new UpdateInvoiceCommandHandler(_unitOfWorkMock.Object, _mapper);
        var invoiceInput = new InvoiceInput() { Id = invoiceId, Number = "2" }; // simulate number change
        var command = new UpdateInvoiceCommand(invoiceInput);
        var invoice = new Invoice() { Id = invoiceId, Number = "1" };

        _invoiceRepositoryMock.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(invoice);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(Result<Invoice>));
    }
}
