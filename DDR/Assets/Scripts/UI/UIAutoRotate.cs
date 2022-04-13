using UnityEngine;

public class UIAutoRotate : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private RectTransform _rectTarget = null;
    [SerializeField] private float _speedX = 0;
    [SerializeField] private float _speedY = 0;
    [SerializeField] private float _speedZ = 0;
    #endregion

    #region Mono Behaviour Hook
    private void Update() {
        DoRotate();
    }
    #endregion

    #region Internal Methods
    private void DoRotate() {
        if (_rectTarget == null) {
            return;
        }

        _rectTarget.Rotate(new Vector3(_speedX, _speedY, _speedZ));
    }
    #endregion
}
