namespace KnowledgeApp.Core.Models;

public class GroupPeriodStatusModel
{
    public GroupPeriodStatusModel(
        int groupId,
        int semesterId,
        bool status,
        int? updatedBy)
    {
        GroupId    = groupId;
        SemesterId = semesterId;
        Status     = status;
        UpdatedBy  = updatedBy;
    }

    public GroupPeriodStatusModel(
        int id,
        int groupId,
        int semesterId,
        bool status,
        int? updatedBy,
        DateTime updatedAt)
    {
        Id         = id;
        GroupId    = groupId;
        SemesterId = semesterId;
        Status     = status;
        UpdatedBy  = updatedBy;
        UpdatedAt  = updatedAt;
    }

    public int      Id         { get; set; }
    public int      GroupId    { get; set; }
    public int      SemesterId { get; set; }
    public bool     Status     { get; set; }
    public int?     UpdatedBy  { get; set; }
    public DateTime UpdatedAt  { get; set; }
}
