using System.Collections.Generic;

public class EventManager : ISingleton<EventManager> {

    public delegate void EventCallback(BaseEventArgs arg);

    #region Internal Fields
    private Dictionary<EventTypes, List<EventCallback>> _eventCallbackMap = new Dictionary<EventTypes, List<EventCallback>>();
    #endregion

    #region APIs
    public void Register(EventTypes type, EventCallback cb) {
        if (type == EventTypes.NONE) {
            return;
        }

        if (!_eventCallbackMap.ContainsKey(type)) {
            _eventCallbackMap.Add(type, new List<EventCallback>());
        }

        _eventCallbackMap[type].Add(cb);
    }

    public void Unregister(EventTypes type, EventCallback cb) {
        if (type == EventTypes.NONE) {
            return;
        }

        if (!_eventCallbackMap.ContainsKey(type)) {
            return;
        }

        _eventCallbackMap[type].Remove(cb);
    }

    public void Dispatch(BaseEventArgs arg) {
        EventTypes type = arg.ID;
        if (type == EventTypes.NONE) {
            return;
        }

        if (!_eventCallbackMap.ContainsKey(type)) {
            return;
        }

        List<EventCallback> cbList = _eventCallbackMap[type];
        for (int i = 0; i < cbList.Count; i++) {
            cbList[i](arg);
        }
    }
    #endregion
}
