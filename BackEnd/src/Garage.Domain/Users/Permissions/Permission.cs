namespace Garage.Domain.Users.Permissions;
public static class Permission
{
    public const string ClaimType = "permission";
    public const string Users_Read   = "users.read";
    public const string Users_Create = "users.create";
    public const string Users_Update = "users.update";
    public const string Users_Delete = "users.delete";
    public const string Roles_Read   = "roles.read";
    public const string Roles_Assign = "roles.assign";
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


    public const string Employees_Read = "employees.read";
    public const string Employees_Create = "employees.create";
    public const string Employees_Update = "employees.update";
    public const string Employees_Delete = "employees.delete";

    public const string Dashbord_View = "dashboard.view";


    public static readonly string[] All =
    [
        Users_Read, Users_Create, Users_Update, Users_Delete,
        Roles_Read, Roles_Assign, Permissions_Read,
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
        Dashbord_View
    ];
}

