namespace AgileBoard.API.DTOs
{
    public record SprintDTO
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public int ProjectId { get; init; }
        public string ProjectName { get; init; } = string.Empty;
        public IEnumerable<WorkItemSummaryDTO>? WorkItems { get; init; }
        public int WorkItemCount { get; init; }
        public bool IsActive { get; init; }
        public int DurationInDays { get; init; }
    }

    public record SprintSummaryDTO
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public int ProjectId { get; init; }
        public int WorkItemCount { get; init; }
        public bool IsActive { get; init; }
        public int DurationInDays { get; init; }
    }

    public record CreateSprintDTO(string Name, string? Description, int ProjectId, DateTime StartDate, DateTime EndDate);
    
    public record UpdateSprintDTO(string? Name, string? Description, DateTime? StartDate, DateTime? EndDate);
}
