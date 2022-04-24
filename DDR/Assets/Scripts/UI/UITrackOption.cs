using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITrackOption : MonoBehaviour {
    #region Serialized Fields
    [Header("Speed")]
    [SerializeField] private Slider _sliderSpeed = null;
    [SerializeField] private TextMeshProUGUI _textSpeedValue = null;

    [Header("Node Moving Type")]
    [SerializeField] private Button _btnNodeMovingFalling = null;
    [SerializeField] private Button _btnNodeMovingRaising = null;

    [Header("Control Type")]
    [SerializeField] private Button _btnControlKeyboard = null;
    [SerializeField] private Button _btnControlTouching = null;

    [Header("Icon Reference")]
    [SerializeField] private Image _imageSelected = null;
    [SerializeField] private Image _imageUnselected = null;
    #endregion

    #region Internal Fields
    private int _curSpeedLevel;
    private int _curNodeMovingType;
    private int _curControlType;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _sliderSpeed.onValueChanged.AddListener(SliderSpeedValueChanged);

        _btnNodeMovingFalling.onClick.AddListener(ButtonMovingFallingOnClick);
        _btnNodeMovingRaising.onClick.AddListener(ButtonMovingRaisingOnClick);

        _btnControlKeyboard.onClick.AddListener(ButtonControlKeyboardOnClick);
        _btnControlTouching.onClick.AddListener(ButtonControlTouchingOnClick);
    }

    private void OnEnable() {
        InitUI();
        Refresh();
    }

    private void OnDestroy() {
        _sliderSpeed.onValueChanged.RemoveListener(SliderSpeedValueChanged);

        _btnNodeMovingFalling.onClick.RemoveListener(ButtonMovingFallingOnClick);
        _btnNodeMovingRaising.onClick.RemoveListener(ButtonMovingRaisingOnClick);

        _btnControlKeyboard.onClick.RemoveListener(ButtonControlKeyboardOnClick);
        _btnControlTouching.onClick.RemoveListener(ButtonControlTouchingOnClick);
    }
    #endregion

    #region Slider Handlings
    private void SliderSpeedValueChanged(float value) {
        int intValue = Mathf.RoundToInt(value);
        if (_curSpeedLevel == intValue) {
            return;
        }

        GameDataManager.SaveInt(Define.GAME_DATA_KEY_SPEED_LEVEL, intValue);

        RefreshSpeedSetting();
    }
    #endregion

    #region Button Handlings
    private void ButtonMovingFallingOnClick() {
        if (_curNodeMovingType == (int) NodeMovingType.Falling) {
            return;
        }

        GameDataManager.SaveInt(Define.GAME_DATA_KEY_NODE_MOVING_TYPE, (int) NodeMovingType.Falling);

        RefreshMovingSetting();
    }

    private void ButtonMovingRaisingOnClick() {
        if (_curNodeMovingType == (int) NodeMovingType.Raising) {
            return;
        }

        GameDataManager.SaveInt(Define.GAME_DATA_KEY_NODE_MOVING_TYPE, (int) NodeMovingType.Raising);

        RefreshMovingSetting();
    }

    private void ButtonControlKeyboardOnClick() {
        if (_curControlType == (int) ControlType.Keyboard) {
            return;
        }

        GameDataManager.SaveInt(Define.GAME_DATA_KEY_CONTROL_TYPE, (int) ControlType.Keyboard);

        RefreshControlSetting();
    }

    private void ButtonControlTouchingOnClick() {
        if (_curControlType == (int) ControlType.Touching) {
            return;
        }

        GameDataManager.SaveInt(Define.GAME_DATA_KEY_CONTROL_TYPE, (int) ControlType.Touching);

        RefreshControlSetting();
    }
    #endregion

    #region Internal Methods
    private void InitUI() {
        _sliderSpeed.minValue = Define.SPEED_LEVEL_MIN;
        _sliderSpeed.maxValue = Define.SPEED_LEVEL_MAX;

        _curSpeedLevel = GameDataManager.LoadInt(Define.GAME_DATA_KEY_SPEED_LEVEL);
        _sliderSpeed.value = _curSpeedLevel;
    }

    private void Refresh() {
        RefreshSpeedSetting();
        RefreshMovingSetting();
        RefreshControlSetting();
    }

    private void RefreshSpeedSetting() {
        _curSpeedLevel = GameDataManager.LoadInt(Define.GAME_DATA_KEY_SPEED_LEVEL);

        float speedValue = Utility.GetTrackSpeed();
        _textSpeedValue.text = string.Format("{0:0.0}", speedValue);
    }

    private void RefreshMovingSetting() {
        _curNodeMovingType = GameDataManager.LoadInt(Define.GAME_DATA_KEY_NODE_MOVING_TYPE);
        _btnNodeMovingFalling.image.sprite = 
            _curNodeMovingType == (int) NodeMovingType.Falling ? _imageSelected.sprite : _imageUnselected.sprite;
        _btnNodeMovingRaising.image.sprite = 
            _curNodeMovingType == (int) NodeMovingType.Raising ? _imageSelected.sprite : _imageUnselected.sprite;
    }

    private void RefreshControlSetting() {
        _curControlType = GameDataManager.LoadInt(Define.GAME_DATA_KEY_CONTROL_TYPE);
        _btnControlKeyboard.image.sprite =
            _curControlType == (int) ControlType.Keyboard ? _imageSelected.sprite : _imageUnselected.sprite;
        _btnControlTouching.image.sprite =
            _curControlType == (int) ControlType.Touching ? _imageSelected.sprite : _imageUnselected.sprite;
    }
    #endregion
}
