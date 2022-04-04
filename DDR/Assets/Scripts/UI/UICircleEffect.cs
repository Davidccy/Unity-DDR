using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleEffect : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private float _maxSpeed = 0;
    [SerializeField] private float _swapThreshold = 0;
    #endregion

    #region Internal Fields
    private RectTransform _rectSelf = null;
    private float _speed = 0;
    private float _boundX = 0;
    private float _boundY = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _rectSelf = this.transform as RectTransform;
    }

    private void OnEnable() {
        _speed = Random.Range(0.1f, _maxSpeed);
    }

    private void Update() {
        UpdatePosition();        
    }
    #endregion

    #region APIs
    public void SetBound(float boundX, float boundY) {
        _boundX = boundX;
        _boundY = boundY;
    }
    #endregion

    #region Properties
    private float SwapThreshold {
        get {
            return Mathf.Max(0, _swapThreshold);
        }
    }
    #endregion

    #region Internal Methods
    private void UpdatePosition() {
        if (_rectSelf == null) {
            return;
        }
        
        float newPosX = _rectSelf.anchoredPosition.x - _speed;        
        float newPosY = _rectSelf.anchoredPosition.y + _speed;

        // Check is out of bound or not
        if (newPosX < -(_boundX / 2 + SwapThreshold)) {
            newPosX = _boundX / 2 + SwapThreshold;
        }

        if (newPosY > (_boundY / 2 + SwapThreshold)) {
            newPosY = -(_boundY + SwapThreshold);
        }

        // Set position
        _rectSelf.anchoredPosition = new Vector2(newPosX, newPosY);
    }
    #endregion
}
