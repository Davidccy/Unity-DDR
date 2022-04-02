using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowCutScene : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Image _image = null;
    [SerializeField] private CustomPlayableDirector _cpdFadeIn = null;
    [SerializeField] private CustomPlayableDirector _cpdFadeOut = null;
    #endregion

    #region APIs
    public void SetColor(Color c) {
        _image.color = c;
    }

    public async Task PlayFadeIn() {
        _cpdFadeOut.Stop();

        await _cpdFadeIn.Play();
    }

    public async Task PlayFadeOut() {
        _cpdFadeIn.Stop();

        await _cpdFadeOut.Play();
    }
    #endregion
}
