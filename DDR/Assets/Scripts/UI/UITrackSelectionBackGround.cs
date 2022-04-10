using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UITrackSelectionBackGround : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private RectTransform _rectRoot = null;
    [SerializeField] private Image _imageRes = null;
    [SerializeField] private float _speed = 0;
    #endregion

    #region Internal Fields
    private CancellationTokenSource _cts = null;
    private List<RectTransform> _rectImageList = new List<RectTransform>();
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        Init();
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
    private void Init() {
        // Remove old images
        if (_rectImageList != null && _rectImageList.Count > 0) {
            for (int i = 0; i < _rectImageList.Count; i++) {
                Destroy(_rectImageList[i].gameObject);
            }
        }
        _rectImageList.Clear();

        // Recalculate size
        float scaleX = _rectRoot.rect.width / _imageRes.sprite.rect.width;
        float scaleY = _rectRoot.rect.height / _imageRes.sprite.rect.height;
        float scale = Mathf.Max(scaleX, scaleY);

        Vector2 imageSize = new Vector2(_imageRes.sprite.rect.width, _imageRes.sprite.rect.height) * scale;

        // Generate new images
        int requiredCount = Mathf.CeilToInt(_rectRoot.rect.width / imageSize.x) + 1;
        for (int i = 0; i < requiredCount; i++) {
            Image newImage = Instantiate(_imageRes, this.transform);
            RectTransform newRectImage = newImage.GetComponent<RectTransform>();
            newRectImage.anchorMin = new Vector2(0, 0.5f);
            newRectImage.anchorMax = new Vector2(0, 0.5f);
            newRectImage.pivot = new Vector2(0, 0.5f);
            newRectImage.sizeDelta = imageSize;

            float posX = (i - 1) * imageSize.x;
            newRectImage.anchoredPosition = new Vector2(posX, 0);

            _rectImageList.Add(newRectImage);
        }
    }

    private async Task PlayTween(CancellationToken ct) {
        float posX = 0;
        float posY = 0;

        while (true) {
            if (ct.IsCancellationRequested) {
                return;
            }

            float imageWidth = _rectImageList[0].rect.width;
            int imageCount = _rectImageList.Count;
            for (int i = 0; i < _rectImageList.Count; i++) {
                posX = _rectImageList[i].anchoredPosition.x + _speed;
                posY = _rectImageList[i].anchoredPosition.y;

                // If out of screen on the left side, swap this hint to the right side
                if (posX > _rectRoot.rect.width) {
                    posX -= _rectImageList.Count * imageWidth;
                }

                _rectImageList[i].anchoredPosition = new Vector2(posX, posY);
            }

            await Task.Delay(1);
        }
    }
    #endregion
}
