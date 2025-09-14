namespace AgileBoard.Domain.Constants
{
    public static class Messages
    {
        public static class Authorization
        {
            public const string CheckFailed = "Authorization check failed.";
            public const string AccessDenied = "Access denied.";
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

        public static class ProjectRetrieval
        {
            public const string ProjectNotFound = "Project not found.";
            public const string ProjectRetrievalErrorGeneric = "An error occurred while retrieving project information.";
        }
        public static class UserRetrieval
        {
            public const string UserNotFound = "User not found.";
            public const string UsersNotFound = "No users found.";
            public const string UserRetrievalErrorGeneric = "An error occurred while retrieving user information.";
        }

        public static class UserDeletion
        {
            public const string UserDeletedSuccessfully = "User deleted successfully.";
            public const string UserDeletionErrorGeneric = "An error occurred while deleting the user.";
            public const string CannotDeleteUserWithProjects = "Cannot delete user who owns active projects.";
        }

        public static class Validation
        {
            public const string InvalidEmailFormat = "Email format is invalid.";
            public const string UsernameMinimumLength = "Username must be at least 3 characters long.";
            public const string UsernameMaximumLength = "Username cannot exceed 50 characters.";
            public const string EmailMaximumLength = "Email cannot exceed 100 characters.";
            public const string PasswordComplexityRequirements = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
        }

        public static class Generic
        {
            public const string InternalServerError = "An internal server error occurred. Please try again later.";
            public const string BadRequestGeneric = "The request is invalid. Please check your input.";
            public const string UnauthorizedGeneric = "You are not authorized to perform this action.";
            public const string NotFoundGeneric = "The requested resource was not found.";
        }
    }
}
