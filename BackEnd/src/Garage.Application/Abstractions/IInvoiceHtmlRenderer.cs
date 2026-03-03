using Garage.Contracts.Invoices;

namespace Garage.Application.Abstractions;

public interface IInvoiceHtmlRenderer
{
    string RenderPublicView(InvoiceDto invoice, string lang = "ar");
}
