﻿using System;
using UnityEngine;

namespace HighlightPlus {

    [CreateAssetMenu(menuName = "Highlight Plus Profile", fileName = "Highlight Plus Profile", order = 100)]
    [HelpURL("https://www.dropbox.com/s/1p9h8xys68lm4a3/Documentation.pdf?dl=0")]
    public class HighlightProfile : ScriptableObject {

        [Tooltip("Different options to specify which objects are affected by this Highlight Effect component.")]
        public TargetOptions effectGroup = TargetOptions.Children;

        [Tooltip("The layer that contains the affected objects by this effect when effectGroup is set to LayerMask.")]
        public LayerMask effectGroupLayer = -1;

        [Tooltip("Only include objects whose names contains this text.")]
        public string effectNameFilter;

        [Tooltip("Use RegEx to determine if an object name matches the effectNameFilter.")]
        public bool effectNameUseRegEx;

        [Tooltip("Combine meshes of all objects in this group affected by Highlight Effect reducing draw calls.")]
        public bool combineMeshes;

        [Tooltip("The alpha threshold for transparent cutout objects. Pixels with alpha below this value will be discarded.")]
        [Range(0, 1)]
        public float alphaCutOff;

        [Tooltip("If back facing triangles are ignored.Backfaces triangles are not visible but you may set this property to false to force highlight effects to act on those triangles as well.")]
        public bool cullBackFaces = true;

        [Tooltip("Normals handling option:\nPreserve original: use original mesh normals.\nSmooth: average normals to produce a smoother outline/glow mesh based effect.\nReorient: recomputes normals based on vertex direction to centroid.")]
        public NormalsOption normalsOption;

        public float fadeInDuration;
        public float fadeOutDuration;

        [Tooltip("Fades out effects based on distance to camera")]
        public bool cameraDistanceFade;

        [Tooltip("The closest distance particles can get to the camera before they fade from the camera's view.")]
        public float cameraDistanceFadeNear;

        [Tooltip("The farthest distance particles can get away from the camera before they fade from the camera's view.")]
        public float cameraDistanceFadeFar = 1000;

        [Tooltip("Keeps the outline/glow size unaffected by object distance.")]
        public bool constantWidth = true;

        [Tooltip("Increases the screen coverage for the outline/glow to avoid cuts when using cloth or vertex shader that transform mesh vertices")]
        public int extraCoveragePixels;

        [Tooltip("Minimum width when the constant width option is not used")]
        [Range(0, 1)]
        public float minimumWidth;

        [Range(0, 1)]
        [Tooltip("Intensity of the overlay effect. A value of 0 disables the overlay completely.")]
        public float overlay;
        public OverlayMode overlayMode = OverlayMode.WhenHighlighted;
        [ColorUsage(showAlpha: false, hdr: true)] public Color overlayColor = Color.yellow;
        public float overlayAnimationSpeed = 1f;
        [Range(0, 1)]
        public float overlayMinIntensity = 0.5f;
        [Range(0, 1)]
        [Tooltip("Controls the blending or mix of the overlay color with the natural colors of the object.")]
        public float overlayBlending = 1.0f;
        [Tooltip("Optional overlay texture.")]
        public Texture2D overlayTexture;
        public TextureUVSpace overlayTextureUVSpace;
        public float overlayTextureScale = 1f;
        public Vector2 overlayTextureScrolling;
        public Visibility overlayVisibility = Visibility.Normal;

        [Tooltip("Optional overlay pattern texture.")]
        public OverlayPattern overlayPattern = OverlayPattern.None;
        public Vector2 overlayPatternScrolling;
        [Tooltip("Scale of the overlay pattern")]
        [Range(1f, 100f)]
        public float overlayPatternScale = 10f;
        [Tooltip("Size/Thickness of the overlay pattern")]
        [Range(0.01f, 1f)]
        public float overlayPatternSize = 0.15f;
        [Tooltip("Softness of the overlay pattern")]
        [Range(0.01f, 0.5f)]
        public float overlayPatternSoftness = 0.02f;
        [Tooltip("Rotation angle for the overlay pattern in degrees")]
        [Range(-180f, 180f)]
        public float overlayPatternRotation = 0f;


        [Range(0, 1)]
        [Tooltip("Intensity of the outline. A value of 0 disables the outline completely.")]
        public float outline = 1f;
        [ColorUsage(true, true)] public Color outlineColor = Color.black;
        public ColorStyle outlineColorStyle = ColorStyle.SingleColor;
        [GradientUsage(hdr: true, ColorSpace.Linear)] public Gradient outlineGradient;
        public bool outlineGradientInLocalSpace;
        [Range(1, 3)]
        public int outlineBlurPasses = 2;
        public float outlineWidth = 0.45f;
        public QualityLevel outlineQuality = QualityLevel.High;
        public OutlineEdgeMode outlineEdgeMode = OutlineEdgeMode.Exterior;
        public float outlineEdgeThreshold = 0.995f;
        [Tooltip("Controls how quickly the outline effect scales down with distance when constant width is disabled. Lower values make the effect fade faster with distance.")]
        public float outlineDistanceScaleBias = 25f;
        public float outlineSharpness = 1f;
        [Range(1, 8)]
        [Tooltip("Reduces the quality of the outline but improves performance a bit.")]
        public int outlineDownsampling = 1;
        public ContourStyle outlineContourStyle = ContourStyle.AroundVisibleParts;
        public float outlineGradientKnee = 0.4f;
        public float outlineGradientPower = 8f;

