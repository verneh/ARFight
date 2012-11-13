/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataSetLoadBehaviour))]
public class DataSetLoadEditor : Editor
{
    #region UNITY_EDITOR_METHODS

    public void OnEnable()
    {
        if (!SceneManager.Instance.SceneInitialized)
        {
            SceneManager.Instance.InitScene();
        }
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataSetLoadBehaviour dslb = (DataSetLoadBehaviour)target;

        // If this instance is a prefab don't show anything in the inspector.
        if (QCARUtilities.GetPrefabType(dslb) == PrefabType.Prefab)
        {
            return;
        }

        // We know that the data set manager also has a default data set that we don't want to use => "num - 1".
        string[] dataSetList = new string[ConfigDataManager.Instance.NumConfigDataObjects - 1];
        // Fill list with available data sets.
        ConfigDataManager.Instance.GetConfigDataNames(dataSetList, false);

        DrawDataSetToActivate(dslb, dataSetList);

        DrawDataSetsToLoad(dslb, dataSetList);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(dslb);
        }
    }

    #endregion // UNITY_EDITOR_METHODS


    #region PUBLIC_METHODS

    // Called by the Scene Manager to notify that the list of data sets may have changed
    public static void OnConfigDataChanged()
    {
        // List of config data objects minus the default object:
        string[] dataSetList = new string[ConfigDataManager.Instance.NumConfigDataObjects - 1];
        ConfigDataManager.Instance.GetConfigDataNames(dataSetList, false);

        DataSetLoadBehaviour[] dslbs = (DataSetLoadBehaviour[])UnityEngine.Object.FindObjectsOfType(
                                            typeof(DataSetLoadBehaviour));

        foreach (DataSetLoadBehaviour dslb in dslbs)
        {
            // Clear the dataset to activate if the dataset no longer exists:
            if ((dslb.DataSetToActivate != null) &&
                (System.Array.Find(dataSetList, s => s.Equals(dslb.DataSetToActivate)) == null))
            {   
                dslb.DataSetToActivate = null;
            }

            // Clear any datasets to load if they no longer exists:
            dslb.mDataSetsToLoad.RemoveAll(s => (System.Array.Find(
                dataSetList, str => str.Equals(s)) == null));
        }
    }

    #endregion // PUBLIC_METHODS


    #region PRIVATE_METHODS

    // Draws a drop down list to choose a data set to activate.
    private void DrawDataSetToActivate(DataSetLoadBehaviour dslb, string[] dataSetList)
    {
        string[] dataSetListDropDownList = new string[dataSetList.Length + 1];
        dataSetListDropDownList[0] = "None";
        dataSetList.CopyTo(dataSetListDropDownList, 1);

        int currentDataSetIndex = QCARUtilities.GetIndexFromString(dslb.DataSetToActivate, dataSetListDropDownList);
        if (currentDataSetIndex < 0)
            currentDataSetIndex = 0;

        int newDataSetIndex = EditorGUILayout.Popup("Activate Data Set",
                                                    currentDataSetIndex,
                                                    dataSetListDropDownList);
        if (newDataSetIndex < 1)
            dslb.DataSetToActivate = null;
        else
        {
            dslb.DataSetToActivate = dataSetListDropDownList[newDataSetIndex];
        }
    }


    // Draws check boxes for all data sets to choose to load them.
    private void DrawDataSetsToLoad(DataSetLoadBehaviour dslb, string[] dataSetList)
    {
        foreach (string dataSet in dataSetList)
        {
            bool loadDataSet = dslb.mDataSetsToLoad.Contains(dataSet);

            // Remove data sets that are being unchecked.
            if (loadDataSet && (!EditorGUILayout.Toggle("Load Data Set " + dataSet, loadDataSet)))
            {
                dslb.mDataSetsToLoad.Remove(dataSet);
            }
            // Add data sets that are being checked.
            else if ((!loadDataSet) && EditorGUILayout.Toggle("Load Data Set " + dataSet, loadDataSet))
            {
                dslb.mDataSetsToLoad.Add(dataSet);
            }
        }
    }

    #endregion // PRIVATE_METHODS
}
