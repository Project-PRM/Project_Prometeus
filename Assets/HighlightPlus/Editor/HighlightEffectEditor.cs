﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HighlightPlus {

    [CustomEditor(typeof(HighlightEffect))]
    [CanEditMultipleObjects]
    public class HighlightEffectEditor : UnityEditor.Editor {

#pragma warning disable 0618

        SerializedProperty profile, profileSync, camerasLayerMask, ignoreObjectVisibility, reflectionProbes, GPUInstancing, optimizeSkinnedMesh, sortingPriority;
        SerializedProperty ignore, effectGroup, effectGroupLayer, effectNameFilter, effectNameUseRegEx, effectTarget, combineMeshes, alphaCutOff, cullBackFaces, padding, normalsOption;
        SerializedProperty highlighted, fadeInDuration, fadeOutDuration, flipY, constantWidth, extraCoveragePixels, subMeshMask, minimumWidth;
        SerializedProperty overlay, overlayMode, overlayColor, overlayAnimationSpeed, overlayMinIntensity, overlayBlending, overlayTexture, overlayTextureUVSpace, overlayTextureScale, overlayTextureScrolling, overlayVisibility;
        SerializedProperty overlayPattern, overlayPatternScrolling, overlayPatternScale, overlayPatternSize, overlayPatternSoftness, overlayPatternRotation;
        SerializedProperty outline, outlineColor, outlineColorStyle, outlineGradient, outlineGradientInLocalSpace, outlineGradientKnee, outlineGradientPower;
        SerializedProperty outlineBlurPasses, outlineWidth, outlineQuality, outlineEdgeMode, outlineEdgeThreshold, outlineSharpness;
        SerializedProperty outlineDownsampling, outlineVisibility, outlineBlitDebug, outlineIndependent, outlineContourStyle, outlineMaskMode;
        SerializedProperty outlineStylized, outlinePattern, outlinePatternScale, outlinePatternThreshold, outlinePatternDistortionAmount, outlinePatternStopMotionScale;
        SerializedProperty outlinePatternDistortionTexture;
        SerializedProperty outlineDashed, outlineDashWidth, outlineDashGap, outlineDashSpeed;
        SerializedProperty outlineDistanceScaleBias;
        SerializedProperty outlinePixelation;
        SerializedProperty glow, glowWidth, glowQuality, glowBlurMethod, glowDownsampling, glowHQColor, glowDithering, glowDitheringStyle, glowMagicNumber1, glowMagicNumber2, glowAnimationSpeed, glowDistanceScaleBias;
        SerializedProperty glowBlendPasses, glowPasses, glowVisibility, glowBlendMode, glowBlitDebug, glowMaskMode, glowHighPrecision;
        SerializedProperty glowPixelation;
        SerializedProperty innerGlow, innerGlowWidth, innerGlowColor, innerGlowBlendMode, innerGlowVisibility, innerGlowPower;
        SerializedProperty iconFX, iconFXAssetType, iconFXPrefab, iconFXCenter, iconFXMesh, iconFXLightColor, iconFXDarkColor, iconFXRotationSpeed, iconFXAnimationOption, iconFXAnimationAmount, iconFXAnimationSpeed, iconFXScale, iconFXScaleToRenderBounds, iconFXOffset, iconFXTransitionDuration, iconFXStayDuration;
        SerializedProperty seeThrough, seeThroughOccluderMask, seeThroughOccluderMaskAccurate, seeThroughOccluderThreshold, seeThroughOccluderCheckInterval, seeThroughOccluderCheckIndividualObjects, seeThroughDepthOffset, seeThroughMaxDepth;
        SerializedProperty seeThroughIntensity, seeThroughTintAlpha, seeThroughTintColor, seeThroughNoise, seeThroughBorder, seeThroughBorderWidth, seeThroughBorderColor, seeThroughOrdered, seeThroughBorderOnly, seeThroughTexture, seeThroughTextureUVSpace, seeThroughTextureScale, seeThroughChildrenSortingMode;
        SerializedProperty targetFX, targetFXTexture, targetFXColor, targetFXCenter, targetFXRotationSpeed, targetFXInitialScale, targetFXEndScale, targetFXScaleToRenderBounds, targetFXUseEnclosingBounds, targetFXSquare, targetFXOffset;
        SerializedProperty targetFXAlignToGround, targetFXFadePower, targetFXGroundMaxDistance, targetFXGroundLayerMask, targetFXTransitionDuration, targetFXStayDuration, targetFXVisibility;
        SerializedProperty targetFXStyle, targetFXFrameWidth, targetFXCornerLength, targetFXFrameMinOpacity, targetFXGroundMinAltitude;
        SerializedProperty targetFXRotationAngle, targetFxCenterOnHitPosition, targetFxAlignToNormal;
        SerializedProperty hitFxInitialIntensity, hitFxMode, hitFxFadeOutDuration, hitFxColor, hitFxRadius, hitFXTriggerMode;
        SerializedProperty cameraDistanceFade, cameraDistanceFadeNear, cameraDistanceFadeFar;
        SerializedProperty labelEnabled, labelText, labelColor, labelPrefab, labelVerticalOffset, lineLength, labelFollowCursor;
        SerializedProperty labelTarget, labelTextSize, labelMode, labelAlignment, labelShowInEditorMode;
        SerializedProperty useOriginalShader, customPassName;


        HighlightEffect thisEffect;
        bool profileChanged, forceRefresh, enableProfileApply;

        UniversalRenderPipelineAsset pipe;
        bool expandGeneralSettings, expandHighlightOptions, expandAdvancedSettings;
        bool showCurrentOccluders;
        const string HP_GENERAL_SETTINGS = "HPGeneralSettings";
        const string HP_HIGHLIGHT_OPTIONS = "HPHighlightOptions";
        const string HP_ADVANCED_SETTINGS = "HPAdvancedSettings";
        GUIStyle foldoutBold;
        readonly List<Transform> occluders = new List<Transform>();
        readonly List<string> passNames = new List<string>();
        readonly List<Renderer> renderers = new List<Renderer>();

        void OnEnable () {
            expandGeneralSettings = EditorPrefs.GetBool(HP_GENERAL_SETTINGS, true);
            expandHighlightOptions = EditorPrefs.GetBool(HP_HIGHLIGHT_OPTIONS, true);
            expandAdvancedSettings = EditorPrefs.GetBool(HP_ADVANCED_SETTINGS, false);

            profile = serializedObject.FindProperty("profile");
            profileSync = serializedObject.FindProperty("profileSync");
            camerasLayerMask = serializedObject.FindProperty("camerasLayerMask");
            ignoreObjectVisibility = serializedObject.FindProperty("ignoreObjectVisibility");
            reflectionProbes = serializedObject.FindProperty("reflectionProbes");
            optimizeSkinnedMesh = serializedObject.FindProperty("optimizeSkinnedMesh");
            sortingPriority = serializedObject.FindProperty("sortingPriority");
            normalsOption = serializedObject.FindProperty("normalsOption");
            GPUInstancing = serializedObject.FindProperty("GPUInstancing");
            ignore = serializedObject.FindProperty("ignore");
            effectGroup = serializedObject.FindProperty("effectGroup");
            effectGroupLayer = serializedObject.FindProperty("effectGroupLayer");
            effectNameFilter = serializedObject.FindProperty("effectNameFilter");
            effectNameUseRegEx = serializedObject.FindProperty("effectNameUseRegEx");
            effectTarget = serializedObject.FindProperty("effectTarget");
            combineMeshes = serializedObject.FindProperty("combineMeshes");
            alphaCutOff = serializedObject.FindProperty("alphaCutOff");
            cullBackFaces = serializedObject.FindProperty("cullBackFaces");
            padding = serializedObject.FindProperty("padding");
            highlighted = serializedObject.FindProperty("_highlighted");
            fadeInDuration = serializedObject.FindProperty("fadeInDuration");
            fadeOutDuration = serializedObject.FindProperty("fadeOutDuration");
            flipY = serializedObject.FindProperty("flipY");
            constantWidth = serializedObject.FindProperty("constantWidth");
            extraCoveragePixels = serializedObject.FindProperty("extraCoveragePixels");
            minimumWidth = serializedObject.FindProperty("minimumWidth");
            subMeshMask = serializedObject.FindProperty("subMeshMask");
            overlay = serializedObject.FindProperty("overlay");
            overlayMode = serializedObject.FindProperty("overlayMode");
            overlayColor = serializedObject.FindProperty("overlayColor");
            overlayAnimationSpeed = serializedObject.FindProperty("overlayAnimationSpeed");
            overlayMinIntensity = serializedObject.FindProperty("overlayMinIntensity");
            overlayBlending = serializedObject.FindProperty("overlayBlending");
            overlayTexture = serializedObject.FindProperty("overlayTexture");
            overlayTextureUVSpace = serializedObject.FindProperty("overlayTextureUVSpace");
            overlayTextureScale = serializedObject.FindProperty("overlayTextureScale");
            overlayTextureScrolling = serializedObject.FindProperty("overlayTextureScrolling");
            overlayVisibility = serializedObject.FindProperty("overlayVisibility");
            overlayPattern = serializedObject.FindProperty("overlayPattern");
            overlayPatternScrolling = serializedObject.FindProperty("overlayPatternScrolling");
            overlayPatternRotation = serializedObject.FindProperty("overlayPatternRotation");
            overlayPatternScale = serializedObject.FindProperty("overlayPatternScale");
            overlayPatternSize = serializedObject.FindProperty("overlayPatternSize");
            overlayPatternSoftness = serializedObject.FindProperty("overlayPatternSoftness");

            outline = serializedObject.FindProperty("outline");
            outlineColor = serializedObject.FindProperty("outlineColor");
            outlineColorStyle = serializedObject.FindProperty("outlineColorStyle");
            outlineGradient = serializedObject.FindProperty("outlineGradient");
            outlineGradientInLocalSpace = serializedObject.FindProperty("outlineGradientInLocalSpace");
            outlineGradientKnee = serializedObject.FindProperty("outlineGradientKnee");
            outlineGradientPower = serializedObject.FindProperty("outlineGradientPower");
            outlineWidth = serializedObject.FindProperty("outlineWidth");
            outlineBlurPasses = serializedObject.FindProperty("outlineBlurPasses");
            outlineQuality = serializedObject.FindProperty("outlineQuality");
            outlineEdgeMode = serializedObject.FindProperty("outlineEdgeMode");
            outlineEdgeThreshold = serializedObject.FindProperty("outlineEdgeThreshold");
            outlineSharpness = serializedObject.FindProperty("outlineSharpness");
            outlineDownsampling = serializedObject.FindProperty("outlineDownsampling");
            outlineVisibility = serializedObject.FindProperty("outlineVisibility");
            outlineBlitDebug = serializedObject.FindProperty("outlineBlitDebug");
            outlineIndependent = serializedObject.FindProperty("outlineIndependent");
            outlineContourStyle = serializedObject.FindProperty("outlineContourStyle");
            outlineMaskMode = serializedObject.FindProperty("outlineMaskMode");
            outlineStylized = serializedObject.FindProperty("outlineStylized");
            outlinePattern = serializedObject.FindProperty("outlinePattern");
            outlinePatternScale = serializedObject.FindProperty("outlinePatternScale");
            outlinePatternThreshold = serializedObject.FindProperty("outlinePatternThreshold");
            outlinePatternDistortionAmount = serializedObject.FindProperty("outlinePatternDistortionAmount");
            outlinePatternStopMotionScale = serializedObject.FindProperty("outlinePatternStopMotionScale");
            outlinePatternDistortionTexture = serializedObject.FindProperty("outlinePatternDistortionTexture");
            outlineDashed = serializedObject.FindProperty("outlineDashed");
            outlineDashWidth = serializedObject.FindProperty("outlineDashWidth");
            outlineDashGap = serializedObject.FindProperty("outlineDashGap");
            outlineDashSpeed = serializedObject.FindProperty("outlineDashSpeed");
            outlineDistanceScaleBias = serializedObject.FindProperty("outlineDistanceScaleBias");
            outlinePixelation = serializedObject.FindProperty("outlinePixelation");

            glow = serializedObject.FindProperty("glow");
            glowWidth = serializedObject.FindProperty("glowWidth");
            glowQuality = serializedObject.FindProperty("glowQuality");
            glowBlurMethod = serializedObject.FindProperty("glowBlurMethod");
            glowHQColor = serializedObject.FindProperty("glowHQColor");
            glowAnimationSpeed = serializedObject.FindProperty("glowAnimationSpeed");
            glowDistanceScaleBias = serializedObject.FindProperty("glowDistanceScaleBias");
            glowBlendPasses = serializedObject.FindProperty("glowBlendPasses");
            glowHighPrecision = serializedObject.FindProperty("glowHighPrecision");
            glowDithering = serializedObject.FindProperty("glowDithering");
            glowDitheringStyle = serializedObject.FindProperty("glowDitheringStyle");
            glowMagicNumber1 = serializedObject.FindProperty("glowMagicNumber1");
            glowMagicNumber2 = serializedObject.FindProperty("glowMagicNumber2");
            glowPasses = serializedObject.FindProperty("glowPasses");
            glowVisibility = serializedObject.FindProperty("glowVisibility");
            glowBlendMode = serializedObject.FindProperty("glowBlendMode");
            glowBlitDebug = serializedObject.FindProperty("glowBlitDebug");
            glowMaskMode = serializedObject.FindProperty("glowMaskMode");
            glowDownsampling = serializedObject.FindProperty("glowDownsampling");
            glowPixelation = serializedObject.FindProperty("glowPixelation");

            innerGlow = serializedObject.FindProperty("innerGlow");
            innerGlowColor = serializedObject.FindProperty("innerGlowColor");
            innerGlowWidth = serializedObject.FindProperty("innerGlowWidth");
            innerGlowPower = serializedObject.FindProperty("innerGlowPower");
            innerGlowBlendMode = serializedObject.FindProperty("innerGlowBlendMode");
            innerGlowVisibility = serializedObject.FindProperty("innerGlowVisibility");

            seeThrough = serializedObject.FindProperty("seeThrough");
            seeThroughOccluderMask = serializedObject.FindProperty("seeThroughOccluderMask");
            seeThroughOccluderMaskAccurate = serializedObject.FindProperty("seeThroughOccluderMaskAccurate");
            seeThroughOccluderThreshold = serializedObject.FindProperty("seeThroughOccluderThreshold");
            seeThroughOccluderCheckInterval = serializedObject.FindProperty("seeThroughOccluderCheckInterval");
            seeThroughOccluderCheckIndividualObjects = serializedObject.FindProperty("seeThroughOccluderCheckIndividualObjects");
            seeThroughDepthOffset = serializedObject.FindProperty("seeThroughDepthOffset");
            seeThroughMaxDepth = serializedObject.FindProperty("seeThroughMaxDepth");
            seeThroughIntensity = serializedObject.FindProperty("seeThroughIntensity");
            seeThroughTintAlpha = serializedObject.FindProperty("seeThroughTintAlpha");
            seeThroughTintColor = serializedObject.FindProperty("seeThroughTintColor");
            seeThroughNoise = serializedObject.FindProperty("seeThroughNoise");
            seeThroughBorder = serializedObject.FindProperty("seeThroughBorder");
            seeThroughBorderWidth = serializedObject.FindProperty("seeThroughBorderWidth");
            seeThroughBorderColor = serializedObject.FindProperty("seeThroughBorderColor");
            seeThroughOrdered = serializedObject.FindProperty("seeThroughOrdered");
            seeThroughBorderOnly = serializedObject.FindProperty("seeThroughBorderOnly");
            seeThroughTexture = serializedObject.FindProperty("seeThroughTexture");
            seeThroughTextureScale = serializedObject.FindProperty("seeThroughTextureScale");
            seeThroughTextureUVSpace = serializedObject.FindProperty("seeThroughTextureUVSpace");
            seeThroughChildrenSortingMode = serializedObject.FindProperty("seeThroughChildrenSortingMode");

            targetFX = serializedObject.FindProperty("targetFX");
            targetFXTexture = serializedObject.FindProperty("targetFXTexture");
            targetFXRotationSpeed = serializedObject.FindProperty("targetFXRotationSpeed");
            targetFXInitialScale = serializedObject.FindProperty("targetFXInitialScale");
            targetFXEndScale = serializedObject.FindProperty("targetFXEndScale");
            targetFXScaleToRenderBounds = serializedObject.FindProperty("targetFXScaleToRenderBounds");
            targetFXUseEnclosingBounds = serializedObject.FindProperty("targetFXUseEnclosingBounds");
            targetFXSquare = serializedObject.FindProperty("targetFXSquare");
            targetFXOffset = serializedObject.FindProperty("targetFXOffset");
            targetFxCenterOnHitPosition = serializedObject.FindProperty("targetFxCenterOnHitPosition");
            targetFxAlignToNormal = serializedObject.FindProperty("targetFxAlignToNormal");
            targetFXAlignToGround = serializedObject.FindProperty("targetFXAlignToGround");
            targetFXFadePower = serializedObject.FindProperty("targetFXFadePower");
            targetFXGroundMaxDistance = serializedObject.FindProperty("targetFXGroundMaxDistance");
            targetFXGroundLayerMask = serializedObject.FindProperty("targetFXGroundLayerMask");
            targetFXColor = serializedObject.FindProperty("targetFXColor");
            targetFXCenter = serializedObject.FindProperty("targetFXCenter");
            targetFXTransitionDuration = serializedObject.FindProperty("targetFXTransitionDuration");
            targetFXStayDuration = serializedObject.FindProperty("targetFXStayDuration");
            targetFXVisibility = serializedObject.FindProperty("targetFXVisibility");
            targetFXStyle = serializedObject.FindProperty("targetFXStyle");
            targetFXFrameWidth = serializedObject.FindProperty("targetFXFrameWidth");
            targetFXCornerLength = serializedObject.FindProperty("targetFXCornerLength");
            targetFXFrameMinOpacity = serializedObject.FindProperty("targetFXFrameMinOpacity");
            targetFXRotationAngle = serializedObject.FindProperty("targetFXRotationAngle");
            targetFXGroundMinAltitude = serializedObject.FindProperty("targetFXGroundMinAltitude");

            iconFX = serializedObject.FindProperty("iconFX");
            iconFXAssetType = serializedObject.FindProperty("iconFXAssetType");
            iconFXPrefab = serializedObject.FindProperty("iconFXPrefab");
            iconFXCenter = serializedObject.FindProperty("iconFXCenter");
            iconFXMesh = serializedObject.FindProperty("iconFXMesh");
            iconFXLightColor = serializedObject.FindProperty("iconFXLightColor");
            iconFXDarkColor = serializedObject.FindProperty("iconFXDarkColor");
            iconFXRotationSpeed = serializedObject.FindProperty("iconFXRotationSpeed");
            iconFXAnimationOption = serializedObject.FindProperty("iconFXAnimationOption");
            iconFXAnimationAmount = serializedObject.FindProperty("iconFXAnimationAmount");
            iconFXAnimationSpeed = serializedObject.FindProperty("iconFXAnimationSpeed");
            iconFXScale = serializedObject.FindProperty("iconFXScale");
            iconFXScaleToRenderBounds = serializedObject.FindProperty("iconFXScaleToRenderBounds");
            iconFXOffset = serializedObject.FindProperty("iconFXOffset");
            iconFXTransitionDuration = serializedObject.FindProperty("iconFXTransitionDuration");
            iconFXStayDuration = serializedObject.FindProperty("iconFXStayDuration");

            labelEnabled = serializedObject.FindProperty("labelEnabled");
            labelText = serializedObject.FindProperty("labelText");
            labelTextSize = serializedObject.FindProperty("labelTextSize");
            labelColor = serializedObject.FindProperty("labelColor");
            labelPrefab = serializedObject.FindProperty("labelPrefab");
            labelVerticalOffset = serializedObject.FindProperty("labelVerticalOffset");
            lineLength = serializedObject.FindProperty("lineLength");
            labelFollowCursor = serializedObject.FindProperty("labelFollowCursor");
            labelTarget = serializedObject.FindProperty("labelTarget");
            labelMode = serializedObject.FindProperty("labelMode");
            labelShowInEditorMode = serializedObject.FindProperty("labelShowInEditorMode");
            labelAlignment = serializedObject.FindProperty("labelAlignment");

            hitFxInitialIntensity = serializedObject.FindProperty("hitFxInitialIntensity");
            hitFxMode = serializedObject.FindProperty("hitFxMode");
            hitFxFadeOutDuration = serializedObject.FindProperty("hitFxFadeOutDuration");
            hitFxColor = serializedObject.FindProperty("hitFxColor");
            hitFxRadius = serializedObject.FindProperty("hitFxRadius");
            hitFXTriggerMode = serializedObject.FindProperty("hitFXTriggerMode");

            cameraDistanceFade = serializedObject.FindProperty("cameraDistanceFade");
            cameraDistanceFadeNear = serializedObject.FindProperty("cameraDistanceFadeNear");
            cameraDistanceFadeFar = serializedObject.FindProperty("cameraDistanceFadeFar");

            useOriginalShader = serializedObject.FindProperty("useOriginalShader");
            customPassName = serializedObject.FindProperty("customPassName");

            thisEffect = (HighlightEffect)target;
            thisEffect.Refresh();
            GetShaderPassNames();
        }

        private void OnDisable () {
            EditorPrefs.SetBool(HP_GENERAL_SETTINGS, expandGeneralSettings);
            EditorPrefs.SetBool(HP_HIGHLIGHT_OPTIONS, expandHighlightOptions);
            EditorPrefs.SetBool(HP_ADVANCED_SETTINGS, expandAdvancedSettings);
        }

        public override void OnInspectorGUI () {

            forceRefresh = false;
            EditorGUILayout.Separator();

            // URP setup helpers
            pipe = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipe == null) {
                EditorGUILayout.HelpBox("You must assign the Universal Rendering Pipeline asset in Project Settings / Graphics. Then, add the Highlight Plus Scriptable Render Feature to the list of Renderer Features of the Forward Renderer.", MessageType.Error);
                if (GUILayout.Button("Watch Setup Video Tutorial")) {
                    Application.OpenURL("https://youtu.be/wXNS3gaBxHE");
                }
                return;
            }

            if (!HighlightPlusRenderPassFeature.installed) {
                EditorGUILayout.HelpBox("Highlight Plus Render Feature must be added to the list of features of the Forward Renderer in the Universal Rendering Pipeline asset.", MessageType.Warning);
                if (GUILayout.Button("Watch Setup Video Tutorial")) {
                    Application.OpenURL("https://youtu.be/wXNS3gaBxHE");
                }
                if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                    Selection.activeObject = pipe;
                }
                EditorGUILayout.Separator();
            }

            bool isManager = IsDefaultEffectUsedByManager();
            serializedObject.Update();

            bool isMeshObject = !thisEffect.spriteMode;

            if (foldoutBold == null) {
                foldoutBold = new GUIStyle(EditorStyles.foldout);
                foldoutBold.fontStyle = FontStyle.Bold;
            }

            EditorGUILayout.BeginHorizontal();
            HighlightProfile prevProfile = (HighlightProfile)profile.objectReferenceValue;
            EditorGUILayout.PropertyField(profile, new GUIContent("Profile", "Create or load stored presets."));
            if (profile.objectReferenceValue != null) {

                if (prevProfile != profile.objectReferenceValue) {
                    profileChanged = true;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(EditorGUIUtility.labelWidth));
                if (GUILayout.Button(new GUIContent("Create", "Creates a new profile which is a copy of the current settings."), GUILayout.Width(60))) {
                    CreateProfile();
                    profileChanged = false;
                    enableProfileApply = false;
                    GUIUtility.ExitGUI();
                    return;
                }
                if (GUILayout.Button(new GUIContent("Load", "Updates settings with the profile configuration."), GUILayout.Width(60))) {
                    profileChanged = true;
                }
                GUI.enabled = enableProfileApply;
                if (GUILayout.Button(new GUIContent("Save", "Updates profile configuration with changes in this inspector."), GUILayout.Width(60))) {
                    enableProfileApply = false;
                    profileChanged = false;
                    thisEffect.profile.Save(thisEffect);
                    EditorUtility.SetDirty(thisEffect.profile);
                    GUIUtility.ExitGUI();
                    return;
                }
                GUI.enabled = true;
                if (GUILayout.Button(new GUIContent("Locate", "Finds the profile in the project"), GUILayout.Width(60))) {
                    if (thisEffect.profile != null) {
                        Selection.activeObject = thisEffect.profile;
                        EditorGUIUtility.PingObject(thisEffect.profile);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(profileSync, new GUIContent("Sync With Profile", "If disabled, profile settings will only be loaded when clicking 'Load' which allows you to customize settings after loading a profile and keep those changes."));
                EditorGUILayout.BeginHorizontal();
            }
            else {
                if (GUILayout.Button(new GUIContent("Create", "Creates a new profile which is a copy of the current settings."), GUILayout.Width(60))) {
                    CreateProfile();
                    GUIUtility.ExitGUI();
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();


            if (isManager) {
                EditorGUILayout.HelpBox("These are default settings for highlighted objects. If the highlighted object already has a Highlight Effect component, those properties will be used.", MessageType.Info);
            }

            expandGeneralSettings = EditorGUILayout.Foldout(expandGeneralSettings, "General Settings", true, foldoutBold);
            if (expandGeneralSettings) {
                DrawLayerMaskField(camerasLayerMask);
                EditorGUILayout.PropertyField(ignoreObjectVisibility);
                if (thisEffect.staticChildren) {
                    EditorGUILayout.HelpBox("This GameObject or one of its children is marked as static. If highlight is not visible, add a MeshCollider to them (the MeshCollider can be disabled).", MessageType.Warning);
                }

                EditorGUILayout.PropertyField(reflectionProbes);

                if (isMeshObject) {
                    EditorGUILayout.PropertyField(normalsOption);
                }
                EditorGUILayout.PropertyField(optimizeSkinnedMesh);
                if (isMeshObject || optimizeSkinnedMesh.boolValue) {
                    EditorGUILayout.PropertyField(GPUInstancing);
                }
                EditorGUILayout.PropertyField(sortingPriority);
                EditorGUILayout.Separator();
            }

            if (!isManager) {
                EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
                if (isManager) {
                    EditorGUILayout.LabelField(new GUIContent("Highlighted", "Highlight state (controlled by Highlight Manager)."), new GUIContent(thisEffect.highlighted.ToString()));
                }
                else {
                    EditorGUILayout.PropertyField(highlighted);
                }
                EditorGUILayout.LabelField(new GUIContent("Selected", "Selection state (used by Highlight Trigger or Manager) when using multi-selection option."), new GUIContent(thisEffect.isSelected.ToString()));
                EditorGUILayout.Separator();
            }

            EditorGUILayout.BeginHorizontal();
            expandHighlightOptions = EditorGUILayout.Foldout(expandHighlightOptions, "Highlight Options", true, foldoutBold);
            if (!isMeshObject) {
                GUILayout.Label(new GUIContent("SPRITE MODE", "Highlight Effect over sprites. Some effects are not available in sprite mode"), EditorStyles.centeredGreyMiniLabel);
            }
            if (GUILayout.Button("Help", GUILayout.Width(50))) {
                EditorUtility.DisplayDialog("Quick Help", "Move the mouse over a setting for a short description.\n\nVisit kronnect.com's forum for support, questions and more cool assets.\n\nIf you like Highlight Plus please rate it or leave a review on the Asset Store! Thanks.", "Ok");
            }
            EditorGUILayout.EndHorizontal();
            if (expandHighlightOptions) {
                if (!isManager) {
                    EditorGUILayout.PropertyField(ignore, new GUIContent("Ignore", "This object won't be highlighted."));
                }
                if (!ignore.boolValue) {
                    EditorGUILayout.PropertyField(effectGroup, new GUIContent("Include", "Additional objects to highlight. Pro tip: when highlighting multiple objects at the same time include them in the same layer or under the same parent."));
                    if (effectGroup.intValue == (int)TargetOptions.LayerInScene || effectGroup.intValue == (int)TargetOptions.LayerInChildren) {
                        EditorGUI.indentLevel++;
                        DrawLayerMaskField(effectGroupLayer, "Layer");
                        EditorGUI.indentLevel--;
                    }
                    if (effectGroup.intValue != (int)TargetOptions.OnlyThisObject && effectGroup.intValue != (int)TargetOptions.Scripting) {
                        EditorGUI.indentLevel++;
                        if (effectGroup.intValue == (int)TargetOptions.RootToChildren) {
                            EditorGUILayout.HelpBox("Root To Children uses the top-most parent by default. Optionally add a Highlight Root component to any parent to specify the root.", MessageType.Info);
                        }
                        if (effectGroup.intValue != (int)TargetOptions.LayerInScene) {
                            EditorGUILayout.PropertyField(effectTarget, new GUIContent("Target", "The target object where the include option applies. By default, this same object."));
                        }
                        EditorGUILayout.PropertyField(effectNameFilter, new GUIContent("Object Name Filter"));
                        if (effectNameUseRegEx.boolValue && !string.IsNullOrEmpty(thisEffect.lastRegExError)) {
                            EditorGUILayout.HelpBox(thisEffect.lastRegExError, MessageType.Error);
                        }
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(effectNameUseRegEx, new GUIContent("Use Regular Expressions", "If enabled, the Object Name Filter is a regular expression."));
                        if (effectNameUseRegEx.boolValue) {
                            if (GUILayout.Button("Help", GUILayout.Width(50))) {
                                if (EditorUtility.DisplayDialog("Regular Expressions", "Check the online Microsoft documentation for regular expressions syntax. You can also use ChatGPT to obtain regular expressions patterns. Some examples:\n^[^A].* will match any name not starting with an A\n.*[^\\d]$ matches any name not ending in a number.", "Online Reference", "Close")) {
                                    Application.OpenURL("https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference");
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        if (isMeshObject) {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(combineMeshes);
                            if (combineMeshes.boolValue) {
                                if (GUILayout.Button("Refresh", GUILayout.Width(70))) {
                                    thisEffect.Refresh(true);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            if (combineMeshes.boolValue) {
                                string warning = thisEffect.ValidateCombineMeshes();
                                if (!string.IsNullOrEmpty(warning)) {
                                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                                }
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(alphaCutOff, new GUIContent("Alpha Cut Off", "Only for semi-transparent objects. Leave this to zero for normal opaque objects."));
                    EditorGUILayout.PropertyField(padding, new GUIContent("Padding"));
                    EditorGUILayout.PropertyField(fadeInDuration);
                    EditorGUILayout.PropertyField(fadeOutDuration);
                    EditorGUILayout.PropertyField(cameraDistanceFade);
                    if (cameraDistanceFade.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(cameraDistanceFadeNear, new GUIContent("Near Distance"));
                        EditorGUILayout.PropertyField(cameraDistanceFadeFar, new GUIContent("Far Distance"));
                        EditorGUI.indentLevel--;
                    }
                    // Show constant width for all quality levels now
                    EditorGUILayout.PropertyField(constantWidth, new GUIContent("Constant Width", "Compensates outline/glow width with depth increase."));
                    if (!constantWidth.boolValue) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(minimumWidth);
                        EditorGUI.indentLevel--;
                    }
                    if (isMeshObject) {
                        EditorGUILayout.PropertyField(outlineIndependent, new GUIContent("Independent", "Do not combine outline or glow with other highlighted objects."));
                    }
                }
            }

            expandAdvancedSettings = EditorGUILayout.Foldout(expandAdvancedSettings, "Advanced", true, foldoutBold);
            if (expandAdvancedSettings) {
                EditorGUI.indentLevel++;
                if (isMeshObject) {
                    EditorGUILayout.PropertyField(cullBackFaces);
                    EditorGUILayout.PropertyField(subMeshMask);
                }
                if ((outlineQuality.intValue == (int)QualityLevel.Highest && outline.floatValue > 0) || (glowQuality.intValue == (int)QualityLevel.Highest && glow.floatValue > 0)) {
                    GUI.enabled = true;
                } else {
                    GUI.enabled = false;
                }
                EditorGUILayout.PropertyField(flipY, new GUIContent("Flip Y Fix", "Flips outline/glow effect to fix bug introduced in Unity 2019.1.0 when VR is enabled."));
                GUI.enabled = true;
                EditorGUILayout.PropertyField(useOriginalShader, new GUIContent("Use Original Shader"));
                if (useOriginalShader.boolValue) {
                    EditorGUI.indentLevel++;
                    
                    if (passNames.Count > 0) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(customPassName, new GUIContent("Custom Pass Name", "A custom pass name in the original material shader that can be used to render the silhouette of the object - ideally this pass should write just a 1 (white). The DepthNormals pass can also be used."));
                        
                        int currentPassIndex = passNames.IndexOf(customPassName.stringValue);
                        int selectedIndex = EditorGUILayout.Popup(currentPassIndex, passNames.ToArray());
                        if (selectedIndex >= 0 && selectedIndex != currentPassIndex) {
                            customPassName.stringValue = passNames[selectedIndex];
                        }
                        EditorGUILayout.EndHorizontal();
                    } else {
                        EditorGUILayout.PropertyField(customPassName, new GUIContent("Custom Pass Name", "A custom pass name in the original material shader that can be used to render the silhouette of the object - ideally this pass should write just a 1 (white). The DepthNormals pass can also be used."));
                    }
                    if ((outlineQuality.intValue != (int)QualityLevel.Highest && outline.floatValue > 0) ||
                        (glowQuality.intValue != (int)QualityLevel.Highest && glow.floatValue > 0)) {
                        EditorGUILayout.HelpBox("This option only works with Outline or Glow in Highest Quality mode.", MessageType.Warning);
                    } 
                        EditorGUILayout.HelpBox("Make sure the selected pass won't be stripped in the build. Don't use passes that only work in Editor or that are not supported in the current rendering path (ie. use Gbuffer pass only in deferred).", MessageType.Info);
                    
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }

            if (!ignore.boolValue) {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(outline, "Outline", outline.floatValue > 0);
                if (outline.floatValue > 0) {
                    EditorGUI.indentLevel++;
                    if (isMeshObject) {
                        EditorGUILayout.BeginHorizontal();
                        QualityPropertyField(outlineQuality);
                        if (outlineQuality.intValue == (int)QualityLevel.Highest) {
                            GUILayout.Label("(Screen-Space Effect)");
                        }
                        else {
                            GUILayout.Label("(Mesh-based Effect)");
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    CheckVRSupport(outlineQuality.intValue);
                    if (outlineQuality.intValue == (int)QualityLevel.Highest) {
                        EditorGUILayout.PropertyField(outlineEdgeMode, new GUIContent("Edges"));
                        if (outlineEdgeMode.intValue == (int)OutlineEdgeMode.Any) {
                            EditorGUILayout.PropertyField(outlineEdgeThreshold, new GUIContent("Edge Detection Threshold"));
                        }
                        EditorGUILayout.PropertyField(outlineContourStyle, new GUIContent("Contour Style"));
                        EditorGUILayout.PropertyField(outlineWidth, new GUIContent("Width"));
                        EditorGUILayout.PropertyField(outlineBlurPasses, new GUIContent("Blur Passes"));
                        EditorGUILayout.PropertyField(outlineSharpness, new GUIContent("Sharpness"));
                        EditorGUILayout.PropertyField(outlineColorStyle, new GUIContent("Color Style"));
                        switch ((ColorStyle)outlineColorStyle.intValue) {
                            case ColorStyle.SingleColor:
                                EditorGUILayout.PropertyField(outlineColor, new GUIContent("Color"));
                                break;
                            case ColorStyle.Gradient:
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(outlineGradient, new GUIContent("Gradient"));
                                EditorGUILayout.PropertyField(outlineGradientKnee, new GUIContent("Knee"));
                                EditorGUILayout.PropertyField(outlineGradientPower, new GUIContent("Power"));
                                EditorGUI.indentLevel--;
                                break;
                        }
                    }
                    else {
                        EditorGUILayout.PropertyField(outlineWidth, new GUIContent("Width"));
                        EditorGUILayout.PropertyField(outlineColorStyle, new GUIContent("Color Style"));
                        switch ((ColorStyle)outlineColorStyle.intValue) {
                            case ColorStyle.SingleColor:
                                EditorGUILayout.PropertyField(outlineColor, new GUIContent("Color"));
                                break;
                            case ColorStyle.Gradient:
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(outlineGradient, new GUIContent("Gradient"));
                                EditorGUILayout.PropertyField(outlineGradientInLocalSpace, new GUIContent("In Local Space"));
                                EditorGUI.indentLevel--;
                                break;
                        }
                    }

                    if (outlineQuality.intValue == (int)QualityLevel.Highest) {
                        if (outlineEdgeMode.intValue != (int)OutlineEdgeMode.Any) {
                            CheckDepthTextureSupport("Highest Quality");
                            EditorGUILayout.PropertyField(outlineDownsampling, new GUIContent("Downsampling"));
                        }
                        if (glow.floatValue > 0 && glowQuality.intValue == (int)QualityLevel.Highest) {
                            outlineVisibility.intValue = glowVisibility.intValue;
                        }
                    }

                    EditorGUILayout.PropertyField(outlineVisibility, new GUIContent("Visibility"));
                    EditorGUILayout.PropertyField(outlineMaskMode, new GUIContent("Mask Mode"));

                    if (outlineQuality.intValue == (int)QualityLevel.Highest) {
                        EditorGUILayout.PropertyField(outlinePixelation, new GUIContent("Pixelation"));
                        EditorGUILayout.PropertyField(outlineStylized, new GUIContent("Stylized Effect"));
                        if (outlineStylized.boolValue) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(outlinePatternDistortionTexture, new GUIContent("Distortion Texture (R)", "Distortion texture for the stylized effect. Only red channel is used."));
                            if (outlinePatternDistortionTexture.objectReferenceValue != null) {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(outlinePatternDistortionAmount, new GUIContent("Distortion Amount"));
                                EditorGUILayout.PropertyField(outlinePatternScale, new GUIContent("Scale"));
                                EditorGUILayout.PropertyField(outlinePatternStopMotionScale, new GUIContent("Stop Motion Speed"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUILayout.PropertyField(outlinePattern, new GUIContent("Pattern (R)", "Pattern for the stylized effect. Only red channel is used."));
                            if (outlinePattern.objectReferenceValue != null) {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(outlinePatternThreshold, new GUIContent("Threshold"));
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.PropertyField(outlineDashed, new GUIContent("Dashed Effect"));
                        if (outlineDashed.boolValue) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(outlineDashWidth, new GUIContent("Width"));
                            EditorGUILayout.PropertyField(outlineDashGap, new GUIContent("Gap"));
                            EditorGUILayout.PropertyField(outlineDashSpeed, new GUIContent("Speed"));
                            EditorGUI.indentLevel--;
                        }
                        if (!constantWidth.boolValue) {
                            EditorGUILayout.PropertyField(outlineDistanceScaleBias, new GUIContent("Distance Scale Bias", "Controls how quickly the outline effect scales down with distance. Lower values make the effect fade faster with distance."));
                        }
                        EditorGUILayout.PropertyField(outlineBlitDebug, new GUIContent("Debug View", "Shows the blitting rectangle on the screen."));
                        if (!Application.isPlaying && outlineBlitDebug.boolValue && (!HighlightPlusRenderPassFeature.showingInEditMode || !highlighted.boolValue)) {
                            EditorGUILayout.HelpBox("Enable \"Preview In Editor\" in the Highlight Render Feature and \"Highlighted\" to display the outline Debug View.", MessageType.Warning);
                        }
                        EditorGUILayout.PropertyField(extraCoveragePixels);
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(glow, "Outer Glow", glow.floatValue > 0);
                if (glow.floatValue > 0) {
                    EditorGUI.indentLevel++;
                    if (isMeshObject) {
                        EditorGUILayout.BeginHorizontal();
                        QualityPropertyField(glowQuality);
                        if (glowQuality.intValue == (int)QualityLevel.Highest) {
                            GUILayout.Label("(Screen-Space Effect)");
                        }
                        else {
                            GUILayout.Label("(Mesh-based Effect)");
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    CheckVRSupport(glowQuality.intValue);
                    if (glowQuality.intValue == (int)QualityLevel.Highest) {
                        CheckDepthTextureSupport("Highest Quality");
                        EditorGUILayout.PropertyField(glowHighPrecision, new GUIContent("High Precision"));
                        EditorGUILayout.PropertyField(outlineContourStyle, new GUIContent("Contour Style"));
                        EditorGUILayout.PropertyField(glowWidth, new GUIContent("Width"));
                        EditorGUILayout.PropertyField(glowHQColor, new GUIContent("Color"));
                        EditorGUILayout.PropertyField(glowBlurMethod, new GUIContent("Blur Method", "Gaussian: better quality. Kawase: faster."));
                        EditorGUILayout.PropertyField(glowDownsampling, new GUIContent("Downsampling"));
                        EditorGUILayout.PropertyField(glowPixelation, new GUIContent("Pixelation"));
                    }
                    else {
                        EditorGUILayout.PropertyField(glowWidth, new GUIContent("Width"));
                    }
                    EditorGUILayout.PropertyField(glowMaskMode, new GUIContent("Mask Mode"));
                    if (glowQuality.intValue == (int)QualityLevel.Highest) {
                        EditorGUILayout.PropertyField(glowVisibility, new GUIContent("Visibility"));
                        EditorGUILayout.PropertyField(glowBlendMode, new GUIContent("Blend Mode"));
                    }
                    else {
                        EditorGUILayout.PropertyField(glowVisibility, new GUIContent("Visibility"));
                        EditorGUILayout.PropertyField(glowDithering, new GUIContent("Dithering"));
                        if (glowDithering.floatValue > 0) {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(glowDitheringStyle, new GUIContent("Style"));
                            if (glowDitheringStyle.intValue == (int)GlowDitheringStyle.Pattern) {
                                EditorGUILayout.PropertyField(glowMagicNumber1, new GUIContent("Magic Number 1"));
                                EditorGUILayout.PropertyField(glowMagicNumber2, new GUIContent("Magic Number 2"));
                            }
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.PropertyField(glowBlendPasses, new GUIContent("Blend Passes"));
                        if (!glowBlendPasses.boolValue) {
                            if (thisEffect.glowPasses != null) {
                                for (int k = 0; k < thisEffect.glowPasses.Length - 1; k++) {
                                    if (thisEffect.glowPasses[k].offset > thisEffect.glowPasses[k + 1].offset) {
                                        EditorGUILayout.HelpBox("Glow pass " + k + " has a greater offset than the next one. Reduce it to ensure the next glow pass is visible.", MessageType.Warning);
                                    }
                                }
                            }
                        }
                        EditorGUILayout.PropertyField(glowPasses, true);
                    }
                    EditorGUILayout.PropertyField(glowAnimationSpeed, new GUIContent("Animation Speed"));
                    if (glowQuality.intValue == (int)QualityLevel.Highest && !constantWidth.boolValue) {
                        EditorGUILayout.PropertyField(glowDistanceScaleBias, new GUIContent("Distance Scale Bias", "Controls how quickly the glow effect scales down with distance. Lower values make the effect fade faster with distance."));
                    }
                    if (glowQuality.intValue == (int)QualityLevel.Highest) {
                        EditorGUILayout.PropertyField(glowBlitDebug, new GUIContent("Debug View", "Shows the blitting rectangle on the screen."));
                        if (!Application.isPlaying && glowBlitDebug.boolValue && (!HighlightPlusRenderPassFeature.showingInEditMode || !highlighted.boolValue)) {
                            EditorGUILayout.HelpBox("Enable \"Preview In Editor\" in the Highlight Render Feature and \"Highlighted\" to display the glow Debug View.", MessageType.Warning);
                        }
                        EditorGUILayout.PropertyField(extraCoveragePixels);
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();

                if (isMeshObject) {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    DrawSectionField(innerGlow, "Inner Glow", innerGlow.floatValue > 0);
                    if (innerGlow.floatValue > 0) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(innerGlowColor, new GUIContent("Color"));
                        EditorGUILayout.PropertyField(innerGlowWidth, new GUIContent("Width"));
                        EditorGUILayout.PropertyField(innerGlowPower, new GUIContent("Power"));
                        EditorGUILayout.PropertyField(innerGlowBlendMode, new GUIContent("Blend Mode"));
                        EditorGUILayout.PropertyField(innerGlowVisibility, new GUIContent("Visibility"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(overlay, "Overlay", overlay.floatValue > 0);
                if (overlay.floatValue > 0) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(overlayMode, new GUIContent("Mode"));
                    EditorGUILayout.PropertyField(overlayColor, new GUIContent("Color"));
                    EditorGUILayout.PropertyField(overlayTexture, new GUIContent("Texture"));
                    if (overlayTexture.objectReferenceValue != null) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(overlayTextureScale, new GUIContent("Scale"));
                        if ((TextureUVSpace)overlayTextureUVSpace.intValue != TextureUVSpace.Triplanar) {
                            EditorGUILayout.PropertyField(overlayTextureScrolling, new GUIContent("Scrolling"));
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(overlayTextureUVSpace, new GUIContent("UV Space"));
                    EditorGUILayout.PropertyField(overlayPattern, new GUIContent("Pattern"));
                    if ((HighlightPlus.OverlayPattern)overlayPattern.enumValueIndex != HighlightPlus.OverlayPattern.None) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(overlayPatternScrolling, new GUIContent("Scrolling"));
                        EditorGUILayout.PropertyField(overlayPatternScale, new GUIContent("Scale"));
                        EditorGUILayout.PropertyField(overlayPatternSize, new GUIContent("Size/Thickness"));
                        EditorGUILayout.PropertyField(overlayPatternSoftness, new GUIContent("Softness"));
                        EditorGUILayout.PropertyField(overlayPatternRotation, new GUIContent("Rotation"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(overlayBlending, new GUIContent("Blending"));
                    EditorGUILayout.PropertyField(overlayMinIntensity, new GUIContent("Min Intensity"));
                    EditorGUILayout.PropertyField(overlayAnimationSpeed, new GUIContent("Animation Speed"));
                    EditorGUILayout.PropertyField(overlayVisibility, new GUIContent("Visibility"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(targetFX, "Target", targetFX.boolValue);
                if (targetFX.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(targetFXStyle, new GUIContent("Style"));
                    if (targetFXStyle.intValue == (int)TargetFXStyle.Texture) {
                        EditorGUILayout.PropertyField(targetFXTexture, new GUIContent("Texture", "The texture that contains the shape to be drawn over the highlighted object."));
                    }
                    else {
                        EditorGUILayout.PropertyField(targetFXFrameWidth, new GUIContent("Width"));
                        EditorGUILayout.PropertyField(targetFXCornerLength, new GUIContent("Length"));
                        EditorGUILayout.PropertyField(targetFXFrameMinOpacity, new GUIContent("Min Opacity"));
                    }
                    EditorGUILayout.PropertyField(targetFXColor, new GUIContent("Color"));
                    EditorGUILayout.PropertyField(targetFXUseEnclosingBounds, new GUIContent("Use Enclosing Bounds"));
                    if (!targetFXUseEnclosingBounds.boolValue) {
                        EditorGUILayout.PropertyField(targetFXCenter, new GUIContent("Center", "Optionally assign a transform. Target will follow transform. If the object is skinned, you can also assign a bone to reflect currenct animation position."));
                    }
                    EditorGUILayout.PropertyField(targetFXOffset, new GUIContent("Offset"));
                    EditorGUILayout.PropertyField(targetFxCenterOnHitPosition, new GUIContent("Center On Hit Position"));
                    EditorGUILayout.PropertyField(targetFxAlignToNormal, new GUIContent("Align To Hit Normal"));
                    EditorGUILayout.PropertyField(targetFXRotationSpeed, new GUIContent("Rotation Speed"));
                    EditorGUILayout.PropertyField(targetFXRotationAngle, new GUIContent("Rotation Angle"));
                    EditorGUILayout.PropertyField(targetFXInitialScale, new GUIContent("Initial Scale"));
                    EditorGUILayout.PropertyField(targetFXEndScale, new GUIContent("End Scale"));
                    EditorGUILayout.PropertyField(targetFXScaleToRenderBounds, new GUIContent("Scale To Object Bounds"));
                    if (targetFXScaleToRenderBounds.boolValue) {
                        EditorGUILayout.PropertyField(targetFXSquare, new GUIContent("Square"));
                    }
                    EditorGUILayout.PropertyField(targetFXAlignToGround, new GUIContent("Align To Ground"));
                    if (targetFXAlignToGround.boolValue) {
                        CheckDepthTextureSupport("Align To Ground option");
                        EditorGUI.indentLevel++;
                        if (thisEffect.includedObjectsCount > 1 && targetFXCenter.objectReferenceValue == null && effectGroup.intValue != (int)TargetOptions.OnlyThisObject) {
                            EditorGUILayout.HelpBox("It's recommended to specify in the 'Center' property above, the specific object used to position the target fx image (will be rendered under that object on the ground).", MessageType.Info);
                        }
                        EditorGUILayout.PropertyField(targetFXGroundMaxDistance, new GUIContent("Ground Max Distance"));
                        DrawLayerMaskField(targetFXGroundLayerMask, "Ground Layer Mask");
                        if ((targetFXGroundLayerMask.intValue & (1 << thisEffect.gameObject.layer)) != 0) {
                            EditorGUILayout.HelpBox("Ground Layer Mask should not include this object layer.", MessageType.Warning);
                        }
                        else {
                            if (thisEffect.alignToGroundTried && !thisEffect.alignToGroundHitGood) {
                                EditorGUILayout.HelpBox("The Target Fx image is not being aligned because the ground was not found. Make sure it has a collider and verify the ground layer mask.", MessageType.Warning);
                            }
                        }
                        EditorGUILayout.PropertyField(targetFXFadePower, new GUIContent("Fade Power"));
                        EditorGUILayout.PropertyField(targetFXGroundMinAltitude, new GUIContent("Ground Min Altitude"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(targetFXTransitionDuration, new GUIContent("Transition Duration"));
                    EditorGUILayout.PropertyField(targetFXStayDuration, new GUIContent("Stay Duration"));
                    EditorGUILayout.PropertyField(targetFXVisibility, new GUIContent("Visibility"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(iconFX, "Icon", iconFX.boolValue);
                if (iconFX.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(iconFXAssetType, new GUIContent("Asset Type"));
                    if (iconFXAssetType.intValue == (int)IconAssetType.Mesh) {
                        EditorGUILayout.PropertyField(iconFXMesh, new GUIContent("Mesh"));
                        EditorGUILayout.PropertyField(iconFXLightColor, new GUIContent("Light Color"));
                        EditorGUILayout.PropertyField(iconFXDarkColor, new GUIContent("Dark Color"));
                    }
                    else {
                        EditorGUILayout.PropertyField(iconFXPrefab, new GUIContent("Prefab"));
                    }
                    EditorGUILayout.PropertyField(iconFXCenter, new GUIContent("Center", "Optionally assign a transform. Icon will follow transform. If the object is skinned, you can also assign a bone to reflect currenct animation position."));
                    EditorGUILayout.PropertyField(iconFXOffset, new GUIContent("Offset"));
                    EditorGUILayout.PropertyField(iconFXRotationSpeed, new GUIContent("Rotation Speed"));
                    EditorGUILayout.PropertyField(iconFXAnimationOption, new GUIContent("Animation"));
                    if (iconFXAnimationOption.intValue != 0) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(iconFXAnimationAmount, new GUIContent("Amount"));
                        EditorGUILayout.PropertyField(iconFXAnimationSpeed, new GUIContent("Speed"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(iconFXScale, new GUIContent("Scale"));
                    EditorGUILayout.PropertyField(iconFXScaleToRenderBounds, new GUIContent("Scale To Object Bounds"));
                    EditorGUILayout.PropertyField(iconFXTransitionDuration, new GUIContent("Transition Duration"));
                    EditorGUILayout.PropertyField(iconFXStayDuration, new GUIContent("Stay Duration"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                DrawSectionField(labelEnabled, "Label", labelEnabled.boolValue);
                if (labelEnabled.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(labelMode, new GUIContent("Mode"));
                    EditorGUILayout.PropertyField(labelShowInEditorMode, new GUIContent("Always Show In Editor Mode"));
                    EditorGUILayout.PropertyField(labelAlignment, new GUIContent("Alignment"));
                    EditorGUILayout.PropertyField(labelText, new GUIContent("Text"));
                    EditorGUILayout.PropertyField(labelTextSize, new GUIContent("Text Size"));
                    EditorGUILayout.PropertyField(labelColor, new GUIContent("Color"));
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(labelPrefab, new GUIContent("Prefab", "The prefab to use for the label. Must contain a Canvas and TextMeshProUGUI component."));
                    if (GUILayout.Button(new GUIContent("Refresh", "Refresh the prefab to ensure label contains the latest edits."), GUILayout.Width(70))) {
                        thisEffect.RefreshLabel();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(labelTarget, new GUIContent("Target", "The anchor for the label or leave this empty to use the center of the object"));
                    EditorGUILayout.PropertyField(labelVerticalOffset, new GUIContent("Vertical Offset", "The vertical offset of the label"));
                    EditorGUILayout.PropertyField(lineLength, new GUIContent("Line Length"));
                    EditorGUILayout.PropertyField(labelFollowCursor, new GUIContent("Follow Cursor"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }

            if (isMeshObject) {

                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.PropertyField(seeThrough);
                if (seeThrough.intValue != (int)SeeThroughMode.Never) {
                    if (isManager && seeThrough.intValue == (int)SeeThroughMode.AlwaysWhenOccluded) {
                        EditorGUILayout.HelpBox("This option is not valid in Manager.\nTo make an object always visible add a Highlight Effect component to the gameobject and enable this option on the component.", MessageType.Error);
                    }
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(seeThroughOccluderMask, new GUIContent("Occluder Layer"));
                    if (seeThroughOccluderMask.intValue > 0) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(seeThroughOccluderMaskAccurate, new GUIContent("Accurate"));
                        EditorGUILayout.PropertyField(seeThroughOccluderThreshold, new GUIContent("Radius Threshold", "Multiplier to the object bounds. Making the bounds smaller prevents false occlusion tests."));
                        EditorGUILayout.PropertyField(seeThroughOccluderCheckInterval, new GUIContent("Check Interval", "Interval in seconds between occlusion tests."));
                        EditorGUILayout.PropertyField(seeThroughOccluderCheckIndividualObjects, new GUIContent("Check Individual Objects"));
                        if (!showCurrentOccluders && Camera.main != null) {
                            GUI.enabled = Application.isPlaying;
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("", GUILayout.Width(30));
                            if (GUILayout.Button(" Show Current Occluders (only during Play Mode) ")) {
                                showCurrentOccluders = true;
                            }
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                            GUI.enabled = true;
                        }
                        if (showCurrentOccluders) {
                            thisEffect.GetOccluders(Camera.main, occluders);
                            int count = occluders != null ? occluders.Count : 0;
                            if (count == 0) {
                                EditorGUILayout.LabelField("No occluders found (using main camera)");
                            }
                            else {
                                EditorGUILayout.LabelField("Occluders found (using main camera):");
                                for (int k = 0; k < count; k++) {
                                    if (occluders[k] == null) continue;
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField(occluders[k].name);
                                    if (GUILayout.Button("Select")) {
                                        Selection.activeGameObject = occluders[k].gameObject;
                                        GUIUtility.ExitGUI();
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(seeThroughDepthOffset, new GUIContent("Depth Offset" + ((seeThroughDepthOffset.floatValue > 0) ? " •" : "")));
                    EditorGUILayout.PropertyField(seeThroughMaxDepth, new GUIContent("Max Depth" + ((seeThroughMaxDepth.floatValue > 0) ? " •" : "")));
                    if (seeThroughDepthOffset.floatValue > 0 || seeThroughMaxDepth.floatValue > 0) {
                        CheckDepthTextureSupport("See-Through Depth Options");
                    }
                    EditorGUILayout.PropertyField(seeThroughIntensity, new GUIContent("Intensity"));
                    EditorGUILayout.PropertyField(seeThroughTintColor, new GUIContent("Color"));
                    EditorGUILayout.PropertyField(seeThroughTintAlpha, new GUIContent("Color Blend"));
                    EditorGUILayout.PropertyField(seeThroughNoise, new GUIContent("Noise"));
                    EditorGUILayout.PropertyField(seeThroughTexture, new GUIContent("Texture"));
                    if (seeThroughTexture.objectReferenceValue != null) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(seeThroughTextureUVSpace, new GUIContent("UV Space"));
                        EditorGUILayout.PropertyField(seeThroughTextureScale, new GUIContent("Texture Scale"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(seeThroughBorder, new GUIContent("Border When Hidden" + ((seeThroughBorder.floatValue > 0) ? " •" : "")));
                    if (seeThroughBorder.floatValue > 0) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(seeThroughBorderWidth, new GUIContent("Width"));
                        EditorGUILayout.PropertyField(seeThroughBorderColor, new GUIContent("Color"));
                        EditorGUILayout.PropertyField(seeThroughBorderOnly, new GUIContent("Border Only"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.PropertyField(seeThroughChildrenSortingMode, new GUIContent("Children Sorting Mode"));
                    EditorGUILayout.PropertyField(seeThroughOrdered, new GUIContent("Ordered"));

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawSectionField(hitFxInitialIntensity, "Hit FX", hitFxInitialIntensity.floatValue > 0);
            if (hitFxInitialIntensity.floatValue > 0) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(hitFxMode, new GUIContent("Style"));
                EditorGUILayout.PropertyField(hitFXTriggerMode, new GUIContent("Trigger Mode"));
                EditorGUILayout.PropertyField(hitFxFadeOutDuration, new GUIContent("Fade Out Duration"));
                EditorGUILayout.PropertyField(hitFxColor, new GUIContent("Color"));
                if ((HitFxMode)hitFxMode.intValue == HitFxMode.LocalHit) {
                    EditorGUILayout.PropertyField(hitFxRadius, new GUIContent("Radius"));
                }

                if (!Application.isPlaying) {
                    EditorGUILayout.HelpBox("Enter Play Mode to test this feature. In your code, call effect.HitFX() method to execute this hit effect.", MessageType.Info);
                }
                else {
                    EditorGUI.indentLevel++;
                    if (GUILayout.Button("Execute Hit")) {
                        thisEffect.HitFX();
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

            if (serializedObject.ApplyModifiedProperties() || forceRefresh || profileChanged || "UndoRedoPerformed".Equals(Event.current.commandName)) {
                if (thisEffect.profile != null) {
                    if (profileChanged) {
                        thisEffect.profile.Load(thisEffect);
                        EditorUtility.SetDirty(target);
                        profileChanged = false;
                        enableProfileApply = false;
                    }
                    else {
                        enableProfileApply = true;
                    }
                }

                foreach (HighlightEffect effect in targets) {
                    effect.Refresh();
                    effect.ResetHighlightStartTime();
                }
            }
        }

        void DrawLayerMaskField (SerializedProperty layerMaskProperty, string label = null) {
            GUIContent propertyLabel = new GUIContent(layerMaskProperty.displayName, layerMaskProperty.tooltip);
            if (!string.IsNullOrEmpty(label)) {
                propertyLabel.text = label;
            }

            EditorGUI.BeginChangeCheck();

            // Convierte el valor del LayerMask en un entero
            int oldLayerMaskValue = layerMaskProperty.intValue;

            // Obtén los nombres de las capas y transforma el valor del LayerMask
            string[] layerNames = GetLayerNames();
            int newLayerMaskValue = EditorGUILayout.MaskField(propertyLabel, UnityEditorInternal.InternalEditorUtility.LayerMaskToConcatenatedLayersMask(oldLayerMaskValue), layerNames);

            if (EditorGUI.EndChangeCheck()) {
                // Aplica el nuevo valor de LayerMask tras la conversión adecuada
                layerMaskProperty.intValue = UnityEditorInternal.InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(newLayerMaskValue);
                forceRefresh = true;
            }
        }

        string[] GetLayerNames () {
            List<string> layerNames = new List<string>();
            for (int i = 0; i < 32; i++) {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName)) {
                    layerNames.Add(layerName);
                }
            }
            return layerNames.ToArray();
        }


        void DrawSectionField (SerializedProperty property, string label, bool active) {
            EditorGUILayout.PropertyField(property, new GUIContent(active ? label + " •" : label));
        }

        void CheckVRSupport (int qualityLevel) {
            if (qualityLevel == (int)QualityLevel.Highest && PlayerSettings.virtualRealitySupported) {
                if (PlayerSettings.stereoRenderingPath != StereoRenderingPath.MultiPass) {
                    EditorGUILayout.HelpBox("Highest Quality only supports VR Multi-Pass as CommandBuffers do not support this VR mode yet. Either switch to 'High Quality' or change VR Stereo mode to Multi-Pass.", MessageType.Error);
                }
            }
        }

        List<string> GetShaderPassNames() {
            passNames.Clear();
            renderers.Clear();

            thisEffect.GetRenderers(renderers);

            // Get pass names from the first material found
            foreach (Renderer renderer in renderers) {
                if (renderer == null || renderer.sharedMaterials == null) continue;
                
                foreach (Material mat in renderer.sharedMaterials) {
                    if (mat == null || mat.shader == null) continue;
                    Shader shader = mat.shader;
                    
                    var shaderData = ShaderUtil.GetShaderData(shader);
                    if (shaderData != null && shaderData.SubshaderCount > 0) {
                            var subShader = shaderData.GetSubshader(0);
                            for (int j = 0; j < subShader.PassCount; j++) {
                                var pass = subShader.GetPass(j);
                                string passName = pass.Name;
                                if (string.IsNullOrEmpty(passName)) {
                                    passName = "Pass #" + j;
                                }
                                passNames.Add(passName);
                            }
                            return passNames; // Return immediately after processing first valid material/subshader
                    }
                }
            }
            return passNames;
        }

        void CheckDepthTextureSupport (string feature) {
#if !UNITY_2021_2_OR_NEWER
            if (pipe == null) return;
            if (!pipe.supportsCameraDepthTexture && !thisEffect.spriteMode) {
                EditorGUILayout.HelpBox(feature + " requires Depth Texture support and currently it's not enabled in the Rendering Pipeline asset.", MessageType.Error);
                if (pipe != null && GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                    Selection.activeObject = pipe;
                }
                EditorGUILayout.Separator();
            }
#endif
        }

        static readonly int[] qualityValues = { 0, 3, 1, 2 };
        static readonly GUIContent[] qualityTexts = { new GUIContent("Fastest"), new GUIContent("Medium"), new GUIContent("High"), new GUIContent("Highest") };

        public static void QualityPropertyField (SerializedProperty prop) {
            prop.intValue = EditorGUILayout.IntPopup(new GUIContent("Quality", "Default and High use a mesh displacement technique. Highest quality can provide best look and also performance depending on the complexity of mesh."), prop.intValue, qualityTexts, qualityValues);
        }

        bool IsDefaultEffectUsedByManager () {
            MonoBehaviour[] components = thisEffect.GetComponents<MonoBehaviour>();
            if (components != null) {
                for (int k = 0; k < components.Length; k++) {
                    if (components[k] == null || !components[k].enabled)
                        continue;
                    string name = components[k].GetType().Name;
                    if ("HighlightManager".Equals(name)) return true;
                }
            }
            return false;
        }

        #region Profile handling

        void CreateProfile () {

            HighlightProfile newProfile = CreateInstance<HighlightProfile>();
            newProfile.Save(thisEffect);

            AssetDatabase.CreateAsset(newProfile, "Assets/Highlight Plus Profile.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newProfile;

            thisEffect.profile = newProfile;
        }

        #endregion

#pragma warning restore 0618

    }

}