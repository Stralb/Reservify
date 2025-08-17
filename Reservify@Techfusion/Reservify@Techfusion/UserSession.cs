using System;

namespace Reservify_Techfusion
{
    public static class UserSession
    {
        // Property to store the current user's ID
        public static int CurrentUserId { get; private set; }

        // Method to set the user ID
        public static void SetUserId(int userId)
        {
            CurrentUserId = userId;
        }

        // Method to get the current user ID
        public static int GetUserId()
        {
            return CurrentUserId;
        }

        // Method to clear the user session (e.g., on logout)
        public static void ClearUser()
        {
            CurrentUserId = 0; // Reset the user ID
        }

        // Optional: Method to check if a user is logged in
        public static bool IsUserLoggedIn()
        {
            return CurrentUserId > 0;
        }
    }
}
