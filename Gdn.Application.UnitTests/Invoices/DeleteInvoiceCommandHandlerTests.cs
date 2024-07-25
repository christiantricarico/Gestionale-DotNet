using Gdn.Application.Invoices;
using Gdn.Application.Invoices.Commands.DeleteInvoice;
using Gdn.Domain.Data;
using Gdn.Domain.Data.Repositories;
using Gdn.Domain.Models;
using Moq;

namespace Gdn.Application.UnitTests.Invoices;

[TestClass]
public class DeleteInvoiceCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    //zprivate DeleteInvoiceCommandHandler _sut;

    public DeleteInvoiceCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();

        _unitOfWorkMock.Setup(m => m.GetRepository<IInvoiceRepository>()).Returns(_invoiceRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Handle_WhenInvoiceNotFound_ReturnsNotFound()
    {
        // Arrange
        var invoiceId = 1;
        Invoice? invoice = null;
        _invoiceRepositoryMock.Setup(x => x.GetAsync(invoiceId)).ReturnsAsync(invoice);

        // Act
        var sut = new DeleteInvoiceCommandHandler(_unitOfWorkMock.Object);
        var result = await sut.Handle(new DeleteInvoiceCommand(invoiceId), CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(InvoiceErrors.NotFound(invoiceId).Code, result.Error!.Code);
    }

    [TestMethod]
    public async Task Handle_WhenInvoiceFound_ReturnsTrue()
    {
        // Arrange
        var invoiceId = 1;
        var invoice = new Invoice { Id = invoiceId, IsDeleted = false };
        _invoiceRepositoryMock.Setup(x => x.GetAsync(invoiceId)).ReturnsAsync(invoice);

        // Act
        var sut = new DeleteInvoiceCommandHandler(_unitOfWorkMock.Object);
        var result = await sut.Handle(new DeleteInvoiceCommand(invoiceId), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Data);
        Assert.IsTrue(invoice.IsDeleted);
    }
}
