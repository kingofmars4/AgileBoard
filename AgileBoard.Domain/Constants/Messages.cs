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

        public static class Validation
        {
            public const string InvalidEmailFormat = "Email format is invalid.";
            public const string UsernameMinimumLength = "Username must be at least 3 characters long.";
            public const string UsernameMaximumLength = "Username cannot exceed 50 characters.";
            public const string EmailMaximumLength = "Email cannot exceed 100 characters.";
            public const string PasswordComplexityRequirements = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
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
        }

        public static class Projects
        {
            public const string ProjectNameRequired = "Project name is required.";
            public const string ProjectCreatedSuccessfully = "Project created successfully.";
            public const string ProjectUpdatedSuccessfully = "Project updated successfully.";
            public const string ProjectDeletedSuccessfully = "Project deleted successfully.";
            public const string CannotDeleteProjectWithWorkItems = "Cannot delete project with existing work items.";
        }

        public static class ProjectUpdate
        {
            public const string NoFieldsSpecified = "At least one field must be provided for update.";
            public const string NoPermissionToUpdate = "You do not have permission to update this project.";
            public const string NoPermissionToDelete = "You do not have permission to delete this project.";
        }
    }
}
