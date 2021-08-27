#if HIRA_BOTS_CREATOR_MODE
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// Extras that aren't a part of the actual project.
    /// </summary>
    internal static class NonProjectRelatedUtilities
    {
        /// <summary>
        /// Print the number of lines of code in the package.
        /// </summary>
        [MenuItem("HiraBots/Print Lines Of Code")]
        private static void PrintLinesOfCode()
        {
            var projectDirectory = Path.GetDirectoryName(Application.dataPath);
            if (projectDirectory == null)
            {
                Debug.LogError("Could not determine project directory.");
                return;
            }

            var packagePath = Path.Combine(projectDirectory, "Packages", "com.rohanjadav.hirabots");

            var textFilesLineCount = GetLinesOfText(Directory.GetFiles(packagePath, "*.txt",
                SearchOption.AllDirectories));

            var locCount = GetLinesOfText(Directory.GetFiles(packagePath, "*.cs",
                SearchOption.AllDirectories));

            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null,
                "<color=green><b>Search completed!</b></color> - Click here to view.\n" +
                "\n" +
                "\n" +
                $"<b>Total Lines:</b> {textFilesLineCount + locCount}\n" +
                "\n" +
                $"<b>*.txt Lines:</b> {textFilesLineCount}\n" +
                "\n" +
                $"<b>*.cs Lines:</b> {locCount}");
        }

        private static int GetLinesOfText(ReadOnlyArrayAccessor<string> files)
        {
            var textFilesLineCount = 0;
            foreach (var file in files)
            {
                using (var reader = File.OpenText(file))
                {
                    while (reader.ReadLine() != null)
                    {
                        textFilesLineCount++;
                    }
                }
            }

            return textFilesLineCount;
        }
    }
}
#endif