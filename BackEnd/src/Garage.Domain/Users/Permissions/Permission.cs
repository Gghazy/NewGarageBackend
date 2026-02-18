namespace Garage.Domain.Users.Permissions;
public static class Permission
{
    public const string ClaimType = "permission";
    public const string Users_Read   = "users.read";
    public const string Users_Create = "users.create";
    public const string Users_Update = "users.update";
    public const string Users_Delete = "users.delete";

    public const string Roles_Read   = "roles.read";
    public const string Roles_Create = "roles.create";
    public const string Roles_Update = "roles.update";

    public const string Permissions_Read = "permissions.read";

    public const string Branches_Read   = "branches.read";
    public const string Branches_Create = "branches.create";
    public const string Branches_Update = "branches.update";
    public const string Branches_Delete = "branches.delete";

    public const string SensorIssue_Read   = "sensorIssue.read";
    public const string SensorIssue_Create = "sensorIssue.create";
    public const string SensorIssue_Update = "sensorIssue.update";
    public const string SensorIssue_Delete = "sensorIssue.delete";


    public const string MechIssue_Read   = "mechIssue.read";
    public const string MechIssue_Create = "mechIssue.create";
    public const string MechIssue_Update = "mechIssue.update";
    public const string MechIssue_Delete = "mechIssue.delete";


    public const string MechIssueType_Read   = "mechIssueType.read";
    public const string MechIssueType_Create = "mechIssueType.create";
    public const string MechIssueType_Update = "mechIssueType.update";
    public const string MechIssueType_Delete = "mechIssueType.delete";

    public const string InteriorIssue_Read   = "interiorIssue.read";
    public const string InteriorIssue_Create = "interiorIssue.create";
    public const string InteriorIssue_Update = "interiorIssue.update";
    public const string InteriorIssue_Delete = "interiorIssue.delete";

    public const string InteriorBodyIssue_Read   = "interiorBodyIssue.read";
    public const string InteriorBodyIssue_Create = "interiorBodyIssue.create";
    public const string InteriorBodyIssue_Update = "interiorBodyIssue.update";
    public const string InteriorBodyIssue_Delete = "interiorBodyIssue.delete";

    public const string ExteriorBodyIssue_Read = "exteriorBodyIssue.read";
    public const string ExteriorBodyIssue_Create = "exteriorBodyIssue.create";
    public const string ExteriorBodyIssue_Update = "exteriorBodyIssue.update";
    public const string ExteriorBodyIssue_Delete = "exteriorBodyIssue.delete";

    public const string AccessoryIssue_Read   = "accessoryIssue.read";
    public const string AccessoryIssue_Create = "accessoryIssue.create";
    public const string AccessoryIssue_Update = "accessoryIssue.update";
    public const string AccessoryIssue_Delete = "accessoryIssue.delete";

    public const string RoadTestIssue_Read   = "roadTestIssue.read";
    public const string RoadTestIssue_Create = "roadTestIssue.create";
    public const string RoadTestIssue_Update = "roadTestIssue.update";
    public const string RoadTestIssue_Delete = "roadTestIssue.delete";

    public const string InsideAndDecorPart_Read   = "insideAndDecorPart.read";
    public const string InsideAndDecorPart_Create = "insideAndDecorPart.create";
    public const string InsideAndDecorPart_Update = "insideAndDecorPart.update";
    public const string InsideAndDecorPart_Delete = "insideAndDecorPart.delete";


    public const string CarMark_Read   = "carMark.read";
    public const string CarMark_Create = "carMark.create";
    public const string CarMark_Update = "carMark.update";
    public const string CarMark_Delete = "carMark.delete";


    public const string Manufacturer_Read   = "manufacturer.read";
    public const string Manufacturer_Create = "manufacturer.create";
    public const string Manufacturer_Update = "manufacturer.update";
    public const string Manufacturer_Delete = "manufacturer.delete";

