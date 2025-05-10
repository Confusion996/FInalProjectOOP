using AdminApplication.Models;

namespace AdminApplication.Helpers
{
    public static class AppState
    {
        public static bool IsAdminLoggedIn { get; set; } = false;
        public static AdminAccount LoggedInAdmin { get; set; } = null;
    }
}

