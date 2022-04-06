using UnityEngine;
using UnityEngine.UI;

public class UIWallpaperResizer : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Canvas _canvas = null;
    [SerializeField] private Image _imageWallpaper = null;
    #endregion

    #region Internal Fields
    private RectTransform _rectCanvas = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _rectCanvas = _canvas.transform as RectTransform;

        Refresh();
    }
    #endregion

    #region Internal Methods
    private void Refresh() {
        float scaleX = _rectCanvas.sizeDelta.x / _imageWallpaper.sprite.rect.width;
        float scaleY = _rectCanvas.sizeDelta.y / _imageWallpaper.sprite.rect.height;

        float scale = Mathf.Max(scaleX, scaleY);

        RectTransform rect = this.transform as RectTransform;
        rect.sizeDelta = new Vector2(_imageWallpaper.sprite.rect.width, _imageWallpaper.sprite.rect.height) * scale;
    }
    #endregion
}
