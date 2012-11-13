/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using UnityEngine;

public class AccessorFactory
{
    #region PUBLIC_METHODS

    // Creates a new Accessor object of the appropriate type. The accessor takes
    // a TrackableBehaviour as a target (the Accessor instance accesses this
    // single object).
    public static TrackableAccessor Create(TrackableBehaviour target)
    {
        System.Type trackableType = target.GetType();

        if (trackableType == typeof(MarkerBehaviour))
        {
            return new MarkerAccessor((MarkerBehaviour)target);
        }
        else if (trackableType == typeof(ImageTargetBehaviour))
        {
            return new ImageTargetAccessor((ImageTargetBehaviour)target);
        }
        else if (trackableType == typeof(MultiTargetBehaviour))
        {
            return new MultiTargetAccessor((MultiTargetBehaviour)target);
        }
        else
        {
            Debug.LogWarning(trackableType.ToString() +
                             " is not derived from TrackableBehaviour.");
            return null;
        }
    }

    #endregion // PUBLIC_METHODS
}
