using System.Globalization;
using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using Microsoft.Extensions.Localization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Garage.Infrastructure.Reporting;

public class ExaminationReportPdfService : IExaminationReportPdfService
{
    private readonly IStringLocalizer _loc;

    public ExaminationReportPdfService(IStringLocalizer localizer)
    {
        _loc = localizer;
    }

    // ── Colors matching the HTML report ──────────────────────────────────────
    private static readonly string HeaderBg = "#f8f9fa";
    private static readonly string StageBorderColor = "#198754";
    private static readonly string InfoBorderColor = "#0d6efd";
    private static readonly string NoIssuesBg = "#d1e7dd";
    private static readonly string NoIssuesText = "#0f5132";
    private static readonly string CommentsBg = "#f8f9fa";
    private static readonly string LabelColor = "#6c757d";
    private static readonly string BorderColor = "#dee2e6";
    private static readonly string LightBorder = "#f0f0f0";

    public byte[] Generate(ExaminationReportDto report)
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var isAr = lang == "ar";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(h => ComposeHeader(h, report.Examination));
                page.Content().Element(c => ComposeContent(c, report, lang));
                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Header
    // ═══════════════════════════════════════════════════════════════════════════

    private void ComposeHeader(IContainer container, ExaminationDto exam)
    {
        container.Column(col =>
        {
            col.Item().AlignCenter().Text(_loc["Report.Title"].Value)
                .FontSize(18).Bold();
            col.Item().AlignCenter().Text(exam.CreatedAtUtc.ToString("yyyy-MM-dd"))
                .FontSize(10).FontColor(LabelColor);
            col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(BorderColor);
        });
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Content
    // ═══════════════════════════════════════════════════════════════════════════

    private void ComposeContent(IContainer container, ExaminationReportDto report, string lang)
    {
        container.Column(col =>
        {
            col.Spacing(12);

            // Exam Info
            ComposeExamInfo(col, report.Examination, lang);

            // Services
            ComposeServices(col, report.Examination, lang);

            // Stage 1: Sensors
            if (report.SensorStage is not null)
                ComposeSensorStage(col, report.SensorStage, lang);

            // Stage 2: Dashboard Indicators
            if (report.DashboardIndicatorsStage is not null)
                ComposeDashboardStage(col, report.DashboardIndicatorsStage);

            // Stage 3: Interior Body
            if (report.InteriorBodyStage is not null)
                ComposePartIssueStage(col, report.InteriorBodyStage, _loc["Report.Stages.InteriorBody"].Value, lang);

            // Stage 4: Exterior Body
            if (report.ExteriorBodyStage is not null)
                ComposePartIssueStage(col, report.ExteriorBodyStage, _loc["Report.Stages.ExteriorBody"].Value, lang);

            // Stage 5: Interior Decor
            if (report.InteriorDecorStage is not null)
                ComposePartIssueStage(col, report.InteriorDecorStage, _loc["Report.Stages.InteriorDecor"].Value, lang);

            // Stage 6: Accessories
            if (report.AccessoryStage is not null)
                ComposePartIssueStage(col, report.AccessoryStage, _loc["Report.Stages.Accessories"].Value, lang);

            // Stage 7: Mechanical
            if (report.MechanicalStage is not null)
                ComposeMechanicalStage(col, report.MechanicalStage, lang);

            // Stage 8: Tires
            if (report.TireStage is not null)
                ComposeTireStage(col, report.TireStage, lang);

            // Stage 9: Road Test
            if (report.RoadTestStage is not null)
                ComposeRoadTestStage(col, report.RoadTestStage, lang);
        });
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Exam Info Section
    // ═══════════════════════════════════════════════════════════════════════════

    private void ComposeExamInfo(ColumnDescriptor col, ExaminationDto exam, string lang)
    {
        col.Item().Element(c => SectionTitle(c, _loc["Report.ExamInfo"].Value, false));

        col.Item().Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.RelativeColumn(); // label
                c.RelativeColumn(); // value
                c.RelativeColumn(); // label
                c.RelativeColumn(); // value
            });

            var rows = new List<(string Label, string Value)>
            {
                (_loc["Report.Client"].Value, N(exam.ClientNameAr, exam.ClientNameEn, lang)),
                (_loc["Report.CarMark"].Value, $"{N(exam.ManufacturerNameAr, exam.ManufacturerNameEn, lang)} - {N(exam.CarMarkNameAr, exam.CarMarkNameEn, lang)}"),
                (_loc["Report.Plate"].Value, GetPlateDisplay(exam)),
                (_loc["Report.Year"].Value, exam.Year?.ToString() ?? "—"),
                (_loc["Report.Color"].Value, exam.Color ?? "—"),
                (_loc["Report.Mileage"].Value, exam.Mileage.HasValue ? $"{exam.Mileage:N0} {exam.MileageUnit}" : "—"),
                (_loc["Report.VIN"].Value, exam.Vin ?? "—"),
                (_loc["Report.Branch"].Value, N(exam.BranchNameAr, exam.BranchNameEn, lang)),
            };

            if (!string.IsNullOrWhiteSpace(exam.Notes))
                rows.Add((_loc["Report.Notes"].Value, exam.Notes));

            uint rowIndex = 1;
            for (int i = 0; i < rows.Count; i += 2)
            {
                // Left pair
                table.Cell().Row(rowIndex).Column(1)
                    .BorderBottom(1).BorderColor(LightBorder).Padding(6)
                    .Text(rows[i].Label).FontSize(9).FontColor(LabelColor);
                table.Cell().Row(rowIndex).Column(2)
                    .BorderBottom(1).BorderColor(LightBorder).Padding(6)
                    .Text(rows[i].Value).FontSize(9).Bold();

                // Right pair (if exists)
                if (i + 1 < rows.Count)
                {
                    table.Cell().Row(rowIndex).Column(3)
                        .BorderBottom(1).BorderColor(LightBorder).Padding(6)
                        .Text(rows[i + 1].Label).FontSize(9).FontColor(LabelColor);
                    table.Cell().Row(rowIndex).Column(4)
                        .BorderBottom(1).BorderColor(LightBorder).Padding(6)
                        .Text(rows[i + 1].Value).FontSize(9).Bold();
                }

                rowIndex++;
            }
        });
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Services Table
    // ═══════════════════════════════════════════════════════════════════════════

    private void ComposeServices(ColumnDescriptor col, ExaminationDto exam, string lang)
    {
        col.Item().Element(c => SectionTitle(c, _loc["Report.Services"].Value, false));

        col.Item().Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(35);  // #
                c.RelativeColumn();     // Service
                c.ConstantColumn(60);   // Qty
            });

            // Header
            TableHeader(table, 1, "#");
            TableHeader(table, 2, _loc["Report.Service"].Value);
            TableHeader(table, 3, _loc["Report.Qty"].Value);

            for (int i = 0; i < exam.Items.Count; i++)
            {
                var item = exam.Items[i];
                uint row = (uint)(i + 2);
                TableCell(table, row, 1, (i + 1).ToString(), true);
                TableCell(table, row, 2, N(item.ServiceNameAr, item.ServiceNameEn, lang));
                TableCell(table, row, 3, item.Quantity.ToString(), true);
            }
        });
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Stage 1: Sensors
    // ═══════════════════════════════════════════════════════════════════════════

    private void ComposeSensorStage(ColumnDescriptor col, SensorStageReportDto stage, string lang)
    {
        col.Item().Element(c => SectionTitle(c, _loc["Report.Stages.Sensors"].Value, true));

        if (stage.NoIssuesFound)
            col.Item().Element(c => NoIssuesBadge(c));

        if (stage.CylinderCount > 0)
            col.Item().Text($"{_loc["Report.CylinderCount"].Value}: {stage.CylinderCount}")
                .FontSize(9).FontColor("#495057");

        if (stage.Issues.Count > 0)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(35);  // #
                    c.ConstantColumn(90);  // Code
                    c.RelativeColumn();     // Description
                    c.ConstantColumn(80);  // Evaluation
                });

                TableHeader(table, 1, "#");
                TableHeader(table, 2, _loc["Report.IssueCode"].Value);
                TableHeader(table, 3, _loc["Report.Description"].Value);
                TableHeader(table, 4, _loc["Report.Evaluation"].Value);

                for (int i = 0; i < stage.Issues.Count; i++)
                {
                    var issue = stage.Issues[i];
                    uint row = (uint)(i + 2);
                    TableCell(table, row, 1, (i + 1).ToString(), true);
                    TableCell(table, row, 2, issue.Code ?? "—");
                    TableCell(table, row, 3, N(issue.NameAr, issue.NameEn, lang));
                    TableCell(table, row, 4, GetEvaluation(issue.Evaluation));
                }
            });
        }

        ComposeComments(col, stage.Comments);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Stage 2: Dashboard Indicators
    // ═══════════════════════════════════════════════════════════════════════════

    private void ComposeDashboardStage(ColumnDescriptor col, DashboardStageReportDto stage)
    {
        col.Item().Element(c => SectionTitle(c, _loc["Report.Stages.DashboardIndicators"].Value, true));

        col.Item().Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.ConstantColumn(35);  // #
                c.RelativeColumn();     // Indicator
                c.ConstantColumn(100); // Value
            });

            TableHeader(table, 1, "#");
            TableHeader(table, 2, _loc["Report.Indicator"].Value);
            TableHeader(table, 3, _loc["Report.Status"].Value);

            for (int i = 0; i < stage.Indicators.Count; i++)
            {
                var ind = stage.Indicators[i];
                uint row = (uint)(i + 2);
                TableCell(table, row, 1, (i + 1).ToString(), true);
                TableCell(table, row, 2, _loc[$"Report.Indicators.{ind.Key}"].Value);

                var value = ind.NotApplicable
                    ? _loc["Report.NA"].Value
                    : ind.Value?.ToString() ?? "—";
                TableCell(table, row, 3, value, true);
            }
        });

        ComposeComments(col, stage.Comments);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Stages 3,4,5,6: Part + Issue (shared)
    // ═══════════════════════════════════════════════════════════════════════════

    private void ComposePartIssueStage(ColumnDescriptor col, PartIssueStageReportDto stage, string title, string lang)
    {
        col.Item().Element(c => SectionTitle(c, title, true));

        if (stage.NoIssuesFound)
            col.Item().Element(c => NoIssuesBadge(c));

        if (stage.Items.Count > 0)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(35);  // #
                    c.RelativeColumn();     // Part
                    c.RelativeColumn();     // Issue
                });

                TableHeader(table, 1, "#");
                TableHeader(table, 2, _loc["Report.Part"].Value);
                TableHeader(table, 3, _loc["Report.Issue"].Value);

                for (int i = 0; i < stage.Items.Count; i++)
                {
                    var item = stage.Items[i];
                    uint row = (uint)(i + 2);
                    TableCell(table, row, 1, (i + 1).ToString(), true);
                    TableCell(table, row, 2, N(item.PartNameAr, item.PartNameEn, lang));
                    TableCell(table, row, 3, N(item.IssueNameAr, item.IssueNameEn, lang));
                }
            });
        }

        ComposeComments(col, stage.Comments);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Stage 7: Mechanical
    // ═══════════════════════════════════════════════════════════════════════════

    private void ComposeMechanicalStage(ColumnDescriptor col, MechanicalStageReportDto stage, string lang)
    {
        col.Item().Element(c => SectionTitle(c, _loc["Report.Stages.Mechanical"].Value, true));

        if (stage.NoIssuesFound)
            col.Item().Element(c => NoIssuesBadge(c));

        if (stage.Rows.Count > 0)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(35);  // #
                    c.RelativeColumn();     // Part Type
                    c.RelativeColumn();     // Part
                    c.RelativeColumn();     // Issues
                });

                TableHeader(table, 1, "#");
                TableHeader(table, 2, _loc["Report.PartType"].Value);
                TableHeader(table, 3, _loc["Report.PartName"].Value);
                TableHeader(table, 4, _loc["Report.Issues"].Value);

                for (int i = 0; i < stage.Rows.Count; i++)
                {
                    var row = stage.Rows[i];
                    uint rowNum = (uint)(i + 2);
                    TableCell(table, rowNum, 1, (i + 1).ToString(), true);
                    TableCell(table, rowNum, 2, N(row.PartTypeNameAr, row.PartTypeNameEn, lang));
                    TableCell(table, rowNum, 3, N(row.PartNameAr, row.PartNameEn, lang));

                    var issueText = row.Issues.Count > 0
                        ? string.Join("، ", row.Issues.Select(iss => N(iss.NameAr, iss.NameEn, lang)))
                        : "—";
                    TableCell(table, rowNum, 4, issueText);
                }
            });
        }

        ComposeComments(col, stage.Comments);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Stage 8: Tires
    // ═══════════════════════════════════════════════════════════════════════════

    private void ComposeTireStage(ColumnDescriptor col, TireStageReportDto stage, string lang)
    {
        col.Item().Element(c => SectionTitle(c, _loc["Report.Stages.Tires"].Value, true));

        if (stage.NoIssuesFound)
            col.Item().Element(c => NoIssuesBadge(c));

        if (stage.Items.Count > 0)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(35);  // #
                    c.RelativeColumn();     // Position
                    c.ConstantColumn(70);  // Year
                    c.ConstantColumn(70);  // Week
                    c.ConstantColumn(100); // Condition
                });

                TableHeader(table, 1, "#");
                TableHeader(table, 2, _loc["Report.Position"].Value);
                TableHeader(table, 3, _loc["Report.TireYear"].Value);
                TableHeader(table, 4, _loc["Report.TireWeek"].Value);
                TableHeader(table, 5, _loc["Report.TireCondition"].Value);

                for (int i = 0; i < stage.Items.Count; i++)
                {
                    var item = stage.Items[i];
                    uint row = (uint)(i + 2);
                    TableCell(table, row, 1, (i + 1).ToString(), true);
                    TableCell(table, row, 2, _loc[$"Report.TirePositions.{item.Position}"].Value);
                    TableCell(table, row, 3, item.Year?.ToString() ?? "—", true);
                    TableCell(table, row, 4, item.Week?.ToString() ?? "—", true);
                    TableCell(table, row, 5, GetTireCondition(item.Condition));
                }
            });
        }

        ComposeComments(col, stage.Comments);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Stage 9: Road Test
    // ═══════════════════════════════════════════════════════════════════════════

    private void ComposeRoadTestStage(ColumnDescriptor col, RoadTestStageReportDto stage, string lang)
    {
        col.Item().Element(c => SectionTitle(c, _loc["Report.Stages.RoadTest"].Value, true));

        if (stage.NoIssuesFound)
            col.Item().Element(c => NoIssuesBadge(c));

        if (stage.Items.Count > 0)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(35);  // #
                    c.RelativeColumn();     // Issue Type
                    c.RelativeColumn();     // Issue
                });

                TableHeader(table, 1, "#");
                TableHeader(table, 2, _loc["Report.IssueType"].Value);
                TableHeader(table, 3, _loc["Report.Issue"].Value);

                for (int i = 0; i < stage.Items.Count; i++)
                {
                    var item = stage.Items[i];
                    uint row = (uint)(i + 2);
                    TableCell(table, row, 1, (i + 1).ToString(), true);
                    TableCell(table, row, 2, N(item.IssueTypeNameAr, item.IssueTypeNameEn, lang));
                    TableCell(table, row, 3, N(item.IssueNameAr, item.IssueNameEn, lang));
                }
            });
        }

        ComposeComments(col, stage.Comments);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Shared UI Components
    // ═══════════════════════════════════════════════════════════════════════════

    private void SectionTitle(IContainer container, string title, bool isStage)
    {
        var borderColor = isStage ? StageBorderColor : InfoBorderColor;
        container
            .Background(HeaderBg)
            .BorderLeft(3).BorderColor(borderColor)
            .Padding(6)
            .Text(title).FontSize(11).Bold();
    }

    private void NoIssuesBadge(IContainer container)
    {
        container
            .Background(NoIssuesBg)
            .Padding(5)
            .Text($"✓ {_loc["Report.NoIssues"].Value}")
            .FontSize(9).Bold().FontColor(NoIssuesText);
    }

    private void ComposeComments(ColumnDescriptor col, string? comments)
    {
        if (string.IsNullOrWhiteSpace(comments)) return;
        col.Item()
            .Background(CommentsBg)
            .Padding(8)
            .Text(text =>
            {
                text.Span($"{_loc["Report.Comments"].Value}: ").Bold().FontSize(9);
                text.Span(comments).FontSize(9).FontColor("#495057");
            });
    }

    private static void TableHeader(TableDescriptor table, uint column, string text)
    {
        table.Cell().Row(1).Column(column)
            .Background(HeaderBg)
            .Border(1).BorderColor(BorderColor)
            .Padding(5)
            .Text(text).FontSize(8).Bold();
    }

    private static void TableCell(TableDescriptor table, uint row, uint column, string text, bool center = false)
    {
        var cell = table.Cell().Row(row).Column(column)
            .Border(1).BorderColor(BorderColor)
            .Padding(5);

        if (center)
            cell.AlignCenter().Text(text).FontSize(9);
        else
            cell.Text(text).FontSize(9);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // Label Resolvers
    // ═══════════════════════════════════════════════════════════════════════════

    private string GetTireCondition(string? condition)
    {
        if (condition is null) return "—";
        var result = _loc[$"Report.TireConditions.{condition}"];
        return result.ResourceNotFound ? condition : result.Value;
    }

    private string GetEvaluation(string evaluation)
    {
        var result = _loc[$"Report.Evaluations.{evaluation}"];
        return result.ResourceNotFound ? evaluation : result.Value;
    }

    private static string GetPlateDisplay(ExaminationDto exam)
    {
        var letters = exam.PlateLetters ?? "";
        var numbers = exam.PlateNumbers ?? "";
        if (string.IsNullOrWhiteSpace(letters) && string.IsNullOrWhiteSpace(numbers))
            return "—";
        return $"{letters} {numbers}".Trim();
    }

    /// <summary>Pick the name based on language, falling back to Arabic.</summary>
    private static string N(string nameAr, string nameEn, string lang)
        => lang == "ar" ? nameAr : (!string.IsNullOrWhiteSpace(nameEn) ? nameEn : nameAr);
}
