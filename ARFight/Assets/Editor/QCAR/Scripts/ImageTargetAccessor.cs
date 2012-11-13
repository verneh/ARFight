/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEditor;
using UnityEngine;

public class ImageTargetAccessor : TrackableAccessor
{
    #region CONSTRUCTION

    // The one ImageTargetBehaviour instance this accessor belongs to is set in
    // the constructor.
    public ImageTargetAccessor(ImageTargetBehaviour target)
    {
        mTarget = target;
    }

    #endregion // CONSTRUCTION



    #region PUBLIC_METHODS

    // This method is called when new configuration values are available and
    // need to be applied to Image Target objects in the scene.
    public override void ApplyDataSetProperties()
    {
        // Prefabs should not be changed
        if (QCARUtilities.GetPrefabType(mTarget) == PrefabType.Prefab)
        {
            return;
        }

        // Update the aspect ratio, visualization and scale of the target:
        ImageTargetBehaviour itb = (ImageTargetBehaviour)mTarget;

        ConfigData.ImageTarget itConfig;
        if (TrackableInDataSet(itb.TrackableName, itb.DataSetName))
        {
            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(itb.DataSetName);
            dataSetData.GetImageTarget(itb.TrackableName, out itConfig);
        }
        else
        {
            // If the Trackable has been removed from the data set we reset it to default.
            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME);
            dataSetData.GetImageTarget(QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME, out itConfig);
            itb.DataSetPath = QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME;
            itb.TrackableName = QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME;
        }

        ImageTargetEditor.UpdateScale(itb, itConfig.size);
        ImageTargetEditor.UpdateVirtualButtons(itb,
            itConfig.virtualButtons.ToArray());
    }


    // This method updates the respective Trackable appearance (e.g.
    // aspect ratio and texture) with data set data.
    public override void ApplyDataSetAppearance()
    {
        // Prefabs should not be changed
        if (QCARUtilities.GetPrefabType(mTarget) == PrefabType.Prefab)
        {
            return;
        }

        // Update the aspect ratio, visualization and scale of the target:
        ImageTargetBehaviour itb = (ImageTargetBehaviour)mTarget;

        ConfigData.ImageTarget itConfig;
        if (TrackableInDataSet(itb.TrackableName, itb.DataSetName))
        {
            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(itb.DataSetName);
            dataSetData.GetImageTarget(itb.TrackableName, out itConfig);
        }
        else
        {
            // If the Trackable has been removed from the data set we reset it to default.
            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME);
            dataSetData.GetImageTarget(QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME, out itConfig);
            itb.DataSetPath = QCARUtilities.GlobalVars.DEFAULT_DATA_SET_NAME;
            itb.TrackableName = QCARUtilities.GlobalVars.DEFAULT_TRACKABLE_NAME;
        }

        ImageTargetEditor.UpdateAspectRatio(itb, itConfig.size);
        ImageTargetEditor.UpdateMaterial(itb);
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    private bool TrackableInDataSet(string trackableName, string dataSetName)
    {
        if (ConfigDataManager.Instance.ConfigDataExists(dataSetName))
        {
            ConfigData dataSetData = ConfigDataManager.Instance.GetConfigData(dataSetName);
            if (dataSetData.ImageTargetExists(trackableName))
            {
                return true;
            }
        }
        return false;
    }

    #endregion // PRIVATE_METHODS
}
