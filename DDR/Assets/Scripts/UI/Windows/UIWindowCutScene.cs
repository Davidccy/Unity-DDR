using UnityEngine;
using UnityEngine.UI;

public class UIWindowCutScene : UIGenericWindow {
    #region Serialized Fields
    [Header("Sub Content")]
    [SerializeField] private Image _image = null;
    #endregion

    #region Exposed Fields
    public override string WindowName => Define.WIDNOW_CUT_SCENE;
    #endregion

    #region APIs
    public void SetColor(Color c) {
        _image.color = c;
    }
    #endregion
}
