using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIAssistant
{
    [HelpURL("https://sites.google.com/view/uiassistant/documentation/animation?authuser=0#h.xo3ieust7gfy")]
    [AddComponentMenu("UI Assistant/UI Animator"), RequireComponent(typeof(CanvasGroup)), RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class UIAnimator : UIAssistantComponent
    {
        #region Variables
        [Tooltip("Determines the Game Object's position, rotation, scale, and alpha in its hidden state.")] public AnimationProfile AnimationProfile;
        [Tooltip("If set to true, the Game Object will appear in its hidden state by default.")] public bool StartHidden;
        [Tooltip("Uncheck this box to allow the Show/Hide methods to override animations in progress.")] public bool WaitForAnimation = true;
        [Tooltip("Check this box if the Game Object's Rect Transform and/or Canvas Group is manipulated by scripts other than this.")] public bool IsDynamic;
        [Tooltip("Unity Events to be invoked at the end of the Show animation.")] public UnityEvent OnShow;
        [Tooltip("Unity Events to be invoked at the end of the Hide animation.")] public UnityEvent OnHide;

        [SerializeField] RectTransform RectTransform;
        [SerializeField] CanvasGroup CanvasGroup;

        [SerializeField] bool ToggleInteractability;
        [SerializeField] bool HorizontalStretch;
        [SerializeField] bool VerticalStretch;

        [SerializeField] Vector2 DefaultOffsetMin;
        [SerializeField] Vector2 DefaultOffsetMax;
        [SerializeField] Vector3 DefaultAngle;
        [SerializeField] Vector2 DefaultScale;
        [SerializeField] float DefaultAlpha;

        [SerializeField] Vector2 HiddenOffsetMin;
        [SerializeField] Vector2 HiddenOffsetMax;
        [SerializeField] Vector3 HiddenAngle;
        [SerializeField] Vector2 HiddenScale;
        [SerializeField] float HiddenAlpha;

        bool Visible = true;
        bool Disabled;
        Vector3 CurrentAngle;
        Coroutine StateTransitionCoroutine;
        #endregion

        #region Function
        void OnEnable()
        {
            Validate();

            if (StartHidden) HideInstantly();
        }
        void Validate()
        {
            if (RectTransform == null) RectTransform = GetComponent<RectTransform>();
            if (CanvasGroup == null) CanvasGroup = GetComponent<CanvasGroup>();

            if (AnimationProfile == null)
            {
                if (AnimationSettings.AnimationProfiles.Count > 0)
                    AnimationProfile = AnimationSettings.AnimationProfiles[0];
                else return;
            }

            SetDefaultValues();
            CalculateHiddenValues();
        }
        void SetDefaultValues()
        {
            ToggleInteractability = CanvasGroup.interactable;

            HorizontalStretch = RectTransform.anchorMin.x != RectTransform.anchorMax.x;
            VerticalStretch = RectTransform.anchorMin.y != RectTransform.anchorMax.y;

            DefaultOffsetMin.x = RectTransform.offsetMin.x;
            DefaultOffsetMax.x = RectTransform.offsetMax.x;
            DefaultOffsetMin.y = RectTransform.offsetMin.y;
            DefaultOffsetMax.y = RectTransform.offsetMax.y;
            DefaultAngle = RectTransform.localEulerAngles;
            DefaultScale = RectTransform.localScale;
            DefaultAlpha = CanvasGroup.alpha;
        }
        void CalculateHiddenValues()
        {
            if (HorizontalStretch)
            {
                HiddenOffsetMin.x = RectTransform.offsetMin.x - AnimationProfile.HiddenPositionOffset.x;
                HiddenOffsetMax.x = RectTransform.offsetMax.x + AnimationProfile.HiddenPositionOffset.x;
            }
            else
            {
                float xMultiplier = 0;
                if (RectTransform.anchorMin.x < .5f) xMultiplier = -1;
                else if (RectTransform.anchorMin.x > .5f) xMultiplier = 1;
                float xOffset = AnimationProfile.HiddenPositionOffset.x * xMultiplier;
                HiddenOffsetMin.x = RectTransform.offsetMin.x + xOffset;
                HiddenOffsetMax.x = RectTransform.offsetMax.x + xOffset;
            }

            if (VerticalStretch)
            {
                HiddenOffsetMin.y = RectTransform.offsetMin.y - AnimationProfile.HiddenPositionOffset.y;
                HiddenOffsetMax.y = RectTransform.offsetMax.y + AnimationProfile.HiddenPositionOffset.y;
            }
            else
            {
                float yMultiplier = 0;
                if (RectTransform.anchorMin.y < .5f) yMultiplier = -1;
                else if (RectTransform.anchorMin.y > .5f) yMultiplier = 1;
                float yOffset = AnimationProfile.HiddenPositionOffset.y * yMultiplier;
                HiddenOffsetMin.y = RectTransform.offsetMin.y + yOffset;
                HiddenOffsetMax.y = RectTransform.offsetMax.y + yOffset;
            }

            HiddenAngle = new(0, 0, DefaultAngle.z + AnimationProfile.HiddenAngleOffset);
            HiddenScale = new(DefaultScale.x * AnimationProfile.HiddenScale.x, DefaultScale.y * AnimationProfile.HiddenScale.y);
            HiddenAlpha = AnimationProfile.HiddenAlpha;
        }
        
        /// <summary>
        /// Sets Rect Transform and Canvas Group parameters based on the selected Animation Profile's hidden state.
        /// </summary>
        public void HideInstantly()
        {
            Visible = false;

            RectTransform.offsetMin = HiddenOffsetMin;
            RectTransform.offsetMax = HiddenOffsetMax;
            CurrentAngle = HiddenAngle;
            RectTransform.localEulerAngles = CurrentAngle;
            RectTransform.localScale = HiddenScale;
            CanvasGroup.alpha = HiddenAlpha;
            if (ToggleInteractability)
            {
                CanvasGroup.interactable = false;
                CanvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// Animates the Rect Transform and/or fades the Canvas Group into its default state.
        /// </summary>
        public void Show()
        {
            if (Disabled || AnimationProfile == null) return;

            if (!WaitForAnimation) Visible = false;
            else if (Visible) return;

            Disabled = WaitForAnimation;

            StartStateTransition();
        }
        void OnShowComplete()
        {
            Visible = true;
            Disabled = false;

            if (ToggleInteractability)
            {
                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;
            }

            OnShow.Invoke();
        }

        /// <summary>
        /// Animates the Rect Transform and/or fades the Canvas Group into its hidden state, defined in the selected Animation Profile.
        /// </summary>
        public void Hide()
        {
            if (Disabled || AnimationProfile == null) return;

            if (!WaitForAnimation) Visible = true;
            else if (!Visible) return;

            Disabled = WaitForAnimation;

            if (IsDynamic)
            {
                SetDefaultValues();
                CalculateHiddenValues();
            }

            if (ToggleInteractability)
            {
                CanvasGroup.interactable = false;
                CanvasGroup.blocksRaycasts = false;
            }

            StartStateTransition();
        }
        void OnHideComplete()
        {
            Visible = false;
            Disabled = false;

            OnHide.Invoke();
        }

        /// <summary>
        /// Switches between the UI Animator's default and hidden states, based on its current state.
        /// </summary>
        public void Toggle()
        {
            if (Visible) Hide();
            else Show();
        }

        void StartStateTransition()
        {
            if (!gameObject.activeInHierarchy) return;

            StopStateTransition();
            StateTransitionCoroutine = StartCoroutine(StateTransition());
        }
        IEnumerator StateTransition()
        {
            Vector2 targetOffsetMin = Visible ? HiddenOffsetMin : DefaultOffsetMin;
            Vector2 targetOffsetMax = Visible ? HiddenOffsetMax : DefaultOffsetMax;
            Vector3 targetAngle = Visible ? HiddenAngle : DefaultAngle;
            Vector2 targetScale = Visible ? HiddenScale : DefaultScale;
            float targetAlpha = Visible ? HiddenAlpha : DefaultAlpha;

            Vector2 startOffsetMin = RectTransform.offsetMin;
            Vector2 startOffsetMax = RectTransform.offsetMax;
            Vector3 startAngle = CurrentAngle;
            Vector2 startScale = RectTransform.localScale;
            float startAlpha = CanvasGroup.alpha;

            float progress = 0;
            float transitionTime = AnimationProfile.TransitionTime;

            while (progress < transitionTime)
            {
                progress += Time.deltaTime;
                float t = AnimationProfile.TransitionCurve.Evaluate(progress / transitionTime);

                RectTransform.offsetMin = Vector2.Lerp(startOffsetMin, targetOffsetMin, t);
                RectTransform.offsetMax = Vector2.Lerp(startOffsetMax, targetOffsetMax, t);
                CurrentAngle = Vector3.Lerp(startAngle, targetAngle, t);
                RectTransform.localEulerAngles = CurrentAngle;
                RectTransform.localScale = Vector2.Lerp(startScale, targetScale, t);
                CanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

                yield return null;
            }

            StateTransitionCoroutine = null;

            if (Visible) OnHideComplete();
            else OnShowComplete();
        }
        void StopStateTransition()
        {
            if (StateTransitionCoroutine != null)
            {
                StopCoroutine(StateTransitionCoroutine);
                StateTransitionCoroutine = null;
            }
        }
        #endregion

#if UNITY_EDITOR

        #region Function
        protected override void OnValidate()
        {
            base.OnValidate();

            Validate();
        }
        #endregion

#endif
    }
}