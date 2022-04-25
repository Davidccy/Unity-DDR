using System.Collections.Generic;

public class GameEventManager : ISingleton<GameEventManager> {

    public delegate void GameEventCallback(BaseGameEventArgs arg);

    #region Internal Fields
    private Dictionary<GameEventTypes, List<GameEventCallback>> _eventCallbackMap = new Dictionary<GameEventTypes, List<GameEventCallback>>();
    #endregion

    #region APIs
    public void Register(GameEventTypes type, GameEventCallback cb) {
        if (type == GameEventTypes.NONE) {
            return;
        }

        if (!_eventCallbackMap.ContainsKey(type)) {
            _eventCallbackMap.Add(type, new List<GameEventCallback>());
        }

        _eventCallbackMap[type].Add(cb);
    }

    public void Unregister(GameEventTypes type, GameEventCallback cb) {
        if (type == GameEventTypes.NONE) {
            return;
        }

        if (!_eventCallbackMap.ContainsKey(type)) {
            return;
        }

        _eventCallbackMap[type].Remove(cb);
    }

    public void Dispatch(BaseGameEventArgs arg) {
        GameEventTypes type = arg.ID;
        if (type == GameEventTypes.NONE) {
            return;
        }

        if (!_eventCallbackMap.ContainsKey(type)) {
            return;
        }

        List<GameEventCallback> cbList = _eventCallbackMap[type];
        for (int i = 0; i < cbList.Count; i++) {
            cbList[i](arg);
        }
    }
    #endregion
}
