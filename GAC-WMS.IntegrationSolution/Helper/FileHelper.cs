namespace GAC_WMS.IntegrationSolution.Helper
{
    public static class FileHelper
    {
        public static void Archive(string path)
            {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var newFileName = $"{Path.GetFileNameWithoutExtension(path)}_{timestamp}{Path.GetExtension(path)}";
            var archivePath = Path.Combine("C:\\LegacyFiles\\archive", newFileName);
            File.Move(path, archivePath);
        }

        public static void MoveToError(string path, Exception ex)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var newFileName = $"{Path.GetFileNameWithoutExtension(path)}_{timestamp}{Path.GetExtension(path)}";
            var errorPath = Path.Combine("C:\\LegacyFiles\\error", newFileName);
            File.Move(path, errorPath);
            File.AppendAllText(errorPath + ".log", ex.ToString());
        }
    }

}
