using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public class UI_ImageSelectors_Sliced
{
    [Header("기본 설정")]
    [Tooltip("이 이미지 설정의 이름을 입력하세요")]
    public string Name = "New Image Setting";
    
    [Header("소스 이미지")]
    [Tooltip("적용할 소스 스프라이트를 선택하세요")]
    public Sprite SourceSprite;
    
    [Header("대상 이미지들")]
    [Tooltip("소스 스프라이트를 적용할 대상 Image 컴포넌트들을 선택하세요")]
    public Image[] TargetImages;
    
    [Header("이미지 타입 설정")]
    [Tooltip("이미지를 Sliced 타입으로 사용할지 설정하세요")]
    public bool IsSliced = false;

    [Header("이미지 비율 유지 설정")]
    [Tooltip("이미지를 비율 유지 여부를 설정하세요")]
    public bool IsPreserveAspect = false;
    
    [Tooltip("Sliced 타입 사용 시 픽셀 단위 배율을 설정하세요")]
    public float pixelPerUnit = 1f;

    /// <summary>
    /// 이 설정의 유효성을 검사합니다
    /// </summary>
    public bool IsValid()
    {
        return SourceSprite != null && TargetImages != null && TargetImages.Length > 0;
    }

    /// <summary>
    /// 이 설정의 정보를 문자열로 반환합니다
    /// </summary>
    public string GetInfo()
    {
        return $"'{Name}' - Source: {(SourceSprite != null ? SourceSprite.name : "null")}, Targets: {(TargetImages != null ? TargetImages.Length : 0)}개, Sliced: {IsSliced}";
    }
}

[Serializable]
public class ImageArrayGroup
{
    [Header("이미지 배열 그룹")]
    [Tooltip("이 배열 그룹의 이름을 입력하세요")]
    public string GroupName = "New Image Group";
    
    [Tooltip("이 그룹에 포함될 이미지 설정들을 추가하세요")]
    public UI_ImageSelectors_Sliced[] Images;
    
    /// <summary>
    /// 이 그룹의 유효한 이미지 개수를 반환합니다
    /// </summary>
    public int GetValidImageCount()
    {
        if (Images == null) return 0;
        
        int count = 0;
        foreach (var image in Images)
        {
            if (image.IsValid()) count++;
        }
        return count;
    }
    
    /// <summary>
    /// 이 그룹의 정보를 문자열로 반환합니다
    /// </summary>
    public string GetGroupInfo()
    {
        return $"'{GroupName}' - 총 {Images?.Length ?? 0}개, 유효 {GetValidImageCount()}개";
    }
}

public class EImageSelector : MonoBehaviour
{
    [Header("이미지 선택기 설정")]
    [Tooltip("이미지 배열 그룹들을 추가/삭제하여 관리하세요")]
    [SerializeField] private List<ImageArrayGroup> imageGroups = new List<ImageArrayGroup>();
    
    [Header("편의 기능")]
    [SerializeField] private bool autoApplyOnStart = false;
    [SerializeField] private bool showDebugInfo = false;

    private void Start()
    {
        if (autoApplyOnStart)
        {
            SetAllImagesToSource();
        }
    }

    /// <summary>
    /// 모든 이미지를 소스로 설정
    /// </summary>
    [ContextMenu("Apply All Images to Source")]
    public void SetAllImagesToSource()
    {
        if (imageGroups == null || imageGroups.Count == 0)
        {
            Debug.LogWarning("EImageSelector: 이미지 그룹이 없습니다.");
            return;
        }

        int totalAppliedCount = 0;

        foreach (var group in imageGroups)
        {
            if (group.Images == null || group.Images.Length == 0) continue;

            foreach (var image in group.Images)
            {
                if (!image.IsValid())
                {
                    if (showDebugInfo)
                        Debug.LogWarning($"EImageSelector: {image.GetInfo()} - 설정이 불완전합니다.");
                    continue;
                }

                foreach (var targetImage in image.TargetImages)
                {
                    if (targetImage != null)
                    {
                        targetImage.sprite = image.SourceSprite;
                        targetImage.type = image.IsSliced ? Image.Type.Sliced : Image.Type.Simple;
                        if (image.IsSliced)
                        {
                            targetImage.pixelsPerUnitMultiplier = image.pixelPerUnit;
                        }
                        if (image.IsPreserveAspect)
                        {
                            targetImage.preserveAspect = true;
                        }
                        totalAppliedCount++;
                    }
                }
            }
        }

        if (showDebugInfo)
            Debug.Log($"EImageSelector: {totalAppliedCount}개의 이미지가 적용되었습니다.");
    }

