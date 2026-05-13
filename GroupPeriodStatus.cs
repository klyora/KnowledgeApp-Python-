using System;

namespace KnowledgeApp.DataAccess.Entities;

public partial class GroupPeriodStatus
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int SemesterId { get; set; }

    public bool Status { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual StudyGroup? Group { get; set; }

    public virtual Semester? Semester { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
