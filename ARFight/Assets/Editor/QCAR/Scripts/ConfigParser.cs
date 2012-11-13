/*==============================================================================
Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
All Rights Reserved.
Qualcomm Confidential and Proprietary
==============================================================================*/

using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

// This class is used to parse the config.xml file into a ConfigData file and
// vice versa. The config.xml file is used to configure Trackables and
// Virtual Buttons.
// Implements a non-thread safe singleton pattern.
public class ConfigParser
{
    #region PROPERTIES

    // Returns the one and only ConfigParser instance.
    public static ConfigParser Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new ConfigParser();

            return mInstance;
        }
    }

    #endregion // PROPERTIES



    #region PRIVATE_MEMBER_VARIABLES

    private static ConfigParser mInstance = null;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PUBLIC_METHODS

    // This method takes a configData object and creates a config.xml file at
    // the given path out of it.
    public bool fileToStruct(string configXMLPath, ConfigData configData)
    {
        if (!File.Exists(configXMLPath))
            return false;

        using (XmlTextReader configReader = new XmlTextReader(configXMLPath))
        {
            while (configReader.Read())
            {
                if (configReader.NodeType == XmlNodeType.Element)
                {
                    // "Global" Attributes
                    string itNameAttr = "";

                    switch (configReader.Name)
                    {
                        case "ImageTarget":

                            // Parse name from config file
                            itNameAttr = configReader.GetAttribute("name");
                            if (itNameAttr == null)
                            {
                                Debug.LogWarning("Found ImageTarget without " +
                                                 "name attribute in " +
                                                 "config.xml. Image Target " +
                                                 "will be ignored.");
                                continue;
                            }

                            // Parse itSize from config file
                            Vector2 itSize = Vector2.zero;
                            string[] itSizeAttr =
                                configReader.GetAttribute("size").Split(' ');
                            if (itSizeAttr != null)
                            {
                                if (!QCARUtilities.SizeFromStringArray(
                                    out itSize, itSizeAttr))
                                {
                                    Debug.LogWarning("Found illegal itSize " +
                                                     "attribute for Image " +
                                                     "Target " + itNameAttr +
                                                     " in config.xml. " +
                                                     "Image Target will be " +
                                                     "ignored.");
                                    continue;
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Image Target " + itNameAttr +
                                                 " is missing a itSize " +
                                                 "attribut in config.xml. " +
                                                 "Image Target will be " +
                                                 "ignored.");
                                continue;
                            }
                            configReader.MoveToElement();

                            ConfigData.ImageTarget imageTarget =
                                new ConfigData.ImageTarget();

                            imageTarget.size = itSize;
                            imageTarget.virtualButtons =
                                new List<ConfigData.VirtualButton>();

                            configData.SetImageTarget(imageTarget, itNameAttr);

                            break;


                        case "VirtualButton":

                            // Parse name from config file
                            string vbNameAttr =
                                configReader.GetAttribute("name");
                            if (vbNameAttr == null)
                            {
                                Debug.LogWarning("Found VirtualButton " +
                                                 "without name attribute in " +
                                                 "config.xml. Virtual Button " +
                                                 "will be ignored.");
                                continue;
                            }

                            // Parse rectangle from config file
                            Vector4 vbRectangle = Vector4.zero;
                            string[] vbRectangleAttr =
                                configReader.GetAttribute("rectangle").Split(' ');
                            if (vbRectangleAttr != null)
                            {
                                if (!QCARUtilities.RectangleFromStringArray(
                                    out vbRectangle, vbRectangleAttr))
                                {
                                    Debug.LogWarning("Found invalid " +
                                                     "rectangle attribute " +
                                                     "for Virtual Button " +
                                                     vbNameAttr +
                                                     " in config.xml. " +
                                                     "Virtual Button will " +
                                                     "be ignored.");
                                    continue;
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Virtual Button " +
                                                 vbNameAttr +
                                                 " has no rectangle " +
                                                 "attribute in config.xml. " +
                                                 "Virtual Button will be " +
                                                 "ignored.");
                                continue;
                            }

                            // Parse enabled boolean from config file
                            bool vbEnabled = true;
                            string enabledAttr =
                                configReader.GetAttribute("enabled");
                            if (enabledAttr != null)
                            {
                                if (string.Compare(enabledAttr,
                                    "true", true) == 0)
                                {
                                    vbEnabled = true;
                                }
                                else if (string.Compare(enabledAttr,
                                    "false", true) == 0)
                                {
                                    vbEnabled = false;
                                }
                                else
                                {
                                    Debug.LogWarning("Found invalid enabled " +
                                                     "attribute for Virtual " +
                                                     "Button " + vbNameAttr +
                                                     " in config.xml. " +
                                                     "Default setting will " +
                                                     "be used.");
                                }
                            }

                            // Parse sensitivity from config file
                            VirtualButtonBehaviour.Sensitivity vbSensitivity =
                                VirtualButtonBehaviour.DEFAULT_SENSITIVITY;
                            string vbSensitivityAttr =
                                configReader.GetAttribute("sensitivity");
                            if (vbSensitivityAttr != null)
                            {
                                if (string.Compare(vbSensitivityAttr,
                                    "low", true) == 0)
                                {
                                    vbSensitivity =
                                    VirtualButtonBehaviour.Sensitivity.LOW;
                                }
                                else if (string.Compare(vbSensitivityAttr,
                                    "medium", true) == 0)
                                {
                                    vbSensitivity =
                                    VirtualButtonBehaviour.Sensitivity.MEDIUM;
                                }
                                else if (string.Compare(vbSensitivityAttr,
                                    "high", true) == 0)
                                {
                                    vbSensitivity =
                                    VirtualButtonBehaviour.Sensitivity.HIGH;
                                }
                                else
                                {
                                    Debug.LogWarning("Found illegal " +
                                                     "sensitivity attribute " +
                                                     "for Virtual Button " +
                                                     vbNameAttr +
                                                     " in config.xml. " +
                                                     "Default setting will " +
                                                     "be used.");
                                }
                            }

                            configReader.MoveToElement();

                            ConfigData.VirtualButton virtualButton =
                                new ConfigData.VirtualButton();

                            string latestITName = GetLatestITName(configData);

                            virtualButton.name = vbNameAttr;
                            virtualButton.rectangle = vbRectangle;
                            virtualButton.enabled = vbEnabled;
                            virtualButton.sensitivity = vbSensitivity;

                            // Since the XML Reader runs top down we can assume
                            // that the Virtual Button that has been found is
                            // part of the latest Image Target.
                            if (configData.ImageTargetExists(latestITName))
                            {
                                configData.AddVirtualButton(virtualButton,
                                                             latestITName);
                            }
                            else
                            {
                                Debug.LogWarning("Image Target with name " +
                                                 latestITName +
                                                 " could not be found. " +
                                                 "Virtual Button " +
                                                 vbNameAttr +
                                                 "will not be added.");
                            }
                            break;

                        case "MultiTarget":

                            // Parse name from config file
                            string mtNameAttr =
                                configReader.GetAttribute("name");
                            if (mtNameAttr == null)
                            {
                                Debug.LogWarning("Found Multi Target without " +
                                                 "name attribute in " +
                                                 "config.xml. Multi Target " +
                                                 "will be ignored.");
                                continue;
                            }
                            configReader.MoveToElement();

                            ConfigData.MultiTarget multiTarget =
                                new ConfigData.MultiTarget();

                            multiTarget.parts =
                                new List<ConfigData.MultiTargetPart>();

                            configData.SetMultiTarget(multiTarget, mtNameAttr);
                            break;


                        case "Part":

                            // Parse name from config file
                            string prtNameAttr =
                                configReader.GetAttribute("name");
                            if (prtNameAttr == null)
                            {
                                Debug.LogWarning("Found Multi Target Part " +
                                                 "without name attribute in " +
                                                 "config.xml. Part will be " +
                                                 "ignored.");
                                continue;
                            }

                            // Parse translations from config file
                            Vector3 prtTranslation = Vector3.zero;
                            string[] prtTranslationAttr =
                                configReader.GetAttribute("translation").Split(' ');
                            if (prtTranslationAttr != null)
                            {
                                if (!QCARUtilities.TransformFromStringArray(
                                    out prtTranslation, prtTranslationAttr))
                                {
                                    Debug.LogWarning("Found illegal " +
                                                     "transform attribute " +
                                                     "for Part " + prtNameAttr +
                                                     " in config.xml. Part " +
                                                     "will be ignored.");
                                    continue;
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Multi Target Part " +
                                                 prtNameAttr + " has no " +
                                                 "translation attribute in " +
                                                 "config.xml. Part will be " +
                                                 "ignored.");
                                continue;
                            }

                            // Parse rotations from config file
                            Quaternion prtRotation = Quaternion.identity;
                            string[] prtRotationAttr =
                                configReader.GetAttribute("rotation").Split(' ');
                            if (prtRotationAttr != null)
                            {
                                if (!QCARUtilities.OrientationFromStringArray(
                                    out prtRotation, prtRotationAttr))
                                {
                                    Debug.LogWarning("Found illegal rotation " +
                                                     "attribute for Part " +
                                                     prtNameAttr +
                                                     " in config.xml. Part " +
                                                     "will be ignored.");
                                    continue;
                                }
                            }
                            else
                            {
                                Debug.LogWarning("Multi Target Part " +
                                                 prtNameAttr + " has no " +
                                                 "rotation attribute in " +
                                                 "config.xml. Part will be " +
                                                 "ignored.");
                                continue;
                            }

                            configReader.MoveToElement();

                            ConfigData.MultiTargetPart multiTargetPart =
                                new ConfigData.MultiTargetPart();

                            string latestMTName = GetLatestMTName(configData);

                            multiTargetPart.name = prtNameAttr;
                            multiTargetPart.rotation = prtRotation;
                            multiTargetPart.translation = prtTranslation;

                            // Since the XML Reader runs top down we can assume
                            // that the Virtual Button that has been found is
                            // part of the latest Image Target.
                            if (configData.MultiTargetExists(latestMTName))
                            {
                                configData.AddMultiTargetPart(multiTargetPart,
                                                               latestMTName);
                            }
                            else
                            {
                                Debug.LogWarning("Multi Target with name " +
                                                 latestMTName +
                                                 " could not be found. " +
                                                 "Multi Target Part " +
                                                 prtNameAttr +
                                                 "will not be added.");
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        return true;
    }


    // This method reads a config.xml file at the given path and fills the
    // ConfigData object with the data.
    public bool structToFile(string configXMLPath, ConfigData configData)
    {
        // If there are no trackables in the data set we don't write a config file.
        if (configData.NumTrackables <= 0)
            return false;

        XmlWriterSettings configWriterSettings = new XmlWriterSettings();
        configWriterSettings.Indent = true;
        using (XmlWriter configWriter =
            XmlWriter.Create(configXMLPath, configWriterSettings))
        {
            configWriter.WriteStartDocument();

            configWriter.WriteStartElement("QCARConfig");
            configWriter.WriteAttributeString("xmlns", "xsi", null,
                "http://www.w3.org/2001/XMLSchema-instance");
            configWriter.WriteAttributeString("xsi",
                "noNamespaceSchemaLocation", null, "qcar_config.xsd");

            configWriter.WriteStartElement("Tracking");

            // Writing Image Target elements into config.xml file.
            string[] imageTargetNames = new string[configData.NumImageTargets];
            configData.CopyImageTargetNames(imageTargetNames, 0);
            for (int i = 0; i < imageTargetNames.Length; ++i)
            {
                ConfigData.ImageTarget it;

                configData.GetImageTarget(imageTargetNames[i], out it);

                configWriter.WriteStartElement("ImageTarget");
                string imageTargetSize = it.size.x.ToString() + " " +
                                         it.size.y.ToString();
                configWriter.WriteAttributeString("size", imageTargetSize);
                configWriter.WriteAttributeString("name", imageTargetNames[i]);

                // Writing Virtual Button elements into config.xml file per
                // Image Target.
                List<ConfigData.VirtualButton> vbs = it.virtualButtons;
                for (int j = 0; j < vbs.Count; j++)
                {
                    configWriter.WriteStartElement("VirtualButton");
                    configWriter.WriteAttributeString("name", vbs[j].name);
                    string virtualButtonRectangle =
                        vbs[j].rectangle.x.ToString() + " " +
                        vbs[j].rectangle.y.ToString() + " " +
                        vbs[j].rectangle.z.ToString() + " " +
                        vbs[j].rectangle.w.ToString();
                    configWriter.WriteAttributeString("rectangle",
                        virtualButtonRectangle);
                    if (vbs[j].enabled)
                        configWriter.WriteAttributeString("enabled",
                                                          "true");
                    else
                        configWriter.WriteAttributeString("enabled",
                                                          "false");
                    if (vbs[j].sensitivity ==
                        VirtualButtonBehaviour.Sensitivity.LOW)
                        configWriter.WriteAttributeString("sensitivity",
                                                          "low");
                    else if (vbs[j].sensitivity ==
                        VirtualButtonBehaviour.Sensitivity.MEDIUM)
                        configWriter.WriteAttributeString("sensitivity",
                                                          "medium");
                    else if (vbs[j].sensitivity ==
                        VirtualButtonBehaviour.Sensitivity.HIGH)
                        configWriter.WriteAttributeString("sensitivity",
                                                          "high");
                    configWriter.WriteEndElement(); // VirtualButton
                }
                configWriter.WriteEndElement(); // ImageTarget
            }

            // Writing Multi Target elements into config.xml file
            string[] multiTargetNames = new string[configData.NumMultiTargets];
            configData.CopyMultiTargetNames(multiTargetNames, 0);
            for (int i = 0; i < multiTargetNames.Length; i++)
            {
                ConfigData.MultiTarget mt;

                configData.GetMultiTarget(multiTargetNames[i], out mt);

                configWriter.WriteStartElement("MultiTarget");
                configWriter.WriteAttributeString("name", multiTargetNames[i]);

                // Writing Multi Target Part elements into config.xml file
                List<ConfigData.MultiTargetPart> prts = mt.parts;
                for (int j = 0; j < prts.Count; j++)
                {
                    configWriter.WriteStartElement("Part");
                    configWriter.WriteAttributeString("name", prts[j].name);
                    string multiTargetTranslation =
                        prts[j].translation.x.ToString() + " " +
                        prts[j].translation.z.ToString() + " " +
                        prts[j].translation.y.ToString();
                    configWriter.WriteAttributeString("translation",
                                                      multiTargetTranslation);
                    float rotationAngle;
                    Vector3 rotationAxis;
                    prts[j].rotation.ToAngleAxis(out rotationAngle,
                                                 out rotationAxis);
                    string multiTargetRotation =
                        "AD: " +
                        (-rotationAxis.x).ToString() + " " +
                        (-rotationAxis.z).ToString() + " " +
                        rotationAxis.y.ToString() + " " +
                        rotationAngle.ToString();
                    configWriter.WriteAttributeString("rotation",
                        multiTargetRotation);
                    configWriter.WriteEndElement(); // Part
                }

                configWriter.WriteEndElement(); // MultiTarget
            }

            configWriter.WriteEndElement(); // Tracking

            configWriter.WriteEndElement(); // QCARConfig

            configWriter.WriteEndDocument();
        }

        return true;
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    // Returns the name of the Image Target that has been parsed the latest.
    // Returns null if no Image Target has been parsed yet.
    private static string GetLatestITName(ConfigData backlog)
    {
        if (backlog == null)
            return null;

        string[] itNames = new string[backlog.NumImageTargets];
        try
        {
            backlog.CopyImageTargetNames(itNames, 0);
        }
        catch
        {
            return null;
        }

        return itNames[backlog.NumImageTargets - 1];
    }


    // Returns the name of the Multi Target that has been parsed the latest.
    // Returns null if no Image Target has been parsed yet.
    private static string GetLatestMTName(ConfigData backlog)
    {
        if (backlog == null)
            return null;

        string[] mtNames = new string[backlog.NumMultiTargets];
        try
        {
            backlog.CopyMultiTargetNames(mtNames, 0);
        }
        catch
        {
            return null;
        }

        return mtNames[backlog.NumMultiTargets - 1];
    }

    #endregion // PRIVATE_METHODS

}