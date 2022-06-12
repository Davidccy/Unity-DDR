using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIWaveText : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _text = null;
    [SerializeField] private bool _playOnEnable = false;
    [SerializeField] private float _startDelay = 0;
    [SerializeField] private float _waveHeight = 0;
    [SerializeField] private float _waveDuration = 0;
    [SerializeField] private float _loopDuration = 0;
    [SerializeField] private float _nextCharacterInterval = 0;
    #endregion

    #region Internal Fields
    private bool _isPlaying = false;
    private float _passedTime = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void OnEnable() {
        Reset();

        if (_playOnEnable) {
            _isPlaying = true;
        }
    }

    private void OnDisable() {
        _isPlaying = false;
    }

    private void Update() {
        _passedTime += Time.deltaTime;
        UpdateWave();
    }
    #endregion

    #region APIs
    public void Play() {
        _isPlaying = true;
    }

    public void Stop() {
        _isPlaying = false;
    }
    #endregion

    #region Internal Methods
    private void Reset() {
        _passedTime = 0;
    }

    private void UpdateWave() {
        if (!_isPlaying) {
            return;
        }

        // Value restrictions
        if (_loopDuration <= 0) {
            _loopDuration = 1;
        }

        _waveDuration = Mathf.Clamp(_waveDuration, 0, _loopDuration);

        // Modify vertics (position of text)
        TMP_TextInfo tInfo = _text.textInfo;
        Dictionary<int, Vector3[]> newVertices = new Dictionary<int, Vector3[]>(); // key = materialReferenceIndex
        for (int i = 0; i < tInfo.characterCount; i++) {
            int characterIndex = i;
            TMP_CharacterInfo cInfo = tInfo.characterInfo[i];

            // Clone original vector data
            int meshInfoIndex = cInfo.materialReferenceIndex;
            if (!newVertices.ContainsKey(meshInfoIndex)) {
                Vector3[] oriVertices = tInfo.meshInfo[cInfo.materialReferenceIndex].vertices.Clone() as Vector3[];
                newVertices.Add(meshInfoIndex, oriVertices);
            }

            // Skip handling if not visible
            if (!cInfo.isVisible) {
                continue;
            }

            // Calculation of wave value
            float delay = _startDelay + _nextCharacterInterval * characterIndex;
            float validTime = _passedTime - delay;
            float wave = 0;
            if (validTime > 0) {
                if (_loopDuration == 0) {
                    _loopDuration = 1;
                }

                validTime %= _loopDuration;

                if (validTime <= _waveDuration) {
                    float degree = (validTime / _waveDuration) * Mathf.PI;
                    wave = _waveHeight * Mathf.Sin(degree);
                }
            }

            // Over write vertor value
            Vector3[] vertices = newVertices[meshInfoIndex];
            for (int j = 0; j < 4; j++) {
                Vector3 oriVertice = vertices[cInfo.vertexIndex + j];
                vertices[cInfo.vertexIndex + j] = oriVertice + new Vector3(0, wave, 0);
            }
        }

        // Update mesh
        for (int i = 0; i < tInfo.meshInfo.Length; i++) {
            if (!newVertices.ContainsKey(i)) {
                continue;
            }

            TMP_MeshInfo mInfo = tInfo.meshInfo[i];
            mInfo.mesh.vertices = newVertices[i];
            _text.UpdateGeometry(mInfo.mesh, i);
        }
    }
    #endregion
}
