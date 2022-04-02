using UnityEngine;

public class SceneBase : MonoBehaviour {
    #region Mono Behaviour Hooks
    private void Awake() {
        OnSceneAwake();
    }

    private void OnEnable() {
        OnSceneEnable();
    }

    private void OnDisable() {
        OnSceneDisable();
    }

    private void OnDestroy() {
        OnSceneDestroy();
    }
    #endregion

    #region Virtual Methods
    protected virtual void OnSceneAwake() { 

    }

    protected virtual void OnSceneEnable() {

    }

    protected virtual void OnSceneDisable() {

    }

    protected virtual void OnSceneDestroy() {

    }
    #endregion
}
