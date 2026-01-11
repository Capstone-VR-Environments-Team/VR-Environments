using SFB;

class FileSelector {
    internal static string[] getFilePaths(string startDirectory, string fileExtension) {
        return _getFilePaths(startDirectory, fileExtension, true);
    }

    internal static string getFilePath(string startDirectory, string fileExtension) {
        return _getFilePaths(startDirectory, fileExtension, false)[0];
    }

    internal static string getFolderPath(string startDirectory = "") {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Data Folder", startDirectory, false);
        return (paths != null && paths.Length > 0) ? paths[0] : "";
    }

    static string[] _getFilePaths(string startDirectory, string fileExtension, bool multiselect) {
        string title = "Select " + fileExtension.ToUpper() + " Files";
        ExtensionFilter[] extensions = new[]
        {
                new ExtensionFilter(fileExtension.ToUpper() + "Files", fileExtension),
                new ExtensionFilter("All Files", "*"),
            };

        string[] paths = StandaloneFileBrowser.OpenFilePanel(title, startDirectory, extensions, multiselect);

        return paths;
    }
}