    /// <summary>
    /// 특정 그룹의 특정 인덱스 이미지 설정
    /// </summary>
    public void SetImageAtGroupIndex(int groupIndex, int imageIndex)
    {
        if (imageGroups == null || groupIndex < 0 || groupIndex >= imageGroups.Count)
        {
            Debug.LogError($"EImageSelector: 유효하지 않은 그룹 인덱스 {groupIndex}");
            return;
        }

        var targetGroup = imageGroups[groupIndex];
        if (targetGroup.Images == null || imageIndex < 0 || imageIndex >= targetGroup.Images.Length)
        {
            Debug.LogError($"EImageSelector: 유효하지 않은 이미지 인덱스 {imageIndex}");
            return;
        }

        var image = targetGroup.Images[imageIndex];
        if (!image.IsValid())
        {
            Debug.LogWarning($"EImageSelector: 그룹 {groupIndex}, 인덱스 {imageIndex}의 이미지 설정이 완료되지 않았습니다.");
            return;
        }

        foreach (var targetImage in image.TargetImages)
        {
            if (targetImage != null)
            {
                targetImage.sprite = image.SourceSprite;
                targetImage.type = image.IsSliced ? Image.Type.Sliced : Image.Type.Simple;
                if (image.IsSliced)
                {
                    targetImage.pixelsPerUnitMultiplier = image.pixelPerUnit;
                }
            }
        }
    }

    /// <summary>
    /// 이름으로 이미지 설정 (모든 그룹에서 검색)
    /// </summary>
    public void SetImageByName(string name)
    {
        if (imageGroups == null) return;
        
        for (int groupIndex = 0; groupIndex < imageGroups.Count; groupIndex++)
        {
            var group = imageGroups[groupIndex];
            if (group.Images == null) continue;

            for (int imageIndex = 0; imageIndex < group.Images.Length; imageIndex++)
            {
                if (group.Images[imageIndex].Name == name)
                {
                    SetImageAtGroupIndex(groupIndex, imageIndex);
                    return;
                }
            }
        }

        Debug.LogWarning($"EImageSelector: '{name}' 이름의 이미지를 찾을 수 없습니다.");
    }

    /// <summary>
    /// 그룹 이름으로 이미지 설정
    /// </summary>
    public void SetAllImagesInGroup(string groupName)
    {
        if (imageGroups == null) return;
        
        for (int groupIndex = 0; groupIndex < imageGroups.Count; groupIndex++)
        {
            var group = imageGroups[groupIndex];
            if (group.GroupName == groupName)
            {
                SetAllImagesInGroup(groupIndex);
                return;
            }
        }

        Debug.LogWarning($"EImageSelector: '{groupName}' 이름의 그룹을 찾을 수 없습니다.");
    }

    /// <summary>
    /// 특정 그룹의 모든 이미지 설정
    /// </summary>
    public void SetAllImagesInGroup(int groupIndex)
    {
        if (imageGroups == null || groupIndex < 0 || groupIndex >= imageGroups.Count)
        {
            Debug.LogError($"EImageSelector: 유효하지 않은 그룹 인덱스 {groupIndex}");
            return;
        }

        var group = imageGroups[groupIndex];
        if (group.Images == null || group.Images.Length == 0)
        {
            Debug.LogWarning($"EImageSelector: 그룹 {groupIndex}에 이미지가 없습니다.");
            return;
        }

        int appliedCount = 0;
        foreach (var image in group.Images)
        {
            if (!image.IsValid()) continue;

            foreach (var targetImage in image.TargetImages)
            {
                if (targetImage != null)
                {
                    targetImage.sprite = image.SourceSprite;
                    targetImage.type = image.IsSliced ? Image.Type.Sliced : Image.Type.Simple;
                    if (image.IsSliced)
                    {
                        targetImage.pixelsPerUnitMultiplier = image.pixelPerUnit;
                    }
                    appliedCount++;
                }
            }
        }

        if (showDebugInfo)
            Debug.Log($"EImageSelector: 그룹 '{group.GroupName}'에서 {appliedCount}개의 이미지가 적용되었습니다.");
    }

    /// <summary>
    /// 현재 설정된 모든 이미지 정보 출력
    /// </summary>
    [ContextMenu("Print Current Image Info")]
    public void PrintCurrentImageInfo()
    {
        if (imageGroups == null || imageGroups.Count == 0)
        {
            Debug.Log("EImageSelector: 설정된 이미지 그룹이 없습니다.");
            return;
        }

        Debug.Log($"=== EImageSelector 정보 ===");
        
        for (int groupIndex = 0; groupIndex < imageGroups.Count; groupIndex++)
        {
            var group = imageGroups[groupIndex];
            Debug.Log($"--- {group.GetGroupInfo()} ---");
            
            if (group.Images != null)
            {
                for (int imageIndex = 0; imageIndex < group.Images.Length; imageIndex++)
                {
                    var image = group.Images[imageIndex];
                    Debug.Log($"[{imageIndex}]: {image.GetInfo()}");
                }
            }
        }
    }

