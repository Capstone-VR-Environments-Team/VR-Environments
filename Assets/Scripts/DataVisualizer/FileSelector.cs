using SFB;
using System.Collections.Generic;

class FileSelector {
    internal static string[] getFilePaths(string startDirectory, string fileExtension) {
        return _getFilePaths(startDirectory, new List<string>() { fileExtension }, true);
    }

    internal static string getFilePath(string startDirectory, string fileExtension) {
        return _getFilePaths(startDirectory, new List<string>() { fileExtension }, false)[0];
    }

    internal static string getFilePath(string startDirectory, List<string> fileExtensions)
    {
        return _getFilePaths(startDirectory, fileExtensions, false)[0];
    }

    internal static string getFolderPath(string startDirectory = "") {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Data Folder", startDirectory, false);
        return (paths != null && paths.Length > 0) ? paths[0] : "";
    }

    static string[] _getFilePaths(string startDirectory, List<string> fileExtensions, bool multiselect) {
        string extensionsName = "";
        foreach (string s in fileExtensions)
        {
            extensionsName += s.ToUpper();
            extensionsName += ",";
        }
        extensionsName = extensionsName.Substring(0, extensionsName.Length - 1);
        string title = "Select " + extensionsName + " Files";
        ExtensionFilter[] extensions = new[]
        {
            new ExtensionFilter(extensionsName + "Files", fileExtensions.ToArray()),
            new ExtensionFilter("All Files", "*"),
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel(title, startDirectory, extensions, multiselect);

        return paths;
    }
}