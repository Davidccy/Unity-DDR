using UnityEngine;

public class SceneCommon : SceneBase {
    #region Override Methods
    protected override void OnSceneAwake() {
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
}
