using AgileBoard.Domain.Common;
using AgileBoard.Domain.Constants;
using AgileBoard.Domain.Entities;
using AgileBoard.Infrastructure.Repositories.Interfaces;
using AgileBoard.Services.Services.Interfaces;

namespace AgileBoard.Services.Services.Implementations
{
    public class SprintService(ISprintRepository sprintRepository) : ISprintService
    {
        private readonly ISprintRepository _sprintRepository = sprintRepository;

        public async Task<Result<IEnumerable<Sprint>>> GetAllSprintsAsync()
        {
            var sprints = await _sprintRepository.GetAllSprintsAsync();
            if (!sprints.Any())
                return Result<IEnumerable<Sprint>>.NotFound(Messages.EntityNames.Sprints, isPlural: true);

            return Result<IEnumerable<Sprint>>.Success(sprints);
        }

        public async Task<Result<Sprint>> GetSprintByIdAsync(int id)
        {
            var sprint = await _sprintRepository.GetSprintByIdAsync(id);
            if (sprint == null)
                return Result<Sprint>.NotFound(Messages.EntityNames.Sprint);

            return Result<Sprint>.Success(sprint);
        }

        public async Task<Result<Sprint>> GetSprintByNameAsync(string name, int projectId)
        {
            var sprint = await _sprintRepository.GetSprintByNameAsync(name, projectId);
            if (sprint == null)
                return Result<Sprint>.NotFound(Messages.EntityNames.Sprint);

            return Result<Sprint>.Success(sprint);
        }

        public async Task<Result<IEnumerable<Sprint>>> GetSprintsByProjectIdAsync(int projectId)
        {
            var sprints = await _sprintRepository.GetSprintsByProjectIdAsync(projectId);
            if (!sprints.Any())
                return Result<IEnumerable<Sprint>>.NotFound(Messages.Sprints.NoSprintsFoundForProject);

            return Result<IEnumerable<Sprint>>.Success(sprints);
        }

        public async Task<Result<IEnumerable<Sprint>>> GetActiveSprintsAsync()
        {
            var sprints = await _sprintRepository.GetActiveSprintsAsync();
            return Result<IEnumerable<Sprint>>.Success(sprints);
        }

        public async Task<Result<IEnumerable<Sprint>>> GetSprintsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
                return Result<IEnumerable<Sprint>>.BadRequest("Start date must be before end date.");

            var sprints = await _sprintRepository.GetSprintsByDateRangeAsync(startDate, endDate);
            return Result<IEnumerable<Sprint>>.Success(sprints);
        }

        public async Task<Result<Sprint>> CreateSprintAsync(string name, string? description, int projectId, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Sprint>.BadRequest(Messages.Sprints.SprintNameRequired);

            if (startDate >= endDate)
                return Result<Sprint>.BadRequest(Messages.Sprints.EndDateMustBeAfterStartDate);

            if (startDate < DateTime.UtcNow.Date)
                return Result<Sprint>.BadRequest(Messages.Sprints.SprintCannotStartInPast);

            var existingSprint = await _sprintRepository.GetSprintByNameAsync(name, projectId);
            if (existingSprint != null)
                return Result<Sprint>.Conflict(Messages.Sprints.SprintNameExists(name));

            try
            {
                var newSprint = new Sprint
                {
                    Name = name,
                    Description = description,
                    ProjectId = projectId,
                    StartDate = startDate,
                    EndDate = endDate
                };

                var createdSprint = await _sprintRepository.AddSprintAsync(newSprint);
                return Result<Sprint>.Success(createdSprint);
            }
            catch (Exception)
            {
                return Result<Sprint>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<Sprint>> UpdateSprintAsync(int id, string? name, string? description, DateTime? startDate, DateTime? endDate)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(description) && !startDate.HasValue && !endDate.HasValue)
                return Result<Sprint>.BadRequest(Messages.SprintUpdate.NoFieldsSpecified);

            var currentSprint = await _sprintRepository.GetSprintByIdAsync(id);
            if (currentSprint == null)
                return Result<Sprint>.NotFound(Messages.EntityNames.Sprint);

            var newStartDate = startDate ?? currentSprint.StartDate;
            var newEndDate = endDate ?? currentSprint.EndDate;

            if (newStartDate >= newEndDate)
                return Result<Sprint>.BadRequest(Messages.Sprints.EndDateMustBeAfterStartDate);

            try
            {
                var updatedSprint = await _sprintRepository.UpdateSprintAsync(id, name, description, startDate, endDate);
                if (updatedSprint == null)
                    return Result<Sprint>.NotFound(Messages.EntityNames.Sprint);

                return Result<Sprint>.Success(updatedSprint);
            }
            catch (Exception)
            {
                return Result<Sprint>.Failure(Messages.Generic.InternalServerError);
            }
        }

        public async Task<Result<bool>> DeleteSprintAsync(int id)
        {
            var sprint = await _sprintRepository.GetSprintByIdAsync(id);
            if (sprint == null)
                return Result<bool>.NotFound(Messages.EntityNames.Sprint);

            var hasWorkItems = await _sprintRepository.HasWorkItemsAsync(id);
            if (hasWorkItems)
                return Result<bool>.BadRequest(Messages.Sprints.CannotDeleteSprintWithWorkItems);

            try
            {
                var deleted = await _sprintRepository.DeleteSprintAsync(id);
                return Result<bool>.Success(deleted);
            }
            catch (Exception)
            {
                return Result<bool>.Failure(Messages.Generic.InternalServerError);
            }
        }
    }
}