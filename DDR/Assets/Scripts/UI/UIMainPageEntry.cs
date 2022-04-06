using UnityEngine;
using UnityEngine.UI;

public class UIMainPageEntry : UIMainPageBase {
    #region Serialized Fields
    [Header("Sub Content")]
    [SerializeField] private Button _btnEntry = null;
    [SerializeField] private GameObject[] _goWallpaper = null;
    #endregion

    #region Internal Fields
    private int _wallpaperIndex = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btnEntry.onClick.AddListener(ButtonEntryonClick);
    }

    private void OnEnable() {
        ActiveRandomWallpaper();
    }

    private void OnDestroy() {
        _btnEntry.onClick.RemoveListener(ButtonEntryonClick);
    }
    #endregion

    #region Button Handlings
    private async void ButtonEntryonClick() {
        await _sceneMain.ChangeToPage(SceneMain.UIPage.TrackSelection);
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