        [Tooltip("Enables stylized outline effect.")]
        public bool outlineStylized;
        [Tooltip("Pattern texture used for the stylized outline effect.")]
        public Texture2D outlinePattern;
        [Tooltip("Scale of the pattern texture.")]
        public float outlinePatternScale = 0.3f;
        [Tooltip("Threshold for the pattern texture.")]
        [Range(0, 1)]
        public float outlinePatternThreshold = 0.3f;
        [Tooltip("Distortion amount for the pattern texture.")]
        [Range(0, 1.5f)]
        public float outlinePatternDistortionAmount = 0.5f;
        [Tooltip("Stop motion scale for the distortion effect.")]
        public float outlinePatternStopMotionScale = 5f;
        [Tooltip("Distortion texture used for the stylized outline effect.")]
        public Texture2D outlinePatternDistortionTexture;
        [Tooltip("Adds a empty margin between the outline mesh and the effects")]
        [Range(0, 1)]
        public float padding;

        [Tooltip("Makes the outline pixelated. A value of 0 disables pixelation. Higher values produce bigger pixels.")]
        [Range(0, 32)]
        public int outlinePixelation;

        [Tooltip("Enables dashed outline effect.")]
        public bool outlineDashed;
        [Tooltip("Width of the dashed outline.")]
        [Range(0, 1)]
        public float outlineDashWidth = 0.5f;
        [Tooltip("Gap of the dashed outline.")]
        [Range(0, 1)]
        public float outlineDashGap = 0.3f;
        [Tooltip("Speed of the dashed outline.")]
        public float outlineDashSpeed = 2f;

        public Visibility outlineVisibility = Visibility.Normal;
        [Tooltip("If enabled, this object won't combine the outline with other objects.")]
        public bool outlineIndependent;
        [Tooltip("Select the mask mode used with this effect.")]
        public MaskMode outlineMaskMode = MaskMode.Stencil;

        [Range(0, 5)]
        [Tooltip("The intensity of the outer glow effect. A value of 0 disables the glow completely.")]
        public float glow;
        public float glowWidth = 0.4f;
        public QualityLevel glowQuality = QualityLevel.High;
        public BlurMethod glowBlurMethod = BlurMethod.Gaussian;
        public bool glowHighPrecision = true;
        [Tooltip("Controls how quickly the glow effect scales down with distance when constant width is disabled. Lower values make the effect fade faster with distance.")]
        public float glowDistanceScaleBias = 25f;
        [Range(1, 8)]
        [Tooltip("Reduces the quality of the glow but improves performance a bit.")]
        public int glowDownsampling = 2;
        [ColorUsage(true, true)] public Color glowHQColor = new Color(0.64f, 1f, 0f, 1f);
        [Tooltip("When enabled, outer glow renders with dithering. When disabled, glow appears as a solid color.")]
        [Range(0, 1)]
        public float glowDithering = 1;
        public GlowDitheringStyle glowDitheringStyle = GlowDitheringStyle.Pattern;
        [Tooltip("Seed for the dithering effect")]
        public float glowMagicNumber1 = 0.75f;
        [Tooltip("Another seed for the dithering effect that combines with first seed to create different patterns")]
        public float glowMagicNumber2 = 0.5f;
        public float glowAnimationSpeed = 1f;
        public Visibility glowVisibility = Visibility.Normal;
        public GlowBlendMode glowBlendMode = GlowBlendMode.Additive;
        [Tooltip("Blends glow passes one after another. If this option is disabled, glow passes won't overlap (in this case, make sure the glow pass 1 has a smaller offset than pass 2, etc.)")]
        public bool glowBlendPasses = true;
        public GlowPassData[] glowPasses;
        [Tooltip("Select the mask mode used with this effect.")]
        public MaskMode glowMaskMode = MaskMode.Stencil;

        [Tooltip("Makes the glow pixelated. A value of 0 disables pixelation. Higher values produce bigger pixels.")]
        [Range(0, 32)]
        public int glowPixelation;

        [Range(0, 5f)]
        [Tooltip("The intensity of the inner glow effect. A value of 0 disables the glow completely.")]
        public float innerGlow;
        [Range(0, 2)]
        public float innerGlowWidth = 1f;
        public float innerGlowPower = 1f;
        public InnerGlowBlendMode innerGlowBlendMode = InnerGlowBlendMode.Additive;
        [ColorUsage(true, true)] public Color innerGlowColor = Color.white;
        public Visibility innerGlowVisibility = Visibility.Normal;

