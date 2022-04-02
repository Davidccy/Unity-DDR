using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIResult : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshPro _textTrackName = null;
    [SerializeField] private TextMeshPro _textTrackBPM = null;

    [SerializeField] private Button _btnNext = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btnNext.onClick.AddListener(ButtonNextOnClick);
    }

    private void OnDestroy() {
        _btnNext.onClick.RemoveListener(ButtonNextOnClick);
    }
    #endregion

    #region Internal Methods
    private void ButtonNextOnClick() {
        Debug.LogErrorFormat("ButtonNextOnClick");

        ResultFinish();
    }

    private async void ResultFinish() {
        CommonWindowManager.Instance.SetCutSceneColor(Color.white);
        await CommonWindowManager.Instance.CutSceneFadeIn();
        SceneManager.LoadScene(Define.SCENE_MAIN, LoadSceneMode.Single);
    }
    #endregion
}
