using UnityEngine;

public class UIWindowLoading : UIGenericWindow {
    #region Serialized Fields
    [Header("Sub Content")]
    [SerializeField] private GameObject[] _goPokemonArray = null;
    #endregion

    #region Internal Fields
    private int _pokemonIndex = 0;
    #endregion

    #region Exposed Fields
    public override string WindowName => Define.WIDNOW_LOADING;
    #endregion

    #region Override Methods
    protected override void OnWindowEnable() {
        HideAllPokemon();
        ShowRandomPokemon();
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