        [Tooltip("Enables the targetFX effect. This effect draws an animated sprite over the object.")]
        public bool targetFX;
        [Tooltip("Style of the target FX effect.")]
        public TargetFXStyle targetFXStyle = TargetFXStyle.Texture;
        [Tooltip("Width of the frame when using Frame style.")]
        [Range(0.001f, 0.5f)]
        public float targetFXFrameWidth = 0.1f;
        [Tooltip("Length of the frame corners when using Frame style.")]
        [Range(0.1f, 1f)]
        public float targetFXCornerLength = 0.25f;
        [Tooltip("Minimum opacity of the frame when using Frame style.")]
        [Range(0, 1)]
        public float targetFXFrameMinOpacity;
        public Texture2D targetFXTexture;
        [ColorUsage(true, true)] public Color targetFXColor = Color.white;
        public float targetFXRotationSpeed = 50f;
        public float targetFXRotationAngle;
        public float targetFXInitialScale = 4f;
        public float targetFXEndScale = 1.5f;
        [Tooltip("Makes target scale relative to object renderer bounds.")]
        public bool targetFXScaleToRenderBounds;
        [Tooltip("Makes target FX effect square")]
        public bool targetFXSquare = true;
        [Tooltip("Places target FX sprite at the bottom of the highlighted object.")]
        public bool targetFXAlignToGround;
        [Tooltip("Max distance from the center of the highlighted object to the ground.")]
        public float targetFXGroundMaxDistance = 15f;
        public LayerMask targetFXGroundLayerMask = -1;
        [Tooltip("Fade out effect with altitude")]
        public float targetFXFadePower = 32;
        [Tooltip("Enable to render a single target FX effect at the center of the enclosing bounds")]
        public bool targetFXUseEnclosingBounds;
        [Tooltip("Optional world space offset for the position of the targetFX effect")]
        public Vector3 targetFXOffset;
        [Tooltip("If enabled, the target FX effect will be centered on the hit position")]
        public bool targetFxCenterOnHitPosition;
        [Tooltip("If enabled, the target FX effect will align to the hit normal")]
        public bool targetFxAlignToNormal;
        public float targetFXTransitionDuration = 0.5f;
        [Tooltip("0 = stay forever")]
        public float targetFXStayDuration = 1.5f;
        public Visibility targetFXVisibility = Visibility.AlwaysOnTop;
        [Tooltip("If the ground is transparent, the effect won't work. You can set this property to the altitude of the transparent ground to force the effect to render at this altitude.")]
        public float targetFXGroundMinAltitude = -1000;

        [Tooltip("Enables the iconFX effect. This effect draws an animated object over the object.")]
        public bool iconFX;
        public IconAssetType iconFXAssetType;
        public GameObject iconFXPrefab;
        public Mesh iconFXMesh;
        [ColorUsage(true, true)] public Color iconFXLightColor = Color.white;
        [ColorUsage(true, true)] public Color iconFXDarkColor = Color.gray;
        public float iconFXRotationSpeed = 50f;
        public IconAnimationOption iconFXAnimationOption = IconAnimationOption.None;
        public float iconFXAnimationAmount = 0.1f;
        public float iconFXAnimationSpeed = 3f;
        public float iconFXScale = 1f;
        [Tooltip("Makes target scale relative to object renderer bounds.")]
        public bool iconFXScaleToRenderBounds;
        [Tooltip("Optional world space offset for the position of the iconFX effect")]
        public Vector3 iconFXOffset = new Vector3(0, 1, 0);
        public float iconFXTransitionDuration = 0.5f;
        [Tooltip("0 = stay forever")]
        public float iconFXStayDuration = 1.5f;

        [Tooltip("Enables the label effect. This effect shows a text label over the object.")]
        public bool labelEnabled;
        [Tooltip("The text to display in the label")]
        public string labelText = "Label";
        [Tooltip("The size of the label text")]
        public float labelTextSize = 14;
        [ColorUsage(true, true)] public Color labelColor = Color.white;
        [Tooltip("The prefab to use for the label. Must contain a Canvas and TextMeshProUGUI component.")]
        public GameObject labelPrefab;
        public float labelVerticalOffset;
        [Tooltip("The horizontal offset of the label with respect to the object bounds")]
        public float lineLength = 200;

        [Tooltip("If enabled, the label will follow the cursor when hovering the object")]
        public bool labelFollowCursor = true;
        public LabelMode labelMode = LabelMode.WhenHighlighted;
        [Tooltip("If enabled, the label will be shown in editor mode (non playing)")]
        public bool labelShowInEditorMode = true;
        [Tooltip("Controls the alignment of the label relative to the target object on screen.")]
        public LabelAlignment labelAlignment = LabelAlignment.Auto;

