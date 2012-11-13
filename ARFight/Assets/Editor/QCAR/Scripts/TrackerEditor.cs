/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEditor;

[CustomEditor(typeof(QCARBehaviour))]
public class TrackerEditor : Editor
{
    #region UNITY_EDITOR_METHODS

    // OnInspectorGUI exposes public Tracker settings in Inspector
    // WorldCenterMode: Defines how the relative transformation that is returned
    //                  by the QCAR Tracker is applied. Either the camera is
    //                  moved in the scene with respect to a "world center" or
    //                  all the targets are moved with respect to the camera.
    public override void OnInspectorGUI()
    {
        QCARBehaviour tb = (QCARBehaviour) target;

        DrawDefaultInspector();

        tb.SetWorldCenterMode((QCARBehaviour.WorldCenterMode)
                EditorGUILayout.EnumPopup("World Center Mode",
                tb.WorldCenterModeSetting));

// We assume Unity 3.2 as minimum requirement.
#if (UNITY_3_3 || UNITY_3_2)
        if (tb.WorldCenterModeSetting == QCARBehaviour.WorldCenterMode.USER)
        {
            tb.SetWorldCenter((TrackableBehaviour)
                EditorGUILayout.ObjectField("World Center", tb.WorldCenter,
                typeof(TrackableBehaviour)));
        }
#else
        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        if (tb.WorldCenterModeSetting == QCARBehaviour.WorldCenterMode.USER)
        {
            tb.SetWorldCenter((TrackableBehaviour)
                EditorGUILayout.ObjectField("World Center", tb.WorldCenter,
                typeof(TrackableBehaviour),
                allowSceneObjects));
        }        
#endif
    }

    #endregion // UNITY_EDITOR_METHODS
}
