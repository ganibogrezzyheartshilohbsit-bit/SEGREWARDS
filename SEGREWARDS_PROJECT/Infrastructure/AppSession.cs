namespace SEGREWARDS_PROJECT.Infrastructure
{
    /// <summary>
    /// Lightweight session for WinForms after login.
    /// </summary>
    public static class AppSession
    {
        public static long? CurrentUserId { get; set; }

        public static string CurrentStudentNumber { get; set; }

        public static string CurrentFullName { get; set; }

        public static int CurrentEcoPoints { get; set; }

        public static void Clear()
        {
            CurrentUserId = null;
            CurrentStudentNumber = null;
            CurrentFullName = null;
            CurrentEcoPoints = 0;
        }
    }
}
