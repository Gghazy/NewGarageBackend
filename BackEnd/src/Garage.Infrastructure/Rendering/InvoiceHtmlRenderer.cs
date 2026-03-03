using System.Globalization;
using System.Net;
using System.Text;
using Garage.Application.Abstractions;
using Garage.Contracts.Invoices;

namespace Garage.Infrastructure.Rendering;

public sealed class InvoiceHtmlRenderer : IInvoiceHtmlRenderer
{
    public string RenderPublicView(InvoiceDto inv, string lang = "ar")
    {
        var isAr = !string.Equals(lang, "en", StringComparison.OrdinalIgnoreCase);
        var t = GetLabels(isAr);
        var dir = isAr ? "rtl" : "ltr";
        var culture = isAr ? new CultureInfo("ar-SA") : new CultureInfo("en-US");
        var dateStr = inv.CreatedAtUtc.ToString("dd/MM/yyyy", culture);
        var timeStr = inv.CreatedAtUtc.ToString("hh:mm tt", culture);

        var itemsRows = BuildItemsRows(inv.Items, isAr);
        var paymentsRows = BuildPaymentsRows(inv.Payments, isAr, culture);

        var statusClass = inv.Status switch
        {
            "Issued" => "bg-issued",
            "Paid"   => "bg-paid",
            _        => "bg-cancelled",
        };
        var statusLabel = inv.Status switch
        {
            "Issued"    => isAr ? "صادرة"   : "Issued",
            "Paid"      => isAr ? "مدفوعة"  : "Paid",
            "Cancelled" => isAr ? "ملغاة"   : "Cancelled",
            "Refunded"  => isAr ? "مستردة"  : "Refunded",
            _           => E(inv.Status),
        };

        var clientName = isAr ? inv.ClientNameAr : inv.ClientNameEn;
        var branchName = isAr ? inv.BranchNameAr : inv.BranchNameEn;

        var sb = new StringBuilder();
        sb.Append($@"<!DOCTYPE html>
<html dir='{dir}' lang='{(isAr ? "ar" : "en")}'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>{t["invoice"]} - {E(inv.InvoiceNumber ?? inv.Id.ToString())}</title>
    <style>
        * {{ margin:0; padding:0; box-sizing:border-box; }}
        body {{ font-family:'Segoe UI',Tahoma,sans-serif; font-size:14px; color:#333; padding:20px; direction:{dir}; max-width:800px; margin:0 auto; background:#fff; }}
        .header {{ text-align:center; margin-bottom:24px; padding-bottom:16px; border-bottom:2px solid #ddd; }}
        .header h1 {{ font-size:24px; margin-bottom:4px; color:#333; }}
        .header .meta {{ font-size:13px; color:#888; }}
        .info-row {{ display:flex; justify-content:space-between; padding:10px 16px; border-bottom:1px solid #eee; }}
        .info-row .label {{ color:#888; font-size:13px; }}
        .info-row .value {{ font-weight:600; }}
        table {{ width:100%; border-collapse:collapse; margin:16px 0; }}
        th, td {{ border:1px solid #ddd; padding:8px; font-size:13px; }}
        th {{ background:#f5f5f5; font-weight:600; }}
        .section-title {{ font-size:15px; font-weight:700; margin:20px 0 8px; padding-bottom:4px; border-bottom:1px solid #eee; }}
        .summary {{ display:grid; grid-template-columns:1fr 1fr; gap:8px; margin:16px 0; }}
        .summary .box {{ border:1px solid #ddd; border-radius:6px; padding:10px; text-align:center; }}
        .summary .box .lbl {{ font-size:12px; color:#888; display:block; }}
        .summary .box .val {{ font-size:16px; font-weight:700; }}
        .text-success {{ color:#198754; }} .text-danger {{ color:#dc3545; }}
        .badge {{ display:inline-block; padding:3px 12px; border-radius:4px; font-size:13px; font-weight:600; color:#fff; }}
        .bg-issued {{ background:#0d6efd; }} .bg-paid {{ background:#198754; }} .bg-cancelled {{ background:#dc3545; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>{t["invoice"]}</h1>
        <div class='meta'>{t["invoiceNumber"]}: {E(inv.InvoiceNumber ?? "—")}</div>
        <div class='meta'>{t["date"]}: {dateStr} {timeStr}</div>
        <div style='margin-top:8px'>
            <span class='badge {statusClass}'>{statusLabel}</span>
        </div>
    </div>

    <div class='info-row'><span class='label'>{t["clientName"]}</span><span class='value'>{E(clientName)}</span></div>
    <div class='info-row'><span class='label'>{t["clientPhone"]}</span><span class='value'>{E(inv.ClientPhone)}</span></div>
    <div class='info-row'><span class='label'>{t["branch"]}</span><span class='value'>{E(branchName)}</span></div>

    <div class='section-title'>{t["items"]}</div>
    <table>
        <thead><tr>
            <th style='width:36px'>#</th>
            <th>{t["description"]}</th>
            <th>{t["unitPrice"]}</th>
            <th>{t["total"]}</th>
        </tr></thead>
        <tbody>{itemsRows}</tbody>
    </table>

    <div class='summary'>
        <div class='box'><span class='lbl'>{t["subTotal"]}</span><div class='val'>{inv.SubTotal:F2}</div></div>
        <div class='box'><span class='lbl'>{t["discount"]}</span><div class='val text-danger'>{inv.DiscountAmount:F2}</div></div>
        <div class='box'><span class='lbl'>{t["tax"]} ({inv.TaxRate * 100:F0}%)</span><div class='val'>{inv.TaxAmount:F2}</div></div>
        <div class='box' style='background:#f5f5f5'><span class='lbl'>{t["totalWithTax"]}</span><div class='val'>{inv.TotalWithTax:F2} {E(inv.Currency)}</div></div>
        <div class='box'><span class='lbl'>{t["totalPaid"]}</span><div class='val text-success'>{inv.TotalPaid:F2}</div></div>
        <div class='box'><span class='lbl'>{t["balance"]}</span><div class='val {(inv.Balance > 0 ? "text-danger" : "text-success")}'>{inv.Balance:F2} {E(inv.Currency)}</div></div>
    </div>");

        if (inv.Payments.Count > 0)
        {
            sb.Append($@"
    <div class='section-title'>{t["payments"]}</div>
    <table>
        <thead><tr>
            <th style='width:36px'>#</th>
            <th>{t["paymentDate"]}</th>
            <th>{t["paymentType"]}</th>
            <th>{t["amount"]}</th>
            <th>{t["method"]}</th>
        </tr></thead>
        <tbody>{paymentsRows}</tbody>
    </table>");
        }

        sb.Append(@"
</body>
</html>");

        return sb.ToString();
    }

    private static string BuildItemsRows(List<InvoiceItemDto> items, bool isAr)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var name = isAr
                ? (item.ServiceNameAr ?? item.Description)
                : (item.ServiceNameEn ?? item.Description);
            sb.Append($@"
            <tr>
                <td style='text-align:center'>{i + 1}</td>
                <td>{E(name)}</td>
                <td style='text-align:center'>{item.UnitPrice:F2}</td>
                <td style='text-align:center'>{item.TotalPrice:F2}</td>
            </tr>");
        }
        return sb.ToString();
    }

    private static string BuildPaymentsRows(List<InvoicePaymentDto> payments, bool isAr, CultureInfo culture)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < payments.Count; i++)
        {
            var p = payments[i];
            var methodName = isAr ? p.MethodNameAr : p.MethodNameEn;
            var dateStr = p.CreatedAtUtc.ToString("dd/MM/yyyy", culture);
            sb.Append($@"
            <tr>
                <td style='text-align:center'>{i + 1}</td>
                <td>{dateStr}</td>
                <td style='text-align:center'>{E(p.Type)}</td>
                <td style='text-align:center'>{p.Amount:F2} {E(p.Currency)}</td>
                <td>{E(methodName)}</td>
            </tr>");
        }
        return sb.ToString();
    }

    private static Dictionary<string, string> GetLabels(bool isAr) => isAr
        ? new Dictionary<string, string>
        {
            ["invoice"]       = "فاتورة",
            ["invoiceNumber"] = "رقم الفاتورة",
            ["date"]          = "التاريخ",
            ["clientName"]    = "اسم العميل",
            ["clientPhone"]   = "رقم العميل",
            ["branch"]        = "الفرع",
            ["items"]         = "البنود",
            ["description"]   = "الوصف",
            ["unitPrice"]     = "سعر الوحدة",
            ["total"]         = "الإجمالي",
            ["subTotal"]      = "المجموع الفرعي",
            ["discount"]      = "الخصم",
            ["tax"]           = "الضريبة",
            ["totalWithTax"]  = "الإجمالي شامل الضريبة",
            ["totalPaid"]     = "المدفوع",
            ["balance"]       = "الرصيد",
            ["payments"]      = "المدفوعات",
            ["paymentDate"]   = "التاريخ",
            ["paymentType"]   = "النوع",
            ["amount"]        = "المبلغ",
            ["method"]        = "الطريقة",
        }
        : new Dictionary<string, string>
        {
            ["invoice"]       = "Invoice",
            ["invoiceNumber"] = "Invoice Number",
            ["date"]          = "Date",
            ["clientName"]    = "Client Name",
            ["clientPhone"]   = "Client Phone",
            ["branch"]        = "Branch",
            ["items"]         = "Items",
            ["description"]   = "Description",
            ["unitPrice"]     = "Unit Price",
            ["total"]         = "Total",
            ["subTotal"]      = "Subtotal",
            ["discount"]      = "Discount",
            ["tax"]           = "Tax",
            ["totalWithTax"]  = "Total with Tax",
            ["totalPaid"]     = "Total Paid",
            ["balance"]       = "Balance",
            ["payments"]      = "Payments",
            ["paymentDate"]   = "Date",
            ["paymentType"]   = "Type",
            ["amount"]        = "Amount",
            ["method"]        = "Method",
        };

    private static string E(string? value) => WebUtility.HtmlEncode(value ?? "");
}
