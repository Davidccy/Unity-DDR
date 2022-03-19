using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIHitEffect : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Image _image = null;
    [SerializeField] private CanvasGroup _cg = null;

    [SerializeField] private float _startScale = 0;
    [SerializeField] private float _endScale = 0;   // End scale should be larger than start scale
    [SerializeField] private float _startAlpha = 0;

    [SerializeField] private AnimationCurve _aniCurveScale = null;
    [SerializeField] private AnimationCurve _aniCurveAlpha = null;

    [SerializeField] private float _duration = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        _cg.alpha = 0;
    }
    #endregion

    #region APIs
    public void Play(TapResult tr) {
        StopAllCoroutines();

        SetEffectColor(tr);
        StartCoroutine(PlayFadeOut());
    }
    #endregion

    #region Internal Methods
    private void SetEffectColor(TapResult tr) {
        _image.color = Utility.GetTapResultColor(tr);
    }

    private IEnumerator PlayFadeOut() {
        float passedTime = 0;
        float progress = 0;

        while (passedTime < _duration) {
            progress = passedTime != 0 ? Mathf.Clamp(passedTime / _duration, 0.0f, 1.0f) : 0;

            this.transform.localScale = Vector3.one * Mathf.Lerp(_startScale, _endScale, _aniCurveScale.Evaluate(progress));
            _cg.alpha = Mathf.Lerp(_startAlpha, 0, _aniCurveAlpha.Evaluate(progress));

            yield return new WaitForEndOfFrame();

            passedTime += Time.deltaTime;
        }

        this.transform.localScale = Vector3.one * _endScale;
        _cg.alpha = 0;
    }
    #endregion
}
