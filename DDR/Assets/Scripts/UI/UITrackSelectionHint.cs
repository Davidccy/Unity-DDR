using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UITrackSelectionHint : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private RectTransform _rectRoot = null;
    [SerializeField] private RectTransform[] _rectHints = null;
    [SerializeField] private float _warpThreshold = 0;
    [SerializeField] private float _speed = 0;
    #endregion

    #region Internal Fields
    private CancellationTokenSource _cts = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        InitPosition();
    }

    private void OnEnable() {
        Play().DoNotAwait();
    }

    private void OnDestroy() {
        Stop();
    }
    #endregion

    #region APIs
    public async Task Play() {
        Stop();

        _cts = new CancellationTokenSource();

        await PlayTween(_cts.Token);
    }

    public void Stop() {
        if (_cts != null) {
            _cts.Cancel();
        }

        _cts = null;
    }
    #endregion

    #region Internal Methods
    private void InitPosition() {
        float posX = 0;
        float posY = 0;

        float rootWidth = _rectRoot.rect.width;
        int hintCount = _rectHints.Length;
        float hintWidth = 0;
        if (hintCount > 0) {
            hintWidth = _rectHints[0].rect.width;
        }

        for (int i = 0; i < _rectHints.Length; i++) {
            posX = i * ((rootWidth + hintWidth) / hintCount);
            posY = 0;
            _rectHints[i].anchoredPosition = new Vector2(posX, posY);
        }
    }

    private async Task PlayTween(CancellationToken ct) {
        float posX = 0;
        float posY = 0;

        while (true) {
            if (ct.IsCancellationRequested) {
                return;
            }

            for (int i = 0; i < _rectHints.Length; i++) {
                posX = _rectHints[i].anchoredPosition.x + _speed;

                // If out of screen on the left side, swap this hint to the right side
                if (posX > _rectRoot.rect.width + _warpThreshold) {
                    posX -= (_rectRoot.rect.width + _rectHints[i].rect.width);
                }

                posY = _rectHints[i].anchoredPosition.y;

                _rectHints[i].anchoredPosition = new Vector2(posX, posY);
            }

            await Task.Delay(1);
        }
    }
    #endregion
}
