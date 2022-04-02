using UnityEngine;
using UnityEngine.UI;

public class UIMainPageEntry : UIMainPageBase {
    #region Serialized Fields
    [Header("Sub Content")]
    [SerializeField] private Button _btnEntry = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btnEntry.onClick.AddListener(ButtonEntryonClick);
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
}
