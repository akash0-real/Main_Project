using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PriceHawk.Managers
{
    /// <summary>
    /// Renders the slide-up toast notification (bottom-right).
    /// Attach to a Canvas child with an Animator that has bool "Visible".
    /// </summary>
    public class ToastManager : MonoBehaviour
    {
        [Header("References")]
        public GameObject  ToastPanel;
        public TMP_Text    MessageLabel;
        public Animator    ToastAnimator; // bool param "Visible"

        [Header("Timing")]
        public float DisplaySeconds = 2.8f;

        Coroutine _hideCoroutine;

        static readonly int VisibleHash = Animator.StringToHash("Visible");

        void Awake()
        {
            AppManager.Instance.OnToastRequested += Show;
            ToastPanel.SetActive(false);
        }

        void OnDestroy()
        {
            if (AppManager.Instance != null)
                AppManager.Instance.OnToastRequested -= Show;
        }

        public void Show(string message)
        {
            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);

            MessageLabel.text = message;
            ToastPanel.SetActive(true);

            if (ToastAnimator != null)
                ToastAnimator.SetBool(VisibleHash, true);

            _hideCoroutine = StartCoroutine(HideAfterDelay());
        }

        IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(DisplaySeconds);

            if (ToastAnimator != null)
            {
                ToastAnimator.SetBool(VisibleHash, false);
                // wait for exit animation
                yield return new WaitForSeconds(0.35f);
            }
            ToastPanel.SetActive(false);
        }
    }
}
