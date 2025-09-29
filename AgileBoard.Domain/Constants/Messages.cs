namespace AgileBoard.Domain.Constants
{
    public static class Messages
    {
        public static class Generic
        {
            public const string InternalServerError = "An internal server error occurred. Please try again later.";
            public const string BadRequestGeneric = "The request is invalid. Please check your input.";
            public const string UnauthorizedGeneric = "You are not authorized to perform this action.";
            public const string NotFoundGeneric = "The requested resource was not found.";
            
            public static string NotFound(string entityName) => $"Could not find {entityName}.";
            public static string NotFoundPlural(string entityName) => $"No {entityName} found.";
        }

        public static class Authentication
        {
            public const string InvalidCredentials = "Invalid username or password.";
            public const string LoginSuccessful = "Login successful";
            public const string LoginErrorGeneric = "An error occurred during login.";
        }

        public static class Registration
        {
            public const string UserCreatedSuccessfully = "User created successfully.";
            public const string RegistrationErrorGeneric = "An error occurred while registering the user.";
            public const string UsernamePasswordEmailRequired = "Username, email, and password must be provided.";
            public const string UsernameAlreadyExists = "Username already exists.";
            public const string EmailAlreadyExists = "Email already exists.";
            public const string TagnameAlreadyExists = "Tag name already exists.";
            public const string ProjectNameAlreadyExists = "Project name already exists.";
        }

        public static class UserUpdate
        {
            public const string UserUpdatedSuccessfully = "User updated successfully.";
            public const string UpdateErrorGeneric = "An error occurred while updating the user.";
            public const string AtLeastOneFieldRequired = "At least one of username or email must be provided for update.";
            public const string NoPermissionToUpdate = "You do not have permission to update this user.";
        }

        public static class PasswordChange
        {
            public const string PasswordChangedSuccessfully = "Password changed successfully.";
            public const string PasswordChangeErrorGeneric = "An error occurred while changing the password.";
            public const string CurrentPasswordIncorrect = "Current password is incorrect.";
            public const string NewPasswordMustBeDifferent = "New password must be different from the current password.";
            public const string PasswordMinimumLength = "New password must be at least 6 characters long.";
            public const string NoPermissionToChangePassword = "You do not have permission to change this password.";
        }

        public static class Tags
        {
            public const string TagNameRequired = "Tag name is required.";
            public const string TagCreatedSuccessfully = "Tag created successfully.";
            public const string TagUpdatedSuccessfully = "Tag updated successfully.";
            public const string TagDeletedSuccessfully = "Tag deleted successfully.";
        }

        public static class TagUpdate
        {
            public const string NoNameSpecified = "Tag name must be provided for update.";
        }

        public static class Projects
        {
            public const string ProjectNameRequired = "Project name is required.";
            public const string ProjectCreatedSuccessfully = "Project created successfully.";
            public const string ProjectUpdatedSuccessfully = "Project updated successfully.";
            public const string ProjectDeletedSuccessfully = "Project deleted successfully.";
            public const string CannotDeleteProjectWithWorkItems = "Cannot delete project with existing work items.";
            
            public const string NoProjectsFoundForUser = "No projects found for current user.";
            public const string ProjectDeleteFailed = "Failed to delete project.";
            
            public static string ProjectNameExists(string name) => $"Project with name '{name}' already exists.";
        }

        public static class ProjectUpdate
        {
            public const string NoFieldsSpecified = "At least one field must be provided for update.";
            public const string NoPermissionToUpdate = "You do not have permission to update this project.";
            public const string NoPermissionToDelete = "You do not have permission to delete this project.";
            
            public const string NoPermissionToAccess = "You do not have permission to access this project.";
            public const string OnlyOwnerCanUpdate = "Only project owner can update the project.";
            public const string OnlyOwnerCanDelete = "Only project owner can delete the project.";
            public const string OnlyOwnerCanAddParticipants = "Only project owner can add participants.";
            public const string OnlyOwnerCanRemoveParticipants = "Only project owner can remove participants.";
        }

        public static class Participants
        {
            public const string AddParticipantSuccess = "Participant added successfully.";
            public const string RemoveParticipantSuccess = "Participant removed successfully.";
            public const string ParticipantAlreadyExists = "User is already a participant in this project.";
            public const string ParticipantNotFound = "User is not a participant in this project.";
            public const string CannotAddOwnerAsParticipant = "Project owner cannot be added as participant.";
            public const string CannotRemoveOwnerFromParticipants = "Project owner cannot be removed from participants.";
            
            public const string AddParticipantFailed = "Failed to add participant. Project or user may not exist, or user may already be a participant.";
            public const string RemoveParticipantFailed = "Failed to remove participant. Project may not exist or user may not be a participant.";
        }

        public static class WorkItems
        {
            public const string WorkItemNameRequired = "Work item name is required.";
            public const string WorkItemCreatedSuccessfully = "Work item created successfully.";
            public const string WorkItemUpdatedSuccessfully = "Work item updated successfully.";
            public const string WorkItemDeletedSuccessfully = "Work item deleted successfully.";
            public const string WorkItemMovedSuccessfully = "Work item moved successfully.";
            public const string InvalidWorkItemState = "Invalid work item state.";
            public const string ProjectIdRequired = "Project ID is required.";
            public const string InvalidIndex = "Invalid index value.";
            public const string NoWorkItemsFoundForProject = "No work items found for this project.";
        }

        public static class WorkItemUpdate
        {
            public const string NoFieldsSpecified = "At least one field must be provided for update.";
            public const string NoPermissionToUpdate = "You do not have permission to update this work item.";
            public const string NoPermissionToDelete = "You do not have permission to delete this work item.";
            public const string NoPermissionToAccess = "You do not have permission to access this work item.";
            public const string OnlyProjectMembersCanModify = "Only project owner and participants can modify work items.";
        }

        public static class WorkItemAssignment
        {
            public const string UserAssignedSuccess = "User assigned to work item successfully.";
            public const string UserUnassignedSuccess = "User unassigned from work item successfully.";
            public const string UserAlreadyAssigned = "User is already assigned to this work item.";
            public const string UserNotAssigned = "User is not assigned to this work item.";
            public const string AssignmentFailed = "Failed to assign user to work item.";
            public const string UnassignmentFailed = "Failed to unassign user from work item.";
            public const string CannotAssignNonProjectMember = "Cannot assign user who is not a project member.";
        }

        public static class WorkItemTags
        {
            public const string TagAddedSuccess = "Tag added to work item successfully.";
            public const string TagRemovedSuccess = "Tag removed from work item successfully.";
            public const string TagAlreadyAdded = "Tag is already added to this work item.";
            public const string TagNotFound = "Tag not found on this work item.";
            public const string AddTagFailed = "Failed to add tag to work item.";
            public const string RemoveTagFailed = "Failed to remove tag from work item.";
        }

        public static class Validation
        {
            public const string InvalidEmailFormat = "Email format is invalid.";
            public const string UsernameMinimumLength = "Username must be at least 3 characters long.";
            public const string UsernameMaximumLength = "Username cannot exceed 50 characters.";
            public const string EmailMaximumLength = "Email cannot exceed 100 characters.";
            public const string PasswordComplexityRequirements = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
            
            public const string ProjectNameMinimumLength = "Project name must be at least 3 characters long.";
            public const string ProjectNameMaximumLength = "Project name cannot exceed 100 characters.";
            public const string ProjectDescriptionMaximumLength = "Project description cannot exceed 500 characters.";
            
            public const string WorkItemNameMinimumLength = "Work item name must be at least 3 characters long.";
            public const string WorkItemNameMaximumLength = "Work item name cannot exceed 200 characters.";
            public const string WorkItemDescriptionMaximumLength = "Work item description cannot exceed 1000 characters.";
        }

        public static class EntityNames
        {
            public const string User = "User";
            public const string Users = "users";
            public const string Tag = "Tag"; 
            public const string Tags = "tags";
            public const string Project = "Project";
            public const string Projects = "projects";
            public const string WorkItem = "Work Item";
            public const string WorkItems = "work items";
            public const string Sprint = "Sprint";
            public const string Sprints = "sprints";
            public const string Participant = "Participant";
            public const string Participants = "participants";
        }
    }
}
