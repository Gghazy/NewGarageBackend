using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Examinations;

namespace Domain.ExaminationManagement.Examinations;

public sealed class Examination : AggregateRoot
{
    public ClientReference Client { get; private set; } = default!;
    public BranchReference Branch { get; private set; } = default!;
    public VehicleSnapshot Vehicle { get; private set; } = default!;

    public ExaminationType Type { get; private set; }
    public ExaminationStatus Status { get; private set; }

    public bool HasWarranty { get; private set; }
    public bool HasPhotos   { get; private set; }
    public string? MarketerCode { get; private set; }
    public string? Notes { get; private set; }

    private readonly List<ExaminationItem> _items = new();
    public IReadOnlyCollection<ExaminationItem> Items => _items.AsReadOnly();

    private SensorStageResult? _sensorStageResult;
    public SensorStageResult? SensorStageResult => _sensorStageResult;

    private DashboardIndicatorsStageResult? _dashboardIndicatorsStageResult;
    public DashboardIndicatorsStageResult? DashboardIndicatorsStageResult => _dashboardIndicatorsStageResult;

    private InteriorDecorStageResult? _interiorDecorStageResult;
    public InteriorDecorStageResult? InteriorDecorStageResult => _interiorDecorStageResult;

    private InteriorBodyStageResult? _interiorBodyStageResult;
    public InteriorBodyStageResult? InteriorBodyStageResult => _interiorBodyStageResult;

    private ExteriorBodyStageResult? _exteriorBodyStageResult;
    public ExteriorBodyStageResult? ExteriorBodyStageResult => _exteriorBodyStageResult;

    private Examination() { }

    private Examination(
        ClientReference client,
        BranchReference branch,
        VehicleSnapshot vehicle,
        ExaminationType type,
        bool hasWarranty,
        bool hasPhotos,
        string? marketerCode)
    {
        Client = client;
        Branch = branch;
        Vehicle = vehicle;

        Type        = type;
        HasWarranty = hasWarranty;
        HasPhotos   = hasPhotos;
        MarketerCode = Normalize(marketerCode);

        Status = ExaminationStatus.Draft;
    }

    public static Examination Create(
        ClientReference client,
        BranchReference branch,
        VehicleSnapshot vehicle,
        ExaminationType type,
        bool hasWarranty,
        bool hasPhotos,
        string? marketerCode = null)
    {
        if (client.ClientId == Guid.Empty)  throw new DomainException("Client is required.");

        return new Examination(client, branch, vehicle, type, hasWarranty, hasPhotos, marketerCode);
    }

    public void SetNotes(string? notes) => Notes = Normalize(notes);

    public void Update(bool hasWarranty, bool hasPhotos, string? marketerCode, string? notes)
    {
        EnsureEditable();
        HasWarranty  = hasWarranty;
        HasPhotos    = hasPhotos;
        MarketerCode = Normalize(marketerCode);
        Notes        = Normalize(notes);
    }

    public void UpdateClientSnapshot(ClientReference clientRef)
    {
        EnsureEditable();
        if (clientRef.ClientId == Guid.Empty) throw new DomainException("Client is required.");
        Client = clientRef;
    }

    public void UpdateVehicleSnapshot(VehicleSnapshot snapshot)
    {
        EnsureEditable();
        Vehicle = snapshot;
    }

    public void UpdateBranchSnapshot(BranchReference branchRef)
    {
        EnsureEditable();
        Branch = branchRef;
    }

    public void AddItem(ServiceSnapshot service, int quantity = 1, decimal? overridePrice = null)
    {
        EnsureEditable();

        if (service.ServiceId == Guid.Empty) throw new DomainException("Service is required.");
        if (_items.Any(x => x.Service.ServiceId == service.ServiceId))
            throw new DomainException("Service already added.");

        _items.Add(new ExaminationItem(service, quantity, overridePrice));
    }

    public void RemoveItem(Guid serviceId)
    {
        EnsureEditable();

        var item = _items.FirstOrDefault(x => x.Service.ServiceId == serviceId);
        if (item is null) return;

        _items.Remove(item);
    }

    // ── Sensor Stage ─────────────────────────────────────────────────────

    public void SaveSensorStage(
        bool noIssuesFound,
        int cylinderCount,
        string? comments,
        IEnumerable<(Guid IssueId, string Evaluation)> issues)
    {
        EnsureEditable();

        if (noIssuesFound && issues.Any())
            throw new DomainException("Cannot add issues when NoIssuesFound is true.");

        if (_sensorStageResult is null)
        {
            _sensorStageResult = SensorStageResult.Create(
                Id,
                noIssuesFound,
                cylinderCount,
                comments);
        }
        else
        {
            _sensorStageResult.Update(noIssuesFound, cylinderCount, comments);
            _sensorStageResult.ClearIssues();
        }

        foreach (var issue in issues)
            _sensorStageResult.AddIssue(issue.IssueId, issue.Evaluation);
    }

