using System.Threading.Tasks;
using UnityEngine;

public class UIAchievementEffect : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private AudioClip _ac = null;
    [SerializeField] private CustomPlayableDirector _cpd = null;
    [SerializeField] private ParticleSystem[] _psArray = null;
    #endregion

    #region APIs
    public async Task Play() {
        if (_ac != null) {
            AudioManager.Instance.PlaySE(_ac);
        }

        if (_psArray != null) {
            for (int i = 0; i < _psArray.Length; i++) {
                _psArray[i].Play();
            }
        }

        if (_cpd != null) {
            await _cpd.Play();
        }
    }

    public void Stop() {
        if (_ac != null) {
            AudioManager.Instance.StopSE();
        }

        if (_psArray != null) {
            for (int i = 0; i < _psArray.Length; i++) {
                _psArray[i].Stop();
            }
        }

        if (_cpd != null) {
            _cpd.Stop();
        }
    }
    #endregion
}
