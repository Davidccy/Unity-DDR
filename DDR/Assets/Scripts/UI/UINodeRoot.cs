using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class UINodeRoot : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private NodePosition _nPos = default;
    [SerializeField] private GameObject _goIconRoot = null;
    [SerializeField] private GameObject _goArrowRoot = null;
    [SerializeField] private GameObject _goRectRoot = null;
    [SerializeField] private TextMeshProUGUI _textTapResult = null;
    [SerializeField] private PlayableDirector _pdTapResult = null;
    [SerializeField] private UIHitEffect _uiHitResult = null;
    [SerializeField] private float _scaleBump = 0.9f;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        _goArrowRoot.SetActive(_nPos != NodePosition.Space);
        _goRectRoot.SetActive(_nPos == NodePosition.Space);

        EventManager.Instance.Register(EventTypes.BUMP, OnBumpTrigger);
        EventManager.Instance.Register(EventTypes.TAP_RESULT, OnTapResult);
    }

    private void OnDisable() {
        if (EventManager.Instance != null) {
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
        _textTapResult.color = Utility.GetTapResultColor(tr);
        _pdTapResult.Stop();
        _pdTapResult.Play();

        if (tr != TapResult.Miss) {
            _uiHitResult.Play(tr);
        }
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
            _goIconRoot.transform.localScale = Vector3.Lerp(Vector3.one * _scaleBump, Vector3.one, progress);

            yield return new WaitForEndOfFrame();

            passedTime += Time.deltaTime;
        }

        _goIconRoot.transform.localScale = Vector3.one;
    }
    #endregion
}
