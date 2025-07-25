﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HighlightPlus {

    public enum TriggerMode {
        ColliderEventsOnlyOnThisObject = 0,
        RaycastOnThisObjectAndChildren = 1,
        Volume = 2
    }

    public enum RayCastSource {
        MousePosition = 0,
        CameraDirection = 1
    }


    [RequireComponent(typeof(HighlightEffect))]
    [ExecuteInEditMode]
    [HelpURL("https://kronnect.com/guides/highlight-plus-introduction/")]
    public class HighlightTrigger : MonoBehaviour {

        [Tooltip("Enables highlight when pointer is over this object.")]
        public bool highlightOnHover = true;
        [Tooltip("Used to trigger automatic highlighting including children objects.")]
#if ENABLE_INPUT_SYSTEM
        public TriggerMode triggerMode = TriggerMode.RaycastOnThisObjectAndChildren;
#else
        public TriggerMode triggerMode = TriggerMode.ColliderEventsOnlyOnThisObject;
#endif
        public Camera raycastCamera;
        public RayCastSource raycastSource = RayCastSource.MousePosition;
        public LayerMask raycastLayerMask = -1;
        [Tooltip("Minimum distance for target.")]
        public float minDistance;
        [Tooltip("Maximum distance for target. 0 = infinity")]
        public float maxDistance;
        [Tooltip("Blocks interaction if pointer is over an UI element")]
        public bool respectUI = true;
        [Tooltip("Unhighlights the object when the pointer is over a UI element")]
        public bool unhighlightOnUI;
        public LayerMask volumeLayerMask;

        const int MAX_RAYCAST_HITS = 100;


        [Tooltip("If the object will be selected by clicking with mouse or tapping on it.")]
        public bool selectOnClick;
        [Tooltip("Profile to use when object is selected by clicking on it.")]
        public HighlightProfile selectedProfile;
        [Tooltip("Profile to use whtn object is selected and highlighted.")]
        public HighlightProfile selectedAndHighlightedProfile;
        [Tooltip("Automatically deselects any other selected object prior selecting this one")]
        public bool singleSelection;
        [Tooltip("Toggles selection on/off when clicking object")]
        public bool toggle;
        [Tooltip("Keeps current selection when clicking outside of any selectable object")]
        public bool keepSelection = true;

        [NonSerialized] public Collider[] colliders;
        [NonSerialized] public Collider2D[] colliders2D;

        public bool hasColliders => colliders != null && colliders.Length > 0;
        public bool hasColliders2D => colliders2D != null && colliders2D.Length > 0;

        UnityEngine.Object currentCollider;
        static RaycastHit[] hits;
        static RaycastHit2D[] hits2D;
        HighlightEffect hb;

        public HighlightEffect highlightEffect { get { return hb; } }

        public event OnObjectSelectionEvent OnObjectSelected;
        public event OnObjectSelectionEvent OnObjectUnSelected;
        public event OnObjectHighlightEvent OnObjectHighlightStart;
        public event OnObjectHighlightEvent OnObjectHighlightStay;
        public event OnObjectHighlightEvent OnObjectHighlightEnd;
        public event OnObjectClickEvent OnObjectClicked;

        TriggerMode currentTriggerMode;


        [RuntimeInitializeOnLoadMethod]
        static void DomainReloadDisabledSupport () {
            HighlightManager.selectedObjects.Clear();
        }

        void OnEnable () {
            Init();
        }

        private void OnValidate () {
            if (currentTriggerMode != triggerMode) {
                UpdateTriggers();
            }
        }

        void UpdateTriggers () {
            currentTriggerMode = triggerMode;
            if (currentTriggerMode == TriggerMode.RaycastOnThisObjectAndChildren) {
                colliders = GetComponentsInChildren<Collider>();
                colliders2D = GetComponentsInChildren<Collider2D>();
                if (hits == null || hits.Length != MAX_RAYCAST_HITS) {
                    hits = new RaycastHit[MAX_RAYCAST_HITS];
                }
                if (hits2D == null || hits2D.Length != MAX_RAYCAST_HITS) {
                    hits2D = new RaycastHit2D[MAX_RAYCAST_HITS];
                }
                if (Application.isPlaying) {
                    StopAllCoroutines();
                    if (isActiveAndEnabled) {
                        StartCoroutine(DoRayCast());
                    }
                }
            }
        }


        public void Init () {
            if (raycastCamera == null) {
                raycastCamera = HighlightManager.GetCamera();
            }
            UpdateTriggers();
            if (hb == null) {
                hb = GetComponent<HighlightEffect>();
            }
            InputProxy.Init();
        }

        void Start () {
            UpdateTriggers();
            if (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren) {
                if (raycastCamera == null) {
                    raycastCamera = HighlightManager.GetCamera();
                    if (raycastCamera == null) {
                        Debug.LogError("Highlight Trigger on " + gameObject.name + ": no camera found!");
                    }
                }
            }
            else {
                if (!TryGetComponent(out Collider _)) {
                    if (TryGetComponent(out MeshFilter _)) {
                        gameObject.AddComponent<MeshCollider>();
                    }
                }
            }
        }


        IEnumerator DoRayCast () {
            yield return null;
            WaitForEndOfFrame w = new WaitForEndOfFrame();
            while (triggerMode == TriggerMode.RaycastOnThisObjectAndChildren) {
                if (raycastCamera == null || !isActiveAndEnabled) {
                    yield return null;
                    continue;
                }

                int hitCount;
                bool hit = false;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
                if (respectUI) {
                    EventSystem es = EventSystem.current;
                    if (es == null) {
                        es = CreateEventSystem();
                    }
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    PointerEventData eventData = new PointerEventData(es);
                    Vector3 cameraPos = raycastCamera.transform.position;
                    if (raycastSource == RayCastSource.MousePosition) {
                        eventData.position = InputProxy.mousePosition;
                    } else {
                        eventData.position = new Vector2(raycastCamera.pixelWidth * 0.5f, raycastCamera.pixelHeight * 0.5f);
                    }
                    es.RaycastAll(eventData, raycastResults);
                    hitCount = raycastResults.Count;
                    // check UI blocker
                    bool blocked = false;
                    for (int k = 0; k < hitCount; k++) {
                        RaycastResult rr = raycastResults[k];
                        if (rr.module is UnityEngine.UI.GraphicRaycaster) {
                            blocked = true;
                            break;
                        }
                    }
                    if (blocked) {
                        if (unhighlightOnUI && hb.highlighted) {
                            hb.SetHighlighted(false);
                        }
                        yield return null;
                        continue;
                    }
                    // look for our gameobject
                    for (int k = 0; k < hitCount; k++) {
                        RaycastResult rr = raycastResults[k];
                        float distance = Vector3.Distance(rr.worldPosition, cameraPos);
                        if (distance < minDistance || (maxDistance > 0 && distance > maxDistance)) continue;

                        GameObject theGameObject = rr.gameObject;
                        for (int c = 0; c < colliders.Length; c++) {
                            if (colliders[c].gameObject == theGameObject) {
                                Collider theCollider = colliders[c];
                                hit = true;
                                UpdateHitPosition(theCollider.transform, rr.worldPosition);
                                if (selectOnClick && InputProxy.GetMouseButtonDown(0)) {
                                    ToggleSelection();
                                    break;
                                } else if (theCollider != currentCollider) {
                                    SwitchCollider(theCollider);
                                    k = hitCount;
                                    break;
                                }
                            }
                        }
                    }
                }
                // if not blocked by UI and no hit found, fallback to raycast (required if no PhysicsRaycaster is present on the camera)

#endif
                Ray ray;
                if (raycastSource == RayCastSource.MousePosition) {
#if !(ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER)

                    if (!CanInteract()) {
                        if (unhighlightOnUI && hb.highlighted) {
                            hb.SetHighlighted(false);
                        }
                        yield return null;
                        continue;
                    }
#endif
                    ray = raycastCamera.ScreenPointToRay(InputProxy.mousePosition);
                }
                else {
                    ray = new Ray(raycastCamera.transform.position, raycastCamera.transform.forward);
                }


                VerifyHighlightStay();

                bool isMouseButonDown = InputProxy.GetMouseButtonDown(0);
                if (hasColliders2D) {
                    if (maxDistance > 0) {
                        hitCount = Physics2D.GetRayIntersectionNonAlloc(ray, hits2D, maxDistance, raycastLayerMask);
                    }
                    else {
                        hitCount = Physics2D.GetRayIntersectionNonAlloc(ray, hits2D, float.MaxValue, raycastLayerMask);
                    }
                    for (int k = 0; k < hitCount; k++) {
                        if (Vector3.Distance(hits2D[k].point, ray.origin) < minDistance) continue;
                        Collider2D theCollider = hits2D[k].collider;
                        int colliders2DCount = colliders2D.Length;
                        for (int c = 0; c < colliders2DCount; c++) {
                            if (colliders2D[c] == theCollider) {
                                hit = true;
                                UpdateHitPosition(hits2D[k].transform, hits2D[k].point);
                                if (selectOnClick && isMouseButonDown) {
                                    ToggleSelection();
                                    break;
                                }
                                else if (theCollider != currentCollider) {
                                    SwitchCollider(theCollider);
                                    k = hitCount;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (hasColliders) {
                    if (maxDistance > 0) {
                        hitCount = Physics.RaycastNonAlloc(ray, hits, maxDistance, raycastLayerMask);
                    }
                    else {
                        hitCount = Physics.RaycastNonAlloc(ray, hits, float.MaxValue, raycastLayerMask);
                    }
                    for (int k = 0; k < hitCount; k++) {
                        if (Vector3.Distance(hits[k].point, ray.origin) < minDistance) continue;
                        Collider theCollider = hits[k].collider;
                        int collidersCount = colliders.Length;
                        for (int c = 0; c < collidersCount; c++) {
                            if (colliders[c] == theCollider) {
                                hit = true;
                                UpdateHitPosition(hits[k].transform, hits[k].point);
                                if (selectOnClick && isMouseButonDown) {
                                    ToggleSelection();
                                    break;
                                }
                                else if (theCollider != currentCollider) {
                                    SwitchCollider(theCollider);
                                    k = hitCount;
                                    break;
                                }
                            }
                        }
                    }
                }


                if (!hit && currentCollider != null) {
                    SwitchCollider(null);
                }

                if (selectOnClick && isMouseButonDown && !keepSelection && !hit) {
                    yield return w; // wait for other potential triggers to act
                    if (HighlightManager.lastTriggerFrame < Time.frameCount) {
                        if (OnObjectUnSelected != null) OnObjectUnSelected(gameObject);
                        HighlightManager.DeselectAll();
                    }
                }

                yield return null;
            }
        }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        EventSystem CreateEventSystem() {
            GameObject eo = new GameObject("Event System created by Highlight Plus", typeof(EventSystem), typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
            return eo.GetComponent<EventSystem>();
        }
#endif

        void VerifyHighlightStay () {
            if (hb == null || !hb.highlighted) return;
            if (OnObjectHighlightStay != null && !OnObjectHighlightStay(hb.gameObject)) {
                SwitchCollider(null);
            }
        }

        void SwitchCollider (UnityEngine.Object newCollider) {
            if (!highlightOnHover && !hb.isSelected) return;

            currentCollider = newCollider;
            if (currentCollider != null) {
                Highlight(true);
            }
            else {
                Highlight(false);
            }
        }

        bool CanInteract () {
            if (!respectUI) return true;
            EventSystem es = EventSystem.current;
            if (es == null) return true;
            if (Application.isMobilePlatform && InputProxy.touchCount > 0 && es.IsPointerOverGameObject(InputProxy.GetFingerIdFromTouch(0))) {
                return false;
            }
            else if (es.IsPointerOverGameObject(-1))
                return false;
            return true;
        }


        void OnMouseDown () {
            if (isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject) {
                if (!CanInteract()) return;
                if (selectOnClick && InputProxy.GetMouseButtonDown(0)) {
                    ToggleSelection();
                    return;
                }
                Highlight(true);
            }
        }

        void OnMouseEnter () {
            if (isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject) {
                if (!CanInteract()) return;
                Highlight(true);
            }
        }

        void OnMouseExit () {
            if (isActiveAndEnabled && triggerMode == TriggerMode.ColliderEventsOnlyOnThisObject) {
                if (!CanInteract()) return;
                Highlight(false);
            }
        }

        void OnMouseOver () {
            if (hb == null || !hb.labelEnabled) return;
            Ray ray = raycastCamera.ScreenPointToRay(InputProxy.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                UpdateHitPosition(hit.transform, hit.point);
            }
            else {
                UpdateHitPosition(transform);
            }
        }

        void Highlight (bool state) {
            if (state) {
                if (!hb.highlighted) {
                    if (OnObjectHighlightStart != null && hb.target != null) {
                        if (!OnObjectHighlightStart(hb.target.gameObject)) {
                            currentCollider = null;
                            return;
                        }
                    }
                }
            }
            else {
                if (hb.highlighted) {
                    if (OnObjectHighlightEnd != null && hb.target != null) {
                        OnObjectHighlightEnd(hb.target.gameObject);
                    }
                }
            }
            if (selectOnClick || hb.isSelected) {
                if (hb.isSelected) {
                    if (state && selectedAndHighlightedProfile != null) {
                        selectedAndHighlightedProfile.Load(hb);
                    }
                    else if (selectedProfile != null) {
                        selectedProfile.Load(hb);
                    }
                    else {
                        hb.previousSettings.Load(hb);
                    }
                    if (hb.highlighted) {
                        hb.UpdateMaterialProperties();
                    }
                    else {
                        hb.SetHighlighted(true);
                    }
                    return;
                }
                else if (!highlightOnHover) {
                    hb.SetHighlighted(false);
                    return;
                }
            }
            hb.SetHighlighted(state);
        }

        void UpdateHitPosition (Transform target, Vector3 positionWS = default, Vector3 normalWS = default) {
            if (hb == null) return;
            Vector3 localPosition = target.InverseTransformPoint(positionWS);
            hb.SetHitPosition(target, localPosition, Misc.vector3Zero, normalWS);

            if (InputProxy.GetMouseButtonDown(0) && OnObjectClicked != null) {
                OnObjectClicked(target.gameObject, positionWS, normalWS);
            }
        }


        void ToggleSelection () {

            HighlightManager.lastTriggerFrame = Time.frameCount;

            bool newState = toggle ? !hb.isSelected : true;
            if (newState) {
                if (OnObjectSelected != null && !OnObjectSelected(gameObject)) return;
            }
            else {
                if (OnObjectUnSelected != null && !OnObjectUnSelected(gameObject)) return;
            }

            if (singleSelection && newState) {
                HighlightManager.DeselectAll();
            }
            hb.isSelected = newState;
            if (newState && !HighlightManager.selectedObjects.Contains(hb)) {
                HighlightManager.selectedObjects.Add(hb);
            }
            else if (!newState && HighlightManager.selectedObjects.Contains(hb)) {
                HighlightManager.selectedObjects.Remove(hb);
            }

            if (hb.isSelected) {
                if (hb.previousSettings == null) {
                    hb.previousSettings = ScriptableObject.CreateInstance<HighlightProfile>();
                }
                hb.previousSettings.Save(hb);
            }
            else {
                hb.RestorePreviousHighlightEffectSettings();
            }

            Highlight(true);
        }

        public void OnTriggerEnter (Collider other) {
            if (triggerMode == TriggerMode.Volume) {
                if ((volumeLayerMask & (1 << other.gameObject.layer)) != 0) {
                    Highlight(true);
                }
            }
        }

        public void OnTriggerExit (Collider other) {
            if (triggerMode == TriggerMode.Volume) {
                if ((volumeLayerMask & (1 << other.gameObject.layer)) != 0) {
                    Highlight(false);
                }
            }
        }
    }

}