        [Tooltip("See-through mode for this Highlight Effect component.")]
        public SeeThroughMode seeThrough = SeeThroughMode.Never;
        [Tooltip("This mask setting let you specify which objects will be considered as occluders and cause the see-through effect for this Highlight Effect component. For example, you assign your walls to a different layer and specify that layer here, so only walls and not other objects, like ground or ceiling, will trigger the see-through effect.")]
        public LayerMask seeThroughOccluderMask = -1;
        [Tooltip("Uses stencil buffers to ensure pixel-accurate occlusion test. If this option is disabled, only physics raycasting is used to test for occlusion.")]
        public bool seeThroughOccluderMaskAccurate;
        [Tooltip("A multiplier for the occluder volume size which can be used to reduce the actual size of occluders when Highlight Effect checks if they're occluding this object.")]
        [Range(0.01f, 0.9f)] public float seeThroughOccluderThreshold = 0.4f;
        [Tooltip("The interval of time between occlusion tests.")]
        public float seeThroughOccluderCheckInterval = 1f;
        [Tooltip("If enabled, occlusion test is performed for each children element. If disabled, the bounds of all children is combined and a single occlusion test is performed for the combined bounds.")]
        public bool seeThroughOccluderCheckIndividualObjects;
        [Tooltip("Shows the see-through effect only if the occluder if at this 'offset' distance from the object.")]
        public float seeThroughDepthOffset;
        [Tooltip("Hides the see-through effect if the occluder is further than this distance from the object (0 = infinite)")]
        public float seeThroughMaxDepth;
        [Range(0, 5f)] public float seeThroughIntensity = 0.8f;
        [Range(0, 1)] public float seeThroughTintAlpha = 0.5f;
        public Color seeThroughTintColor = Color.red;
        [Range(0, 1)] public float seeThroughNoise = 1f;
        [Range(0, 1)] public float seeThroughBorder;
        public Color seeThroughBorderColor = Color.black;
        public float seeThroughBorderWidth = 0.45f;
        [Tooltip("Only display the border instead of the full see-through effect.")]
        public bool seeThroughBorderOnly;
        [Tooltip("This option clears the stencil buffer after rendering the see-through effect which results in correct rendering order and supports other stencil-based effects that render afterwards.")]
        public bool seeThroughOrdered;
        [Tooltip("Optional see-through mask effect texture.")]
        public Texture2D seeThroughTexture;
        public TextureUVSpace seeThroughTextureUVSpace;
        public float seeThroughTextureScale = 1f;
        [Tooltip("The order by which children objects are rendered by the see-through effect")]
        public SeeThroughSortingMode seeThroughChildrenSortingMode = SeeThroughSortingMode.Default;

        [Range(0, 1)] public float hitFxInitialIntensity;
        public HitFxMode hitFxMode = HitFxMode.Overlay;
        public HitFXTriggerMode hitFXTriggerMode = HitFXTriggerMode.Scripting;
        public float hitFxFadeOutDuration = 0.25f;
        [ColorUsage(true, true)] public Color hitFxColor = Color.white;
        public float hitFxRadius = 0.5f;

