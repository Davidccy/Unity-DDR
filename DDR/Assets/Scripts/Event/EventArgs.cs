using UnityEngine;

public class BaseEventArgs {
    public EventTypes ID {
        get;
        protected set;
    }

    public void Dispatch() {
        EventManager.Instance.Dispatch(this);
    }
}

public class TestEventArgs : BaseEventArgs {
    public int TestValue;

    public TestEventArgs() {
        ID = EventTypes.TEST;
    }
}

public class BumpEventArgs : BaseEventArgs {
    public BumpEventArgs() {
        ID = EventTypes.BUMP;
    }
}

public class NodePressedEventArgs : BaseEventArgs {
    public NodePosition NP;

    public NodePressedEventArgs() {
        ID = EventTypes.NODE_PRESSED;
    }
}

public class TapResultEventArgs : BaseEventArgs {
    public TapResult TR;
    public NodePosition NP;

    public TapResultEventArgs() {
        ID = EventTypes.TAP_RESULT;
    }
}
