using System;

namespace KnowledgeApp.DataAccess.Entities;

public partial class GroupSelectedDisciplines
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int SemesterId { get; set; }

    public int? Discipline1Id { get; set; }

    public int? Discipline1OwnerDepartmentId { get; set; }

    public bool? Discipline1IsOwn { get; set; }

    public int? Discipline2Id { get; set; }

    public int? Discipline2OwnerDepartmentId { get; set; }

    public bool? Discipline2IsOwn { get; set; }

    public int? SelectedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual StudyGroup? Group { get; set; }

    public virtual Semester? Semester { get; set; }

    public virtual Discipline? Discipline1 { get; set; }

    public virtual Discipline? Discipline2 { get; set; }

    public virtual Department? Discipline1OwnerDepartment { get; set; }

    public virtual Department? Discipline2OwnerDepartment { get; set; }

    public virtual User? SelectedByUser { get; set; }
}