        public void Load (HighlightEffect effect) {
            effect.effectGroup = effectGroup;
            effect.effectGroupLayer = effectGroupLayer;
            effect.effectNameFilter = effectNameFilter;
            effect.effectNameUseRegEx = effectNameUseRegEx;
            effect.combineMeshes = combineMeshes;
            effect.alphaCutOff = alphaCutOff;
            effect.cullBackFaces = cullBackFaces;
            effect.padding = padding;
            effect.normalsOption = normalsOption;
            effect.fadeInDuration = fadeInDuration;
            effect.fadeOutDuration = fadeOutDuration;
            effect.cameraDistanceFade = cameraDistanceFade;
            effect.cameraDistanceFadeFar = cameraDistanceFadeFar;
            effect.cameraDistanceFadeNear = cameraDistanceFadeNear;
            effect.constantWidth = constantWidth;
            effect.extraCoveragePixels = extraCoveragePixels;
            effect.minimumWidth = minimumWidth;

            effect.overlay = overlay;
            effect.overlayMode = overlayMode;
            effect.overlayColor = overlayColor;
            effect.overlayAnimationSpeed = overlayAnimationSpeed;
            effect.overlayMinIntensity = overlayMinIntensity;
            effect.overlayBlending = overlayBlending;
            effect.overlayTexture = overlayTexture;
            effect.overlayTextureUVSpace = overlayTextureUVSpace;
            effect.overlayTextureScale = overlayTextureScale;
            effect.overlayTextureScrolling = overlayTextureScrolling;
            effect.overlayVisibility = overlayVisibility;
            effect.overlayPattern = overlayPattern;
            effect.overlayPatternScrolling = overlayPatternScrolling;
            effect.overlayPatternScale = overlayPatternScale;
            effect.overlayPatternSize = overlayPatternSize;
            effect.overlayPatternSoftness = overlayPatternSoftness;
            effect.overlayPatternRotation = overlayPatternRotation;

            effect.outline = outline;
            effect.outlineColor = outlineColor;
            effect.outlineColorStyle = outlineColorStyle;
            effect.outlineGradient = outlineGradient;
            effect.outlineGradientInLocalSpace = outlineGradientInLocalSpace;
            effect.outlineWidth = outlineWidth;
            effect.outlineBlurPasses = outlineBlurPasses;
            effect.outlineQuality = outlineQuality;
            effect.outlineEdgeMode = outlineEdgeMode;
            effect.outlineEdgeThreshold = outlineEdgeThreshold;
            effect.outlineDistanceScaleBias = outlineDistanceScaleBias;
            effect.outlineSharpness = outlineSharpness;
            effect.outlineDownsampling = outlineDownsampling;
            effect.outlineVisibility = outlineVisibility;
            effect.outlineIndependent = outlineIndependent;
            effect.outlineContourStyle = outlineContourStyle;
            effect.outlineMaskMode = outlineMaskMode;
            effect.outlineStylized = outlineStylized;
            effect.outlinePattern = outlinePattern;
            effect.outlinePatternScale = outlinePatternScale;
            effect.outlinePatternThreshold = outlinePatternThreshold;
            effect.outlinePatternDistortionTexture = outlinePatternDistortionTexture;
            effect.outlinePatternDistortionAmount = outlinePatternDistortionAmount;
            effect.outlinePatternStopMotionScale = outlinePatternStopMotionScale;
            effect.outlineDashed = outlineDashed;
            effect.outlineDashWidth = outlineDashWidth;
            effect.outlineDashGap = outlineDashGap;
            effect.outlineDashSpeed = outlineDashSpeed;
            effect.outlineGradientKnee = outlineGradientKnee;
            effect.outlineGradientPower = outlineGradientPower;
            effect.outlinePixelation = outlinePixelation;

            effect.glow = glow;
            effect.glowWidth = glowWidth;
            effect.glowQuality = glowQuality;
            effect.glowBlurMethod = glowBlurMethod;
            effect.glowHighPrecision = glowHighPrecision;
            effect.glowDistanceScaleBias = glowDistanceScaleBias;
            effect.glowDownsampling = glowDownsampling;
            effect.glowHQColor = glowHQColor;
            effect.glowDithering = glowDithering;
            effect.glowDitheringStyle = glowDitheringStyle;
            effect.glowMagicNumber1 = glowMagicNumber1;
            effect.glowMagicNumber2 = glowMagicNumber2;
            effect.glowAnimationSpeed = glowAnimationSpeed;
            effect.glowVisibility = glowVisibility;
            effect.glowBlendMode = glowBlendMode;
            effect.glowBlendPasses = glowBlendPasses;
            effect.glowPasses = GetGlowPassesCopy(glowPasses);
            effect.glowMaskMode = glowMaskMode;
            effect.glowPixelation = glowPixelation;

            effect.innerGlow = innerGlow;
            effect.innerGlowWidth = innerGlowWidth;
            effect.innerGlowPower = innerGlowPower;
            effect.innerGlowColor = innerGlowColor;
            effect.innerGlowBlendMode = innerGlowBlendMode;
            effect.innerGlowVisibility = innerGlowVisibility;

            effect.targetFX = targetFX;
            effect.targetFXColor = targetFXColor;
            effect.targetFXInitialScale = targetFXInitialScale;
            effect.targetFXEndScale = targetFXEndScale;
            effect.targetFXScaleToRenderBounds = targetFXScaleToRenderBounds;
            effect.targetFXAlignToGround = targetFXAlignToGround;
            effect.targetFXGroundMaxDistance = targetFXGroundMaxDistance;
            effect.targetFXGroundLayerMask = targetFXGroundLayerMask;
            effect.targetFXFadePower = targetFXFadePower;
            effect.targetFXRotationSpeed = targetFXRotationSpeed;
            effect.targetFXRotationAngle = targetFXRotationAngle;
            effect.targetFXStayDuration = targetFXStayDuration;
            effect.targetFXTexture = targetFXTexture;
            effect.targetFXTransitionDuration = targetFXTransitionDuration;
            effect.targetFXVisibility = targetFXVisibility;
            effect.targetFXUseEnclosingBounds = targetFXUseEnclosingBounds;
            effect.targetFXSquare = targetFXSquare;
            effect.targetFXOffset = targetFXOffset;
            effect.targetFxCenterOnHitPosition = targetFxCenterOnHitPosition;
            effect.targetFxAlignToNormal = targetFxAlignToNormal;
            effect.targetFXStyle = targetFXStyle;
            effect.targetFXFrameWidth = targetFXFrameWidth;
            effect.targetFXCornerLength = targetFXCornerLength;
            effect.targetFXFrameMinOpacity = targetFXFrameMinOpacity;
            effect.targetFXGroundMinAltitude = targetFXGroundMinAltitude;

            effect.iconFX = iconFX;
            effect.iconFXAssetType = iconFXAssetType;
            effect.iconFXPrefab = iconFXPrefab;
            effect.iconFXMesh = iconFXMesh;
            effect.iconFXLightColor = iconFXLightColor;
            effect.iconFXDarkColor = iconFXDarkColor;
            effect.iconFXAnimationOption = iconFXAnimationOption;
            effect.iconFXAnimationAmount = iconFXAnimationAmount;
            effect.iconFXAnimationSpeed = iconFXAnimationSpeed;
            effect.iconFXScale = iconFXScale;
            effect.iconFXScaleToRenderBounds = iconFXScaleToRenderBounds;
            effect.iconFXOffset = iconFXOffset;
            effect.iconFXRotationSpeed = iconFXRotationSpeed;
            effect.iconFXStayDuration = iconFXStayDuration;
            effect.iconFXTransitionDuration = iconFXTransitionDuration;

            effect.seeThrough = seeThrough;
            effect.seeThroughOccluderMask = seeThroughOccluderMask;
            effect.seeThroughOccluderMaskAccurate = seeThroughOccluderMaskAccurate;
            effect.seeThroughOccluderThreshold = seeThroughOccluderThreshold;
            effect.seeThroughOccluderCheckInterval = seeThroughOccluderCheckInterval;
            effect.seeThroughOccluderCheckIndividualObjects = seeThroughOccluderCheckIndividualObjects;
            effect.seeThroughIntensity = seeThroughIntensity;
            effect.seeThroughTintAlpha = seeThroughTintAlpha;
            effect.seeThroughTintColor = seeThroughTintColor;
            effect.seeThroughNoise = seeThroughNoise;
            effect.seeThroughBorder = seeThroughBorder;
            effect.seeThroughBorderColor = seeThroughBorderColor;
            effect.seeThroughBorderWidth = seeThroughBorderWidth;
            effect.seeThroughBorderOnly = seeThroughBorderOnly;
            effect.seeThroughDepthOffset = seeThroughDepthOffset;
            effect.seeThroughMaxDepth = seeThroughMaxDepth;
            effect.seeThroughOrdered = seeThroughOrdered;
            effect.seeThroughTexture = seeThroughTexture;
            effect.seeThroughTextureScale = seeThroughTextureScale;
            effect.seeThroughTextureUVSpace = seeThroughTextureUVSpace;
            effect.seeThroughChildrenSortingMode = seeThroughChildrenSortingMode;

            effect.hitFxInitialIntensity = hitFxInitialIntensity;
            effect.hitFxMode = hitFxMode;
            effect.hitFXTriggerMode = hitFXTriggerMode;
            effect.hitFxFadeOutDuration = hitFxFadeOutDuration;
            effect.hitFxColor = hitFxColor;
            effect.hitFxRadius = hitFxRadius;

            effect.labelEnabled = labelEnabled;
            effect.labelText = labelText;
            effect.labelTextSize = labelTextSize;
            effect.labelColor = labelColor;
            effect.labelPrefab = labelPrefab;
            effect.labelVerticalOffset = labelVerticalOffset;
            effect.lineLength = lineLength;
            effect.labelFollowCursor = labelFollowCursor;
            effect.labelMode = labelMode;
            effect.labelAlignment = labelAlignment;
            effect.labelShowInEditorMode = labelShowInEditorMode;

            effect.UpdateMaterialProperties();
        }