    // ── Dashboard Indicators Stage ────────────────────────────────────────

    public void SaveDashboardIndicatorsStage(
        string? comments,
        IEnumerable<(string Key, decimal? Value, bool NotApplicable)> indicators)
    {
        EnsureEditable();

        if (_dashboardIndicatorsStageResult is null)
        {
            _dashboardIndicatorsStageResult = DashboardIndicatorsStageResult.Create(Id, comments);
        }
        else
        {
            _dashboardIndicatorsStageResult.Update(comments);
            _dashboardIndicatorsStageResult.ClearItems();
        }

        foreach (var ind in indicators)
            _dashboardIndicatorsStageResult.AddIndicator(ind.Key, ind.Value, ind.NotApplicable);
    }

    // ── Interior Decor Stage ───────────────────────────────────────────

    public void SaveInteriorDecorStage(
        bool noIssuesFound,
        string? comments,
        IEnumerable<(Guid PartId, Guid IssueId)> items)
    {
        EnsureEditable();

        if (_interiorDecorStageResult is null)
        {
            _interiorDecorStageResult = InteriorDecorStageResult.Create(Id, noIssuesFound, comments);
        }
        else
        {
            _interiorDecorStageResult.Update(noIssuesFound, comments);
            _interiorDecorStageResult.ClearItems();
        }

        foreach (var item in items)
            _interiorDecorStageResult.AddItem(item.PartId, item.IssueId);
    }

    // ── Interior Body Stage ─────────────────────────────────────────────

    public void SaveInteriorBodyStage(
        bool noIssuesFound,
        string? comments,
        IEnumerable<(Guid PartId, Guid IssueId)> items)
    {
        EnsureEditable();

        if (_interiorBodyStageResult is null)
        {
            _interiorBodyStageResult = InteriorBodyStageResult.Create(Id, noIssuesFound, comments);
        }
        else
        {
            _interiorBodyStageResult.Update(noIssuesFound, comments);
            _interiorBodyStageResult.ClearItems();
        }

        foreach (var item in items)
            _interiorBodyStageResult.AddItem(item.PartId, item.IssueId);
    }

    // ── Exterior Body Stage ─────────────────────────────────────────────

    public void SaveExteriorBodyStage(
        bool noIssuesFound,
        string? comments,
        IEnumerable<(Guid PartId, Guid IssueId)> items)
    {
        EnsureEditable();

        if (_exteriorBodyStageResult is null)
        {
            _exteriorBodyStageResult = ExteriorBodyStageResult.Create(Id, noIssuesFound, comments);
        }
        else
        {
            _exteriorBodyStageResult.Update(noIssuesFound, comments);
            _exteriorBodyStageResult.ClearItems();
        }

        foreach (var item in items)
            _exteriorBodyStageResult.AddItem(item.PartId, item.IssueId);
    }

    // ── Status transitions ──────────────────────────────────────────────

    public void Start()
    {
        EnsureDraft();
        if (_items.Count == 0)
            throw new DomainException("Cannot start examination without items.");

        Status = ExaminationStatus.Pending;
    }

    public void BeginWork()
    {
        if (Status != ExaminationStatus.Pending)
            throw new DomainException("Only Pending examination can begin work.");

        Status = ExaminationStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != ExaminationStatus.InProgress)
            throw new DomainException("Only InProgress examination can be completed.");

        Status = ExaminationStatus.Completed;
    }

    public void Deliver()
    {
        if (Status != ExaminationStatus.Completed)
            throw new DomainException("Only Completed examination can be delivered.");

        Status = ExaminationStatus.Delivered;
    }

    public void Cancel(string? reason = null)
    {
        if (Status == ExaminationStatus.Delivered)
            throw new DomainException("Delivered examination cannot be cancelled.");

        Status = ExaminationStatus.Cancelled;
        Notes = Normalize(reason);
    }

    private void EnsureEditable()
    {
        if (Status != ExaminationStatus.Draft && Status != ExaminationStatus.Pending && Status != ExaminationStatus.InProgress)
            throw new DomainException("Examination can only be updated in Draft, Pending or InProgress status.");
    }

    private void EnsureDraft()
    {
        if (Status != ExaminationStatus.Draft)
            throw new DomainException("You can modify examination only in Draft status.");
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
