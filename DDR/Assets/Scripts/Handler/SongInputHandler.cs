using UnityEngine;

public class SongInputHandler : MonoBehaviour {
    #region Internal Fields
    private bool _isActive = false;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        int controlType = GameDataManager.LoadInt(Define.GAME_DATA_KEY_CONTROL_TYPE);
        _isActive = controlType == (int) ControlType.Keyboard;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            OnNodePressed(NodePosition.Left);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            OnNodePressed(NodePosition.Up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            OnNodePressed(NodePosition.Down);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            OnNodePressed(NodePosition.Right);
        }
        else if (Input.GetKeyDown(KeyCode.Space)) {
            OnNodePressed(NodePosition.Space);
        }
    }
    #endregion

    #region Event Handlings
    private void OnNodePressed(NodePosition np) {
        if (!_isActive) {
            return;
        }

        NodePressedGameEventArgs args = new NodePressedGameEventArgs();
        args.NP = np;
        args.Dispatch();
    }
    #endregion
}
