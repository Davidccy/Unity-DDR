using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimatedGif : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Image _imageIcon = null;
    [SerializeField] private Sprite[] _spriteArray = null;
    [SerializeField] private float _period = 0;
    [SerializeField] private bool _playOnAwake = false;
    #endregion

    #region Internal Fields
    private bool _playAnimation = false;
    private int _spriteIndex = 0;
    private float _timer = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        if (_playOnAwake) {
            PlayAnimation();
        }
    }
        
    private void Update() {
        RefreshSprite();
    }
    #endregion

    #region APIs
    private void SetSprites(Sprite[] spriteArray) {
        _spriteArray = spriteArray;
    }

    private void SetPeriod(float period) {
        _period = period;
    }
    #endregion

    #region Internal Methods
    private void RefreshSprite() {
        if (!_playAnimation) {
            return;
        }

        _timer += Time.deltaTime;
        if (_timer > _period) {
            _timer -= _period;
            _spriteIndex += 1;
        }

        if (_spriteIndex >= _spriteArray.Length) {
            _spriteIndex -= _spriteArray.Length;
        }

        _imageIcon.sprite = _spriteArray[_spriteIndex];
    }

    private void PlayAnimation() {
        _playAnimation = true;
        _timer = 0;
    }

    private void StopAnimation() {
        _playAnimation = false;
        _timer = 0;
    }
    #endregion
}