        public void Save (HighlightEffect effect) {
            effectGroup = effect.effectGroup;
            effectGroupLayer = effect.effectGroupLayer;
            effectNameFilter = effect.effectNameFilter;
            effectNameUseRegEx = effect.effectNameUseRegEx;
            combineMeshes = effect.combineMeshes;
            alphaCutOff = effect.alphaCutOff;
            cullBackFaces = effect.cullBackFaces;
            padding = effect.padding;
            normalsOption = effect.normalsOption;
            fadeInDuration = effect.fadeInDuration;
            fadeOutDuration = effect.fadeOutDuration;
            cameraDistanceFade = effect.cameraDistanceFade;
            cameraDistanceFadeFar = effect.cameraDistanceFadeFar;
            cameraDistanceFadeNear = effect.cameraDistanceFadeNear;
            constantWidth = effect.constantWidth;
            extraCoveragePixels = effect.extraCoveragePixels;
            minimumWidth = effect.minimumWidth;

            overlay = effect.overlay;
            overlayMode = effect.overlayMode;
            overlayColor = effect.overlayColor;
            overlayAnimationSpeed = effect.overlayAnimationSpeed;
            overlayMinIntensity = effect.overlayMinIntensity;
            overlayBlending = effect.overlayBlending;
            overlayTexture = effect.overlayTexture;
            overlayTextureUVSpace = effect.overlayTextureUVSpace;
            overlayTextureScale = effect.overlayTextureScale;
            overlayTextureScrolling = effect.overlayTextureScrolling;
            overlayVisibility = effect.overlayVisibility;
            overlayPattern = effect.overlayPattern;
            overlayPatternScrolling = effect.overlayPatternScrolling;
            overlayPatternScale = effect.overlayPatternScale;
            overlayPatternSize = effect.overlayPatternSize;
            overlayPatternSoftness = effect.overlayPatternSoftness;
            overlayPatternRotation = effect.overlayPatternRotation;

            outline = effect.outline;
            outlineColor = effect.outlineColor;
            outlineColorStyle = effect.outlineColorStyle;
            outlineGradient = effect.outlineGradient;
            outlineGradientInLocalSpace = effect.outlineGradientInLocalSpace;
            outlineWidth = effect.outlineWidth;
            outlineBlurPasses = effect.outlineBlurPasses;
            outlineQuality = effect.outlineQuality;
            outlineEdgeMode = effect.outlineEdgeMode;
            outlineEdgeThreshold = effect.outlineEdgeThreshold;
            outlineDistanceScaleBias = effect.outlineDistanceScaleBias;
            outlineSharpness = effect.outlineSharpness;
            outlineDownsampling = effect.outlineDownsampling;
            outlineVisibility = effect.outlineVisibility;
            outlineIndependent = effect.outlineIndependent;
            outlineContourStyle = effect.outlineContourStyle;
            outlineMaskMode = effect.outlineMaskMode;
            outlineStylized = effect.outlineStylized;
            outlinePattern = effect.outlinePattern;
            outlinePatternScale = effect.outlinePatternScale;
            outlinePatternThreshold = effect.outlinePatternThreshold;
            outlinePatternDistortionTexture = effect.outlinePatternDistortionTexture;
            outlinePatternDistortionAmount = effect.outlinePatternDistortionAmount;
            outlinePatternStopMotionScale = effect.outlinePatternStopMotionScale;
            outlineDashed = effect.outlineDashed;
            outlineDashWidth = effect.outlineDashWidth;
            outlineDashGap = effect.outlineDashGap;
            outlineDashSpeed = effect.outlineDashSpeed;
            outlineGradientKnee = effect.outlineGradientKnee;
            outlineGradientPower = effect.outlineGradientPower;
            outlinePixelation = effect.outlinePixelation;

            glow = effect.glow;
            glowWidth = effect.glowWidth;
            glowQuality = effect.glowQuality;
            glowHighPrecision = effect.glowHighPrecision;
            glowBlurMethod = effect.glowBlurMethod;
            glowDistanceScaleBias = effect.glowDistanceScaleBias;
            glowDownsampling = effect.glowDownsampling;
            glowHQColor = effect.glowHQColor;
            glowDithering = effect.glowDithering;
            glowDitheringStyle = effect.glowDitheringStyle;
            glowMagicNumber1 = effect.glowMagicNumber1;
            glowMagicNumber2 = effect.glowMagicNumber2;
            glowAnimationSpeed = effect.glowAnimationSpeed;
            glowVisibility = effect.glowVisibility;
            glowBlendMode = effect.glowBlendMode;
            glowBlendPasses = effect.glowBlendPasses;
            glowPasses = GetGlowPassesCopy(effect.glowPasses);
            glowMaskMode = effect.glowMaskMode;
            glowPixelation = effect.glowPixelation;

            innerGlow = effect.innerGlow;
            innerGlowWidth = effect.innerGlowWidth;
            innerGlowPower = effect.innerGlowPower;
            innerGlowColor = effect.innerGlowColor;
            innerGlowBlendMode = effect.innerGlowBlendMode;
            innerGlowVisibility = effect.innerGlowVisibility;

            targetFX = effect.targetFX;
            targetFXColor = effect.targetFXColor;
            targetFXInitialScale = effect.targetFXInitialScale;
            targetFXEndScale = effect.targetFXEndScale;
            targetFXScaleToRenderBounds = effect.targetFXScaleToRenderBounds;
            targetFXAlignToGround = effect.targetFXAlignToGround;
            targetFXGroundMaxDistance = effect.targetFXGroundMaxDistance;
            targetFXGroundLayerMask = effect.targetFXGroundLayerMask;
            targetFXFadePower = effect.targetFXFadePower;
            targetFXRotationSpeed = effect.targetFXRotationSpeed;
            targetFXRotationAngle = effect.targetFXRotationAngle;
            targetFXStayDuration = effect.targetFXStayDuration;
            targetFXTexture = effect.targetFXTexture;
            targetFXTransitionDuration = effect.targetFXTransitionDuration;
            targetFXVisibility = effect.targetFXVisibility;
            targetFXUseEnclosingBounds = effect.targetFXUseEnclosingBounds;
            targetFXSquare = effect.targetFXSquare;
            targetFXOffset = effect.targetFXOffset;
            targetFxCenterOnHitPosition = effect.targetFxCenterOnHitPosition;
            targetFxAlignToNormal = effect.targetFxAlignToNormal;
            targetFXStyle = effect.targetFXStyle;
            targetFXFrameWidth = effect.targetFXFrameWidth;
            targetFXCornerLength = effect.targetFXCornerLength;
            targetFXFrameMinOpacity = effect.targetFXFrameMinOpacity;
            targetFXGroundMinAltitude = effect.targetFXGroundMinAltitude;

            iconFX = effect.iconFX;
            iconFXAssetType = effect.iconFXAssetType;
            iconFXPrefab = effect.iconFXPrefab;
            iconFXMesh = effect.iconFXMesh;
            iconFXLightColor = effect.iconFXLightColor;
            iconFXDarkColor = effect.iconFXDarkColor;
            iconFXAnimationOption = effect.iconFXAnimationOption;
            iconFXAnimationAmount = effect.iconFXAnimationAmount;
            iconFXAnimationSpeed = effect.iconFXAnimationSpeed;
            iconFXScaleToRenderBounds = effect.iconFXScaleToRenderBounds;
            iconFXScale = effect.iconFXScale;
            iconFXOffset = effect.iconFXOffset;
            iconFXRotationSpeed = effect.iconFXRotationSpeed;
            iconFXStayDuration = effect.iconFXStayDuration;
            iconFXTransitionDuration = effect.iconFXTransitionDuration;

            seeThrough = effect.seeThrough;
            seeThroughOccluderMask = effect.seeThroughOccluderMask;
            seeThroughOccluderMaskAccurate = effect.seeThroughOccluderMaskAccurate;
            seeThroughOccluderThreshold = effect.seeThroughOccluderThreshold;
            seeThroughOccluderCheckInterval = effect.seeThroughOccluderCheckInterval;
            seeThroughOccluderCheckIndividualObjects = effect.seeThroughOccluderCheckIndividualObjects;
            seeThroughIntensity = effect.seeThroughIntensity;
            seeThroughTintAlpha = effect.seeThroughTintAlpha;
            seeThroughTintColor = effect.seeThroughTintColor;
            seeThroughNoise = effect.seeThroughNoise;
            seeThroughBorder = effect.seeThroughBorder;
            seeThroughBorderColor = effect.seeThroughBorderColor;
            seeThroughBorderWidth = effect.seeThroughBorderWidth;
            seeThroughDepthOffset = effect.seeThroughDepthOffset;
            seeThroughBorderOnly = effect.seeThroughBorderOnly;
            seeThroughMaxDepth = effect.seeThroughMaxDepth;
            seeThroughOrdered = effect.seeThroughOrdered;
            seeThroughTexture = effect.seeThroughTexture;
            seeThroughTextureScale = effect.seeThroughTextureScale;
            seeThroughTextureUVSpace = effect.seeThroughTextureUVSpace;
            seeThroughChildrenSortingMode = effect.seeThroughChildrenSortingMode;

            hitFxInitialIntensity = effect.hitFxInitialIntensity;
            hitFxMode = effect.hitFxMode;
            hitFXTriggerMode = effect.hitFXTriggerMode;
            hitFxFadeOutDuration = effect.hitFxFadeOutDuration;
            hitFxColor = effect.hitFxColor;
            hitFxRadius = effect.hitFxRadius;

            labelEnabled = effect.labelEnabled;
            labelText = effect.labelText;
            labelTextSize = effect.labelTextSize;
            labelColor = effect.labelColor;
            labelPrefab = effect.labelPrefab;
            labelVerticalOffset = effect.labelVerticalOffset;
            lineLength = effect.lineLength;
            labelFollowCursor = effect.labelFollowCursor;
            labelMode = effect.labelMode;
            labelAlignment = effect.labelAlignment;
            labelShowInEditorMode = effect.labelShowInEditorMode;
        }

