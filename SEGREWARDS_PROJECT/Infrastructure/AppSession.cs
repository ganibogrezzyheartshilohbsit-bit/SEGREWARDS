using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

        public static string GetProfileImagePath(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber)) return null;
            return Path.Combine(GetProfileImagesDirectory(), SanitizeFileName(studentNumber.Trim()) + ".png");
        }

        public static void SaveProfileImage(string studentNumber, Image image)
        {
            if (string.IsNullOrWhiteSpace(studentNumber) || image == null) return;

            var dir = GetProfileImagesDirectory();
            Directory.CreateDirectory(dir);

            var path = GetProfileImagePath(studentNumber);
            image.Save(path, ImageFormat.Png);
        }

        public static Image TryLoadProfileImage(string studentNumber)
        {
            try
            {
                var path = GetProfileImagePath(studentNumber);
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;

                // Load a copy so the file isn't locked.
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var temp = Image.FromStream(fs))
                {
                    return new Bitmap(temp);
                }
            }
            catch
            {
                return null;
            }
        }

        private static string GetProfileImagesDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SEGREWARDS",
                "profile-images");
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            return name;
        }
    }
}
