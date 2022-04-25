using UnityEngine;

public class BaseGameEventArgs {
    public GameEventTypes ID {
        get;
        protected set;
    }

    public void Dispatch() {
        GameEventManager.Instance.Dispatch(this);
    }
}

public class TestGameEventArgs : BaseGameEventArgs {
    public int TestValue;

    public TestGameEventArgs() {
        ID = GameEventTypes.TEST;
    }
}

public class BumpGameEventArgs : BaseGameEventArgs {
    public BumpGameEventArgs() {
        ID = GameEventTypes.BUMP;
    }
}

public class NodePressedGameEventArgs : BaseGameEventArgs {
    public NodePosition NP;

    public NodePressedGameEventArgs() {
        ID = GameEventTypes.NODE_PRESSED;
    }
}

public class TapResultGameEventArgs : BaseGameEventArgs {
    public TapResult TR;
    public NodePosition NP;

    public TapResultGameEventArgs() {
        ID = GameEventTypes.TAP_RESULT;
    }
}

public class NodeGeneratedGameEventArgs : BaseGameEventArgs {
    public NodeGeneratedGameEventArgs() {
        ID = GameEventTypes.NODE_GENERATED;
    }
}

public class FinalNodeFinishedGameEventArgs : BaseGameEventArgs {
    public FinalNodeFinishedGameEventArgs() {
        ID = GameEventTypes.FINAL_NODE_FINISHED;
    }
}

public class TrackLoadedGameEventArgs : BaseGameEventArgs {
    public TrackLoadedGameEventArgs() {
        ID = GameEventTypes.TRACK_LOADED;
    }
}
