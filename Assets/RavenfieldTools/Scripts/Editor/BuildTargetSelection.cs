using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildTargetSelection : MonoBehaviour {

    public static BuildTarget buildTarget = BuildTarget.StandaloneWindows;

    [MenuItem("Ravenfield Tools/Build Target/Set Build Target")]
    static void SetBuildTarget() {
        int selection = EditorUtility.DisplayDialogComplex("Set Build Target", "The Windows build target is recommended and will also work on mac and linux most of the time. If you are having trouble with pink materials and shaders when mac/linux users play your content, you can change the build target to create special max and linux builds that may solve those issues.", "Windows (Recommended)", "Mac", "Linux");

        if (selection == 0) {
            buildTarget = BuildTarget.StandaloneWindows;
        }
        else if (selection == 1) {
            buildTarget = BuildTarget.StandaloneOSXUniversal;
        }
        else if (selection == 2) {
            buildTarget = BuildTarget.StandaloneLinuxUniversal;
        }
    }

    [MenuItem("Ravenfield Tools/Build Target/Windows (Recommended)")]
    static void Windows() { SetBuildTarget(); }

    [MenuItem("Ravenfield Tools/Build Target/Mac")]
    static void Mac() { SetBuildTarget(); }

    [MenuItem("Ravenfield Tools/Build Target/Linux")]
    static void Linux() { SetBuildTarget(); }

    [MenuItem("Ravenfield Tools/Build Target/Windows (Recommended)", true)]
    static bool VWindows() { return buildTarget == BuildTarget.StandaloneWindows; }

    [MenuItem("Ravenfield Tools/Build Target/Mac", true)]
    static bool VMac() { return buildTarget == BuildTarget.StandaloneOSXUniversal; }

    [MenuItem("Ravenfield Tools/Build Target/Linux", true)]
    static bool VLinux() { return buildTarget == BuildTarget.StandaloneLinuxUniversal; }
}
