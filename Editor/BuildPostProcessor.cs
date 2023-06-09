//This is a modified version of https://gist.github.com/eppz/1ebbc1cf6a77741f56d63d3803e57ba3
#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class BuildPostProcessor
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.iOS)
        {
            // Read.
            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);
            
            var targetGUID = project.GetUnityFrameworkTargetGuid();
            
            AddFrameworks(project, targetGUID);
            
            var plistPath = Path.Combine(path, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            plist.root.SetString("NSSpeechRecognitionUsageDescription", "This app needs access to Speech Recognition");
            plist.root.SetString("NSMicrophoneUsageDescription", "Used for Speech Recognition");
            plist.WriteToFile(plistPath);

            // Write.
            project.WriteToFile(projectPath);
        }
    }

    static void AddFrameworks(PBXProject project, string targetGUID)
    {
        project.AddFrameworkToProject(targetGUID, "Speech.framework", false);
        //This project appears to be a default now:
        //project.AddFrameworkToProject(targetGUID, "AVFoundation.framework", false);
        // Add `-ObjC` to "Other Linker Flags".
        project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");
    }
}
#endif