    /// <summary>
    /// 모든 이미지 설정 유효성 검사
    /// </summary>
    [ContextMenu("Validate Image Settings")]
    public void ValidateImageSettings()
    {
        if (imageGroups == null || imageGroups.Count == 0)
        {
            Debug.LogWarning("EImageSelector: 이미지 그룹이 없습니다.");
            return;
        }

        int totalValidCount = 0;
        int totalCount = 0;

        for (int groupIndex = 0; groupIndex < imageGroups.Count; groupIndex++)
        {
            var group = imageGroups[groupIndex];
            if (group.Images == null || group.Images.Length == 0) continue;

            int groupValidCount = 0;
            for (int imageIndex = 0; imageIndex < group.Images.Length; imageIndex++)
            {
                var image = group.Images[imageIndex];
                totalCount++;
                
                bool isValid = image.IsValid();
                if (isValid)
                {
                    groupValidCount++;
                    totalValidCount++;
                }
                else
                {
                    Debug.LogWarning($"EImageSelector: {group.GroupName}[{imageIndex}] {image.GetInfo()} - 설정이 불완전합니다.");
                }
            }

            if (group.Images.Length > 0)
            {
                Debug.Log($"EImageSelector: {group.GroupName} - {groupValidCount}/{group.Images.Length}개 유효");
            }
        }

        Debug.Log($"EImageSelector: 전체 {totalValidCount}/{totalCount}개의 이미지가 유효합니다.");
    }

    /// <summary>
    /// 모든 이미지 설정의 이름을 자동으로 생성
    /// </summary>
    [ContextMenu("Auto Generate Names")]
    public void AutoGenerateNames()
    {
        if (imageGroups == null) return;
        
        for (int groupIndex = 0; groupIndex < imageGroups.Count; groupIndex++)
        {
            var group = imageGroups[groupIndex];
            
            // 그룹 이름 자동 생성
            if (string.IsNullOrEmpty(group.GroupName) || group.GroupName == "New Image Group")
            {
                group.GroupName = $"Image Group {groupIndex + 1}";
            }
            
            if (group.Images != null)
            {
                for (int imageIndex = 0; imageIndex < group.Images.Length; imageIndex++)
                {
                    if (string.IsNullOrEmpty(group.Images[imageIndex].Name) || group.Images[imageIndex].Name == "New Image Setting")
                    {
                        group.Images[imageIndex].Name = $"{group.GroupName} Setting {imageIndex + 1}";
                    }
                }
            }
        }

        Debug.Log("EImageSelector: 모든 이미지 설정 이름이 자동 생성되었습니다.");
    }

    /// <summary>
    /// 새로운 이미지 그룹 추가
    /// </summary>
    [ContextMenu("Add New Image Group")]
    public void AddNewImageGroup()
    {
        if (imageGroups == null)
            imageGroups = new List<ImageArrayGroup>();

        var newGroup = new ImageArrayGroup
        {
            GroupName = $"Image Group {imageGroups.Count + 1}",
            Images = new UI_ImageSelectors_Sliced[0]
        };

        imageGroups.Add(newGroup);
        Debug.Log($"EImageSelector: 새로운 이미지 그룹 '{newGroup.GroupName}'이 추가되었습니다.");
    }

    /// <summary>
    /// 특정 그룹 삭제
    /// </summary>
    public void RemoveImageGroup(int groupIndex)
    {
        if (imageGroups == null || groupIndex < 0 || groupIndex >= imageGroups.Count)
        {
            Debug.LogError($"EImageSelector: 유효하지 않은 그룹 인덱스 {groupIndex}");
            return;
        }

        string groupName = imageGroups[groupIndex].GroupName;
        imageGroups.RemoveAt(groupIndex);
        Debug.Log($"EImageSelector: 이미지 그룹 '{groupName}'이 삭제되었습니다.");
    }

    /// <summary>
    /// 특정 그룹의 특정 인덱스 이름 변경
    /// </summary>
    public void RenameImageAtGroupIndex(int groupIndex, int imageIndex, string newName)
    {
        if (imageGroups == null || groupIndex < 0 || groupIndex >= imageGroups.Count)
        {
            Debug.LogError($"EImageSelector: 유효하지 않은 그룹 인덱스 {groupIndex}");
            return;
        }

        var targetGroup = imageGroups[groupIndex];
        if (targetGroup.Images == null || imageIndex < 0 || imageIndex >= targetGroup.Images.Length)
        {
            Debug.LogError($"EImageSelector: 유효하지 않은 이미지 인덱스 {imageIndex}");
            return;
        }

        targetGroup.Images[imageIndex].Name = newName;
        Debug.Log($"EImageSelector: {targetGroup.GroupName}[{imageIndex}]의 이름이 '{newName}'으로 변경되었습니다.");
    }
} 