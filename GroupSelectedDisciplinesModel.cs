namespace KnowledgeApp.Core.Models;

public class GroupSelectedDisciplinesModel
{
    // Конструктор для сохранения нового выбора (без Id / department-полей)
    public GroupSelectedDisciplinesModel(
        int groupId,
        int semesterId,
        int? discipline1Id,
        int? discipline2Id,
        int? selectedBy)
    {
        GroupId       = groupId;
        SemesterId    = semesterId;
        Discipline1Id = discipline1Id;
        Discipline2Id = discipline2Id;
        SelectedBy    = selectedBy;
    }

    // Конструктор для чтения полной записи из БД
    public GroupSelectedDisciplinesModel(
        int id,
        int groupId,
        int semesterId,
        int? discipline1Id,
        int? discipline1OwnerDepartmentId,
        bool? discipline1IsOwn,
        int? discipline2Id,
        int? discipline2OwnerDepartmentId,
        bool? discipline2IsOwn,
        int? selectedBy,
        DateTime updatedAt)
    {
        Id                           = id;
        GroupId                      = groupId;
        SemesterId                   = semesterId;
        Discipline1Id                = discipline1Id;
        Discipline1OwnerDepartmentId = discipline1OwnerDepartmentId;
        Discipline1IsOwn             = discipline1IsOwn;
        Discipline2Id                = discipline2Id;
        Discipline2OwnerDepartmentId = discipline2OwnerDepartmentId;
        Discipline2IsOwn             = discipline2IsOwn;
        SelectedBy                   = selectedBy;
        UpdatedAt                    = updatedAt;
    }

    public int      Id                           { get; set; }
    public int      GroupId                      { get; set; }
    public int      SemesterId                   { get; set; }

    public int?     Discipline1Id                { get; set; }
    public int?     Discipline1OwnerDepartmentId { get; set; }
    public bool?    Discipline1IsOwn             { get; set; }

    public int?     Discipline2Id                { get; set; }
    public int?     Discipline2OwnerDepartmentId { get; set; }
    public bool?    Discipline2IsOwn             { get; set; }

    public int?     SelectedBy                   { get; set; }
    public DateTime UpdatedAt                    { get; set; }
}
