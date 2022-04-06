using System.Threading.Tasks;
using UnityEngine;

public class UIWindowLoading : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private CustomPlayableDirector _cpdFadeIn = null;
    [SerializeField] private CustomPlayableDirector _cpdFadeOut = null;

    [SerializeField] private GameObject[] _goPokemonArray = null;
    #endregion

    #region Internal Fields
    private int _pokemonIndex = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        HideAllPokemon();
    }
    #endregion

    #region APIs
    public async Task PlayFadeIn() {
        ShowRandomPokemon();

        _cpdFadeOut.Stop();

        await _cpdFadeIn.Play();
    }

    public async Task PlayFadeOut() {
        _cpdFadeIn.Stop();

        await _cpdFadeOut.Play();
    }
    #endregion

    #region Internal Methods
    private void HideAllPokemon() {
        for (int i = 0; i < _goPokemonArray.Length; i++) {
            _goPokemonArray[i].SetActive(false);
        }
    }

    private void ShowRandomPokemon() {
        _goPokemonArray[_pokemonIndex].SetActive(false);

        _pokemonIndex = Random.Range(0, _goPokemonArray.Length);
        _goPokemonArray[_pokemonIndex].SetActive(true);
    }
    #endregion
}
