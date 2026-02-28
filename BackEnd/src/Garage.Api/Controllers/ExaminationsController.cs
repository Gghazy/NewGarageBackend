using Garage.Api.Controllers.Common;
using Garage.Application.Examinations.Commands.ChangeStatus;
using Garage.Application.Examinations.Commands.Create;
using Garage.Application.Examinations.Commands.Delete;
using Garage.Application.Examinations.Commands.Update;
using Garage.Application.Examinations.Queries.GetAll;
using Garage.Application.Examinations.Queries.GetById;
using Garage.Application.Examinations.Queries.CanComplete;
using Garage.Application.Examinations.Queries.GetHistory;
using Garage.Application.Examinations.Queries.GetReport;
using Garage.Application.Examinations.Queries.GetCount;
using Garage.Application.Examinations.Queries.GetServiceUsage;
using Garage.Application.Examinations.Queries.GetSensorStage;
using Garage.Application.Examinations.Queries.GetDashboardIndicatorsStage;
using Garage.Application.Examinations.Queries.GetWorkflow;
using Garage.Application.Examinations.Commands.SaveSensorStage;
using Garage.Application.Examinations.Commands.SaveDashboardIndicatorsStage;
using Garage.Application.Examinations.Commands.SaveInteriorDecorStage;
using Garage.Application.Examinations.Commands.SaveInteriorBodyStage;
using Garage.Application.Examinations.Commands.SaveExteriorBodyStage;
using Garage.Application.Examinations.Queries.GetInteriorDecorStage;
using Garage.Application.Examinations.Queries.GetInteriorBodyStage;
using Garage.Application.Examinations.Queries.GetExteriorBodyStage;
using Garage.Application.Examinations.Commands.SaveTireStage;
using Garage.Application.Examinations.Queries.GetTireStage;
using Garage.Application.Examinations.Commands.SaveAccessoryStage;
using Garage.Application.Examinations.Queries.GetAccessoryStage;
using Garage.Application.Examinations.Commands.SaveMechanicalStage;
using Garage.Application.Examinations.Queries.GetMechanicalStage;
using Garage.Application.Examinations.Commands.SaveRoadTestStage;
using Garage.Application.Examinations.Queries.GetRoadTestStage;
using Garage.Application.Abstractions;
using Garage.Contracts.Common;
using Garage.Contracts.Examinations;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Garage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExaminationsController : ApiControllerBase
{
    private readonly IMediator _mediator;
    private readonly IExaminationReportPdfService _pdfService;

    public ExaminationsController(IMediator mediator, IStringLocalizer localizer, IExaminationReportPdfService pdfService)
        : base(localizer)
    {
        _mediator = mediator;
        _pdfService = pdfService;
    }

    [HttpPost("pagination")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await _mediator.Send(new GetAllExaminationsQuery(search));
        return Success(result);
    }

    [HttpGet("{id:Guid}")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetExaminationByIdQuery(id));
        if (result is null) return NotFound();
        return Success(result);
    }

    [HttpGet("{id:Guid}/can-complete")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> CanComplete(Guid id)
    {
        var result = await _mediator.Send(new CanCompleteExaminationQuery(id));
        return Success(result);
    }

    [HttpGet("{id:Guid}/report-data")]
    [HasPermission(Permission.Examination_Report)]
    public async Task<IActionResult> GetReportData(Guid id)
    {
        var result = await _mediator.Send(new GetExaminationReportQuery(id));
        if (result is null) return NotFound();
        return Success(result);
    }

    [HttpGet("{id:Guid}/report-pdf")]
    [HasPermission(Permission.Examination_Report)]
    public async Task<IActionResult> GetReportPdf(Guid id)
    {
        var report = await _mediator.Send(new GetExaminationReportQuery(id));
        if (report is null) return NotFound();
        var bytes = _pdfService.Generate(report);
        return File(bytes, "application/pdf", $"examination-report-{id}.pdf");
    }

    [HttpPost]
    [HasPermission(Permission.Examination_Create)]
    public async Task<IActionResult> Create(CreateExaminationRequest request)
    {
        var result = await _mediator.Send(new CreateExaminationCommand(request));
        return HandleResult(result, "Examination.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Update(Guid id, UpdateExaminationRequest request)
    {
        var result = await _mediator.Send(new UpdateExaminationCommand(id, request));
        return HandleResult(result, "Examination.Updated");
    }


    [HttpPost("{id:Guid}/start")]
    [HasPermission(Permission.Examination_Start)]
    public async Task<IActionResult> Start(Guid id)
    {
        var result = await _mediator.Send(new StartExaminationCommand(id));
        return HandleResult(result, "Examination.Started");
    }

    [HttpPost("{id:Guid}/begin-work")]
    [HasPermission(Permission.Examination_Start)]
    public async Task<IActionResult> BeginWork(Guid id)
    {
        var result = await _mediator.Send(new BeginWorkExaminationCommand(id));
        return HandleResult(result, "Examination.BeganWork");
    }

    [HttpPost("{id:Guid}/complete")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Complete(Guid id)
    {
        var result = await _mediator.Send(new CompleteExaminationCommand(id));
        return HandleResult(result, "Examination.Completed");
    }

    [HttpPost("{id:Guid}/deliver")]
    [HasPermission(Permission.Examination_Deliver)]
    public async Task<IActionResult> Deliver(Guid id)
    {
        var result = await _mediator.Send(new DeliverExaminationCommand(id));
        return HandleResult(result, "Examination.Delivered");
    }

    [HttpPost("{id:Guid}/reopen")]
    [HasPermission(Permission.Examination_Reopen)]
    public async Task<IActionResult> Reopen(Guid id)
    {
        var result = await _mediator.Send(new ReopenExaminationCommand(id));
        return HandleResult(result, "Examination.Reopened");
    }

    [HttpPost("{id:Guid}/cancel")]
    [HasPermission(Permission.Examination_Cancel)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string? reason = null)
    {
        var result = await _mediator.Send(new CancelExaminationCommand(id, reason));
        return HandleResult(result, "Examination.Cancelled");
    }


    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.Examination_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteExaminationCommand(id));
        return HandleResult(result, "Examination.Deleted");
    }

    [HttpGet("{id:Guid}/history")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetHistory(Guid id)
    {
        var result = await _mediator.Send(new GetExaminationHistoryQuery(id));
        return Success(result);
    }

    [HttpGet("{id:Guid}/workflow")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetWorkflow(Guid id)
    {
        var result = await _mediator.Send(new GetExaminationWorkflowQuery(id));
        if (result is null) return NotFound();
        return Success(result);
    }

    [HttpPost("{id:Guid}/stages/sensors")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> SaveSensorStage(Guid id, SaveSensorStageRequest request)
    {
        var result = await _mediator.Send(new SaveSensorStageCommand(id, request));
        return HandleResult(result, "Examination.SensorStageSaved");
    }

    [HttpGet("{id:Guid}/stages/sensors")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetSensorStage(Guid id)
    {
        var result = await _mediator.Send(new GetSensorStageQuery(id));
        return Success(result);
    }

    [HttpPost("{id:Guid}/stages/dashboard-indicators")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> SaveDashboardIndicatorsStage(Guid id, SaveDashboardIndicatorsStageRequest request)
    {
        var result = await _mediator.Send(new SaveDashboardIndicatorsStageCommand(id, request));
        return HandleResult(result, "Examination.DashboardIndicatorsStageSaved");
    }

    [HttpGet("{id:Guid}/stages/dashboard-indicators")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetDashboardIndicatorsStage(Guid id)
    {
        var result = await _mediator.Send(new GetDashboardIndicatorsStageQuery(id));
        return Success(result);
    }

    [HttpPost("{id:Guid}/stages/interior-decor")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> SaveInteriorDecorStage(Guid id, SaveInteriorDecorStageRequest request)
    {
        var result = await _mediator.Send(new SaveInteriorDecorStageCommand(id, request));
        return HandleResult(result, "Examination.InteriorDecorStageSaved");
    }

    [HttpGet("{id:Guid}/stages/interior-decor")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetInteriorDecorStage(Guid id)
    {
        var result = await _mediator.Send(new GetInteriorDecorStageQuery(id));
        return Success(result);
    }

    [HttpPost("{id:Guid}/stages/interior-body")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> SaveInteriorBodyStage(Guid id, SaveInteriorBodyStageRequest request)
    {
        var result = await _mediator.Send(new SaveInteriorBodyStageCommand(id, request));
        return HandleResult(result, "Examination.InteriorBodyStageSaved");
    }

    [HttpGet("{id:Guid}/stages/interior-body")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetInteriorBodyStage(Guid id)
    {
        var result = await _mediator.Send(new GetInteriorBodyStageQuery(id));
        return Success(result);
    }

    [HttpPost("{id:Guid}/stages/exterior-body")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> SaveExteriorBodyStage(Guid id, SaveExteriorBodyStageRequest request)
    {
        var result = await _mediator.Send(new SaveExteriorBodyStageCommand(id, request));
        return HandleResult(result, "Examination.ExteriorBodyStageSaved");
    }

    [HttpGet("{id:Guid}/stages/exterior-body")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetExteriorBodyStage(Guid id)
    {
        var result = await _mediator.Send(new GetExteriorBodyStageQuery(id));
        return Success(result);
    }

    [HttpPost("{id:Guid}/stages/tires")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> SaveTireStage(Guid id, SaveTireStageRequest request)
    {
        var result = await _mediator.Send(new SaveTireStageCommand(id, request));
        return HandleResult(result, "Examination.TireStageSaved");
    }

    [HttpGet("{id:Guid}/stages/tires")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetTireStage(Guid id)
    {
        var result = await _mediator.Send(new GetTireStageQuery(id));
        return Success(result);
    }

    [HttpPost("{id:Guid}/stages/accessories")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> SaveAccessoryStage(Guid id, SaveAccessoryStageRequest request)
    {
        var result = await _mediator.Send(new SaveAccessoryStageCommand(id, request));
        return HandleResult(result, "Examination.AccessoryStageSaved");
    }

    [HttpGet("{id:Guid}/stages/accessories")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetAccessoryStage(Guid id)
    {
        var result = await _mediator.Send(new GetAccessoryStageQuery(id));
        return Success(result);
    }

    [HttpPost("{id:Guid}/stages/mechanical")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> SaveMechanicalStage(Guid id, SaveMechanicalStageRequest request)
    {
        var result = await _mediator.Send(new SaveMechanicalStageCommand(id, request));
        return HandleResult(result, "Examination.MechanicalStageSaved");
    }

    [HttpGet("{id:Guid}/stages/mechanical")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetMechanicalStage(Guid id)
    {
        var result = await _mediator.Send(new GetMechanicalStageQuery(id));
        return Success(result);
    }

    [HttpPost("{id:Guid}/stages/road-test")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> SaveRoadTestStage(Guid id, SaveRoadTestStageRequest request)
    {
        var result = await _mediator.Send(new SaveRoadTestStageCommand(id, request));
        return HandleResult(result, "Examination.RoadTestStageSaved");
    }

    [HttpGet("{id:Guid}/stages/road-test")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetRoadTestStage(Guid id)
    {
        var result = await _mediator.Send(new GetRoadTestStageQuery(id));
        return Success(result);
    }

    [HttpGet("count")]
    [HasPermission(Permission.Dashboard_Examinations)]
    public async Task<IActionResult> GetCount([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branchId)
    {
        var result = await _mediator.Send(new GetExaminationsCountQuery(from, to, branchId));
        return Success(result);
    }

    [HttpGet("service-usage")]
    [HasPermission(Permission.Dashboard_Examinations)]
    public async Task<IActionResult> GetServiceUsage([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branchId)
    {
        var result = await _mediator.Send(new GetServiceUsageQuery(from, to, branchId));
        return Success(result);
    }
}