    public const string ServiceType_Read   = "serviceType.read";
    public const string ServiceType_Create = "serviceType.create";
    public const string ServiceType_Update = "serviceType.update";
    public const string ServiceType_Delete = "serviceType.delete";

    public const string Service_Read   = "service.read";
    public const string Service_Create = "service.create";
    public const string Service_Update = "service.update";
    public const string Service_Delete = "service.delete";

    public const string Crane_Read   = "crane.read";
    public const string Crane_Create = "crane.create";
    public const string Crane_Update = "crane.update";
    public const string Crane_Delete = "crane.delete";

    public const string Term_Read   = "term.read";
    public const string Term_Create = "term.create";
    public const string Term_Update = "term.update";
    public const string Term_Delete = "term.delete";

    public const string ServicePrice_Read   = "servicePrice.read";
    public const string ServicePrice_Create = "servicePrice.create";
    public const string ServicePrice_Update = "servicePrice.update";
    public const string ServicePrice_Delete = "servicePrice.delete";



    public const string Client_Read   = "clients.read";
    public const string Client_Create = "clients.create";
    public const string Client_Update = "clients.update";
    public const string Client_Delete = "clients.delete";



    public const string ClientResource_Read   = "clientResource.read";
    public const string ClientResource_Create = "clientResource.create";
    public const string ClientResource_Update = "clientResource.update";
    public const string ClientResource_Delete = "clientResource.delete";




    public const string Employees_Read = "employees.read";
    public const string Employees_Create = "employees.create";
    public const string Employees_Update = "employees.update";
    public const string Employees_Delete = "employees.delete";

    public const string Dashbord_Read = "dashboard.read";


    public static readonly string[] All =
    [
        Users_Read, Users_Create, Users_Update, Users_Delete,
        Roles_Read, Roles_Create, Roles_Update,
        Permissions_Read,
        Branches_Read, Branches_Create, Branches_Update, Branches_Delete,
        Employees_Create, Employees_Read, Employees_Update, Employees_Delete,
        SensorIssue_Create, SensorIssue_Update, SensorIssue_Delete,SensorIssue_Read,
        MechIssue_Read, MechIssue_Create,MechIssue_Update, MechIssue_Delete,
        MechIssueType_Read, MechIssueType_Create,MechIssueType_Update, MechIssueType_Delete,
        InteriorIssue_Read, InteriorIssue_Create,InteriorIssue_Update, InteriorIssue_Delete,
        InteriorBodyIssue_Read, InteriorBodyIssue_Create,InteriorBodyIssue_Update, InteriorBodyIssue_Delete,
        ExteriorBodyIssue_Read, ExteriorBodyIssue_Create,ExteriorBodyIssue_Update, ExteriorBodyIssue_Delete,
        AccessoryIssue_Read, AccessoryIssue_Create,AccessoryIssue_Update, AccessoryIssue_Delete,
        RoadTestIssue_Read, RoadTestIssue_Create,RoadTestIssue_Update, RoadTestIssue_Delete,
        InsideAndDecorPart_Read, InsideAndDecorPart_Create,InsideAndDecorPart_Update, InsideAndDecorPart_Delete,
        CarMark_Read, CarMark_Create,CarMark_Update, CarMark_Delete,
        Manufacturer_Read, Manufacturer_Create,Manufacturer_Update, Manufacturer_Delete,
        ServiceType_Read, ServiceType_Create,ServiceType_Update, ServiceType_Delete,
        Service_Read, Service_Create,Service_Update, Service_Delete,
        ServicePrice_Create, ServicePrice_Read, ServicePrice_Update, ServicePrice_Delete,
        Crane_Read, Crane_Create,Crane_Update, Crane_Delete,
        Term_Read, Term_Create,Term_Update, Term_Delete,
        Client_Read, Client_Create,Client_Update, Client_Delete,
        ClientResource_Read, ClientResource_Create,ClientResource_Update, ClientResource_Delete,
        Dashbord_Read
    ];
}

