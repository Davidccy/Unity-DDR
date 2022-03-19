using UnityEngine;

public class SongInputManager : ISingleton<SongInputManager> {
    #region Mono Behaviour Hooks
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
        NodePressedEventArgs args = new NodePressedEventArgs();
        args.NP = np;
        args.Dispatch();
    }
    #endregion
}
