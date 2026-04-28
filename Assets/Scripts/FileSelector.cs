using SFB;
using System.Linq;

class FileSelector {

    internal static string[] getFilePaths(string startDirectory, string[] fileExtensions) {
        return _getFilePaths(startDirectory, fileExtensions, true);
    }

    internal static string[] getFilePaths(string startDirectory, string fileExtension) {
        return _getFilePaths(startDirectory, new string[] { fileExtension }, true);
    }

    internal static string getFilePath(string startDirectory, string[] fileExtensions) {
        return _getFilePaths(startDirectory, fileExtensions, false).FirstOrDefault();
    }

    internal static string getFilePath(string startDirectory, string fileExtension) {
        return _getFilePaths(startDirectory, new string[] { fileExtension }, false).FirstOrDefault();
    }

    internal static string getFolderPath(string startDirectory = "") {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Select Data Folder", startDirectory, false);
        return (paths != null && paths.Length > 0) ? paths[0] : "";
    }

    internal static string getSaveFilePath(string startDirectory, string defaultName, string fileExtension = "csv", string title = "Save File") {
        string path = StandaloneFileBrowser.SaveFilePanel(title, startDirectory, defaultName, fileExtension);
        return path ?? "";
    }

    static string[] _getFilePaths(string startDirectory, string[] fileExtensions, bool multiselect) {
        string combinedExtensions = string.Join(", ", fileExtensions).ToUpper();
        string title = "Select " + combinedExtensions.ToUpper() + " Files";
        ExtensionFilter[] extensions = new[]
        {
                new ExtensionFilter(combinedExtensions.ToUpper() + "Files", fileExtensions),
                new ExtensionFilter("All Files", "*"),
            };

        string[] paths = StandaloneFileBrowser.OpenFilePanel(title, startDirectory, extensions, multiselect);

        return paths;
    }
}