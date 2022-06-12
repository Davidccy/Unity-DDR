using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWindowTrackOption : UIGenericWindow {
    #region Serialized Fields
    [Header("Sub Content - Speed")]
    [SerializeField] private Slider _sliderSpeed = null;
    [SerializeField] private TextMeshProUGUI _textSpeedValue = null;

    [Header("Sub Content - Node Sound Perfect")]
    [SerializeField] private Button[] _btnNodeSoundPerfect = null;

    [Header("Sub Content - Node Sound Normal")]
    [SerializeField] private Button[] _btnNodeSoundNormal = null;

    [Header("Sub Content - Node Moving Type")]
    [SerializeField] private Button _btnNodeMovingRaising = null;
    [SerializeField] private Button _btnNodeMovingFalling = null;

    [Header("Sub Content - Control Type")]
    [SerializeField] private Button _btnControlKeyboard = null;
    [SerializeField] private Button _btnControlTouching = null;

    [Header("Sub Content - Icon Reference")]
    [SerializeField] private Image _imageSelected = null;
    [SerializeField] private Image _imageUnselected = null;
    #endregion

    #region Internal Fields
    private int _curSpeedLevel;
    private int _curNodeMovingType;
    private int _curControlType;
    #endregion

    #region Exposed Fields
    public override string WindowName => Define.WIDNOW_TRACK_OPTION;
    #endregion

    #region Override Methods
    protected override void OnWindowAwake() {
        _sliderSpeed.onValueChanged.AddListener(SliderSpeedValueChanged);

        for (int i = 0; i < _btnNodeSoundPerfect.Length; i++) {
            int index = i;
            _btnNodeSoundPerfect[i].onClick.AddListener(() => ButtonSoundPerfectOnClick(index));
        }

        for (int i = 0; i < _btnNodeSoundNormal.Length; i++) {
            int index = i;
            _btnNodeSoundNormal[i].onClick.AddListener(() => ButtonSoundNormalOnClick(index));
        }

        _btnNodeMovingRaising.onClick.AddListener(ButtonMovingRaisingOnClick);
        _btnNodeMovingFalling.onClick.AddListener(ButtonMovingFallingOnClick);

        _btnControlKeyboard.onClick.AddListener(ButtonControlKeyboardOnClick);
        _btnControlTouching.onClick.AddListener(ButtonControlTouchingOnClick);
    }

    protected override void OnWindowEnable() {
        InitUI();
        Refresh();
    }

    protected override void OnWindowDestroy() {
        _sliderSpeed.onValueChanged.RemoveListener(SliderSpeedValueChanged);

        for (int i = 0; i < _btnNodeSoundPerfect.Length; i++) {
            _btnNodeSoundPerfect[i].onClick.RemoveAllListeners();
        }

        for (int i = 0; i < _btnNodeSoundNormal.Length; i++) {
            _btnNodeSoundNormal[i].onClick.RemoveAllListeners();
        }

        _btnNodeMovingRaising.onClick.RemoveListener(ButtonMovingRaisingOnClick);
        _btnNodeMovingFalling.onClick.RemoveListener(ButtonMovingFallingOnClick);

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
    private void ButtonSoundPerfectOnClick(int index) {
        GameDataManager.SaveInt(Define.GAME_DATA_KEY_SE_PERFECT, index);

        if (AudioManager.Instance != null) {
            AudioClip ac = Utility.GetSEPerfect();
            AudioManager.Instance.PlaySE(ac);
        }

        RefreshNodeSoundPerfect();
    }

    private void ButtonSoundNormalOnClick(int index) {
        GameDataManager.SaveInt(Define.GAME_DATA_KEY_SE_NORMAL, index);

        if (AudioManager.Instance != null) {
            AudioClip ac = Utility.GetSENormal();
            AudioManager.Instance.PlaySE(ac);
        }

        RefreshNodeSoundNormal();
    }

    private void ButtonMovingRaisingOnClick() {
        if (_curNodeMovingType == (int) NodeMovingType.Raising) {
            return;
        }

        GameDataManager.SaveInt(Define.GAME_DATA_KEY_NODE_MOVING_TYPE, (int) NodeMovingType.Raising);

        RefreshMovingSetting();
    }

    private void ButtonMovingFallingOnClick() {
        if (_curNodeMovingType == (int) NodeMovingType.Falling) {
            return;
        }

        GameDataManager.SaveInt(Define.GAME_DATA_KEY_NODE_MOVING_TYPE, (int) NodeMovingType.Falling);

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

        _curSpeedLevel = Utility.GetTrackSpeedLevel();
        _sliderSpeed.value = _curSpeedLevel;
    }

    private void Refresh() {
        RefreshSpeedSetting();
        RefreshNodeSoundPerfect();
        RefreshNodeSoundNormal();
        RefreshMovingSetting();
        RefreshControlSetting();
    }

    private void RefreshSpeedSetting() {
        _curSpeedLevel = Utility.GetTrackSpeedLevel();

        float speedValue = Utility.GetTrackSpeedValue();
        _textSpeedValue.text = string.Format("{0:0.0}", speedValue);
    }

    private void RefreshNodeSoundPerfect() {
        int seIndex = Utility.GetSEPerfectIndex();

        for (int i = 0; i < _btnNodeSoundPerfect.Length; i++) {
            _btnNodeSoundPerfect[i].image.sprite = i == seIndex ? _imageSelected.sprite : _imageUnselected.sprite;
        }
    }

    private void RefreshNodeSoundNormal() {
        int seIndex = Utility.GetSENormalIndex();

        for (int i = 0; i < _btnNodeSoundNormal.Length; i++) {
            _btnNodeSoundNormal[i].image.sprite = i == seIndex ? _imageSelected.sprite : _imageUnselected.sprite;
        }
    }

    private void RefreshMovingSetting() {
        _curNodeMovingType = Utility.GetNodeMovingType();

        _btnNodeMovingRaising.image.sprite = 
            _curNodeMovingType == (int) NodeMovingType.Raising ? _imageSelected.sprite : _imageUnselected.sprite;
        _btnNodeMovingFalling.image.sprite =
            _curNodeMovingType == (int) NodeMovingType.Falling ? _imageSelected.sprite : _imageUnselected.sprite;
    }

    private void RefreshControlSetting() {
        _curControlType = Utility.GetControlType();

        _btnControlKeyboard.image.sprite =
            _curControlType == (int) ControlType.Keyboard ? _imageSelected.sprite : _imageUnselected.sprite;
        _btnControlTouching.image.sprite =
            _curControlType == (int) ControlType.Touching ? _imageSelected.sprite : _imageUnselected.sprite;
    }
    #endregion
}
