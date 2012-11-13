using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

public class DatasetImportWindow : EditorWindow
{
    private int mVersionIndex = 0;
    private GUIStyle mWindowStyle = null;
    private const string INFO_MESSAGE =
        "Choose the desired QCAR version for correct dataset configuration.";

    private static string[] versions = { "QCAR 1.0.x", "QCAR 1.5.x" };


    // This method needs to be called to instantiate the Window object and
    // draw a Window on the screen.
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one
        DatasetImportWindow instance =
            EditorWindow.GetWindow<DatasetImportWindow>(true, "QCAR Version", true);

        int width = 300;
        int height = 100;
        instance.position = new Rect((Screen.width - width) / 2,
                                     (Screen.height + height) / 2, width, height);

        instance.ShowPopup();
    }


    // OnGUI is called when GUI actions happen on the Window.
    public void OnGUI()
    {
        this.mWindowStyle = new GUIStyle(GUI.skin.label);
        this.mWindowStyle.wordWrap = true;

        // Show info text
        GUILayout.Label(INFO_MESSAGE, mWindowStyle, null);

        // Show drop down
        mVersionIndex = EditorGUILayout.Popup("QCAR Version", mVersionIndex, versions);

        // Show Ok button
        if (GUILayout.Button("Ok"))
        {
            if (mVersionIndex == 0)
            {
                // Downgrade dataset for use with 1.0
                // Note we use reflection to avoid errors when these scripts are deleted
                Type actionType = Type.GetType("DatasetImportAction");
                if (actionType != null)
                {
                    actionType.InvokeMember("DowngradeDataset",
                            BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                            null, null, null);
                }
            }

            this.Close();
        }
    }
}
