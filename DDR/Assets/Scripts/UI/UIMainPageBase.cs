using System.Threading.Tasks;
using UnityEngine;

public class UIMainPageBase : MonoBehaviour {
    #region Serialized Fields
    [Header("Base Contents")]
    [SerializeField] protected SceneMain _sceneMain = null;

    [SerializeField] protected CustomPlayableDirector _cpdFadeIn = null;
    [SerializeField] protected CustomPlayableDirector _cpdFadeOut = null;
    #endregion

    #region APIs
    public async Task PlayFadeIn() {
        if (_cpdFadeIn != null) {
            await _cpdFadeIn.Play();
        }        
    }

    public async Task PlayFadeOut() {
        if (_cpdFadeOut != null) {
            await _cpdFadeOut.Play();
        }        
    }
    #endregion
}
