using UnityEngine;
using UnityEngine.UI;

public class UIMainPageEntry : UIMainPageBase {
    #region Serialized Fields
    [Header("Sub Content")]
    [SerializeField] private Button _btnEntry = null;
    [SerializeField] private Button _btnCredits = null;
    [SerializeField] private GameObject[] _goWallpaper = null;
    [SerializeField] private CustomPlayableDirector _cpdIdle = null;
    #endregion

    #region Internal Fields
    private int _wallpaperIndex = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btnEntry.onClick.AddListener(ButtonEntryOnClick);
        _btnCredits.onClick.AddListener(ButtonCreditsOnClick);
    }

    private void OnEnable() {
        ActiveRandomWallpaper();
    }

    private void OnDestroy() {
        _btnEntry.onClick.RemoveListener(ButtonEntryOnClick);
        _btnCredits.onClick.RemoveListener(ButtonEntryOnClick);
    }
    #endregion

    #region Override Methods
    public override void OnFadeInDone() {
        _cpdIdle.Play().DoNotAwait();
    }

    public override void OnFadeOutDone() {
        _cpdIdle.Stop();
    }
    #endregion

    #region Button Handlings
    private async void ButtonEntryOnClick() {
        await _sceneMain.ChangeToPage(SceneMain.UIPage.TrackSelection);
    }

    private async void ButtonCreditsOnClick() {
        if (WindowManager.Instance != null) {
            await WindowManager.Instance.OpenWindow(Define.WIDNOW_CREDITS);
        }
    }
    #endregion

    #region Internal Methods
    private void ActiveRandomWallpaper() {
        int newWallpaperIndex = Random.Range(0, _goWallpaper.Length);

        _goWallpaper[_wallpaperIndex].SetActive(false);
        _goWallpaper[newWallpaperIndex].SetActive(true);

        _wallpaperIndex = newWallpaperIndex;
    }
    #endregion
}
