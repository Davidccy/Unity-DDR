using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneResult : SceneBase {
    #region Serialized Fields
    [SerializeField] private UIResult _uiResult = null;
    #endregion

    #region Override Methods
    protected override void OnSceneAwake() {
        Init();
    }
    #endregion

    #region Internal Methods
    private async void Init() {
        await CommonWindowManager.Instance.CutSceneFadeOut();
    }
    #endregion
}
