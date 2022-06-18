using UnityEngine;
using UnityEngine.UI;

public class UIWallpaperResizer : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private RectTransform _rectReference = null;
    [SerializeField] private Image _imageWallpaper = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        GameEventManager.Instance.Register(GameEventTypes.RESOLUTION_CHANGED, OnResolutionChanged);

        Refresh();
    }

    private void OnDestroy() {
        if (GameEventManager.Instance != null) {
            GameEventManager.Instance.Unregister(GameEventTypes.RESOLUTION_CHANGED, OnResolutionChanged);
        }
    }
    #endregion

    #region Event Handlings
    private void OnResolutionChanged(BaseGameEventArgs args) {
        Refresh();
    }
    #endregion

    #region APIs
    public void SetSprite(Sprite sp) {
        _imageWallpaper.sprite = sp;

        Refresh();
    }
    #endregion

    #region Internal Methods
    private void Refresh() {
        if (_imageWallpaper == null || _imageWallpaper.sprite == null) {
            Debug.LogErrorFormat("Null wallpaper sprite to refresh");
            return;
        }

        float scaleX = _rectReference.rect.width / _imageWallpaper.sprite.rect.width;
        float scaleY = _rectReference.rect.height / _imageWallpaper.sprite.rect.height;

        float scale = Mathf.Max(scaleX, scaleY);

        RectTransform rect = this.transform as RectTransform;
        rect.sizeDelta = new Vector2(_imageWallpaper.sprite.rect.width, _imageWallpaper.sprite.rect.height) * scale;
    }
    #endregion
}
