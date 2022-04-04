using System.Threading.Tasks;
using UnityEngine;

public class UIWindowLoading : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private CustomPlayableDirector _cpdFadeIn = null;
    [SerializeField] private CustomPlayableDirector _cpdFadeOut = null;
    #endregion

    #region APIs
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