        GlowPassData[] GetGlowPassesCopy (GlowPassData[] glowPasses) {
            if (glowPasses == null) {
                return new GlowPassData[0];
            }
            GlowPassData[] pd = new GlowPassData[glowPasses.Length];
            for (int k = 0; k < glowPasses.Length; k++) {
                pd[k].alpha = glowPasses[k].alpha;
                pd[k].color = glowPasses[k].color;
                pd[k].offset = glowPasses[k].offset;
            }
            return pd;
        }

        public void OnValidate () {
            outlineGradientKnee = Mathf.Max(0f, outlineGradientKnee);
            outlineGradientPower = Mathf.Max(0f, outlineGradientPower);
            outlineEdgeThreshold = Mathf.Clamp01(outlineEdgeThreshold);
            outlineSharpness = Mathf.Max(outlineSharpness, 1f);
            outlineDistanceScaleBias = Mathf.Max(1, outlineDistanceScaleBias);
            extraCoveragePixels = Mathf.Max(0, extraCoveragePixels);
            glowWidth = Mathf.Max(0, glowWidth);
            glowAnimationSpeed = Mathf.Max(0, glowAnimationSpeed);
            glowDistanceScaleBias = Mathf.Max(1, glowDistanceScaleBias);
            outlineDistanceScaleBias = Mathf.Max(1, outlineDistanceScaleBias);
            overlayAnimationSpeed = Mathf.Max(0, overlayAnimationSpeed);
            innerGlowPower = Mathf.Max(1f, innerGlowPower);
            seeThroughDepthOffset = Mathf.Max(0, seeThroughDepthOffset);
            seeThroughMaxDepth = Mathf.Max(0, seeThroughMaxDepth);
            seeThroughBorderWidth = Mathf.Max(0, seeThroughBorderWidth);
            targetFXFadePower = Mathf.Max(0, targetFXFadePower);
            cameraDistanceFadeNear = Mathf.Max(0, cameraDistanceFadeNear);
            cameraDistanceFadeFar = Mathf.Max(0, cameraDistanceFadeFar);
            iconFXScale = Mathf.Max(0, iconFXScale);
            iconFXAnimationAmount = Mathf.Max(0, iconFXAnimationAmount);
            iconFXAnimationSpeed = Mathf.Max(0, iconFXAnimationSpeed);
            outlinePatternScale = Mathf.Max(0, outlinePatternScale);
            outlinePatternDistortionAmount = Mathf.Max(0, outlinePatternDistortionAmount);
            outlinePatternThreshold = Mathf.Max(0, outlinePatternThreshold);
            outlinePatternStopMotionScale = Mathf.Max(1, outlinePatternStopMotionScale);
            if (glowPasses == null || glowPasses.Length == 0) {
                glowPasses = new GlowPassData[4];
                glowPasses[0] = new GlowPassData() { offset = 4, alpha = 0.1f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[1] = new GlowPassData() { offset = 3, alpha = 0.2f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[2] = new GlowPassData() { offset = 2, alpha = 0.3f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[3] = new GlowPassData() { offset = 1, alpha = 0.4f, color = new Color(0.64f, 1f, 0f, 1f) };
            }
            if (labelPrefab == null) {
                labelPrefab = Resources.Load<GameObject>("HighlightPlus/Label");
            }
            lineLength = Mathf.Max(0, lineLength);
        }
    }
}

