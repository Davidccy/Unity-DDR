using UnityEngine;
using System.Threading.Tasks;

public class SceneGame : SceneBase {
    #region Serialized Fields
    [SerializeField] private UISample _uiSample = null;
    #endregion

    #region Internal Fields
    private int _curPageIndex = -1;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        Init();
    }
    #endregion

    #region Internal Methods
    private async void Init() {
        await CommonWindowManager.Instance.CutSceneFadeOut();
        _uiSample.ButtonOnClick();
    }
    #endregion
}
