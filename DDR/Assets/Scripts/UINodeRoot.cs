using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class UINodeRoot : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private NodePosition _nPos = default;
    [SerializeField] private Image _imageArrow = null;
    [SerializeField] private TextMeshProUGUI _textTapResult = null;
    [SerializeField] private PlayableDirector _pdTapResult = null;
    [SerializeField] private UIHitEffect _uiHitResult = null;
    [SerializeField] private float _scaleBump = 0.9f;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        EventManager.Instance.Register(EventTypes.BUMP, OnBumpTrigger);
        EventManager.Instance.Register(EventTypes.TAP_RESULT, OnTapResult);
    }

    private void OnDisable() {
        if (EventManager.Instance == null) {
            EventManager.Instance.Unregister(EventTypes.BUMP, OnBumpTrigger);
            EventManager.Instance.Unregister(EventTypes.TAP_RESULT, OnTapResult);
        }        
    }
    #endregion

    #region Event Handlings
    private void OnBumpTrigger(BaseEventArgs args) {
        BumpEventArgs bArgs = args as BumpEventArgs;

        if (bArgs != null) {
            PlayBouncing();
        }
    }

    private void OnTapResult(BaseEventArgs args) {
        TapResultEventArgs trArgs = args as TapResultEventArgs;
        if (trArgs.NP != _nPos) {
            return;
        }

        TapResult tr = trArgs.TR;
        PlayTapResultEffect(tr);
        PlayTapResultSountEffect(tr);
    }
    #endregion

    #region Internal Methods
    private void PlayBouncing() {
        StopAllCoroutines();
        StartCoroutine(StartBouncing());
    }

    private void PlayTapResultEffect(TapResult tr) {
        _textTapResult.text = Utility.GetTapResultText(tr);
        _pdTapResult.Stop();
        _pdTapResult.Play();

        _textTapResult.color = Utility.GetTapResultColor(tr);
        _uiHitResult.Play(tr);
    }

    private void PlayTapResultSountEffect(TapResult tr) {
        // TODO
    }

    private IEnumerator StartBouncing() {
        // NOTE:
        // Bouncing frequency is depend on track BPM

        int bpm = TrackManager.Instance.BPM;
        float bps = (float) bpm / 60;
        float spb = 1 / bps;
        float duration = spb / 2;

        float passedTime = 0;
        while (passedTime < duration) {

            float progress = passedTime / duration;
            _imageArrow.transform.localScale = Vector3.Lerp(Vector3.one * _scaleBump, Vector3.one, progress);

            yield return new WaitForEndOfFrame();

            passedTime += Time.deltaTime;
        }

        _imageArrow.transform.localScale = Vector3.one;
    }
    #endregion
}
