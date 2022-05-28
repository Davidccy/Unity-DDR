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

public class NodeResultGameEventArgs : BaseGameEventArgs {
    public NodeResult NR;
    public NodePosition NP;

    public NodeResultGameEventArgs() {
        ID = GameEventTypes.NODE_RESULT;
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

public class TrackAchievementGameEventArgs : BaseGameEventArgs {
    public bool IsAllPerfect {
        get;
        private set;
    }

    public bool IsFullCombo {
        get;
        private set;
    }

    public TrackAchievementGameEventArgs(bool isAllPerfect, bool isFullCombo) {
        ID = GameEventTypes.TRACK_ACHIEVEMENT;

        IsAllPerfect = isAllPerfect;
        IsFullCombo = isFullCombo;
    }
}

public class ScoreChangedGameEventArgs : BaseGameEventArgs {
    public int PreviousScore {
        get;
        private set;
    }

    public int CurrentScore {
        get;
        private set;
    }

    public ScoreChangedGameEventArgs(int preScore, int curScore) {
        ID = GameEventTypes.SCORE_CHANGED;

        PreviousScore = preScore;
        CurrentScore = curScore;
    }
}

public class ComboChangedGameEventArgs : BaseGameEventArgs {
    public int PreviousCombo {
        get;
        private set;
    }

    public int CurrentCombo {
        get;
        private set;
    }

    public ComboChangedGameEventArgs(int preCombo, int curCombo) {
        ID = GameEventTypes.COMBO_CHANGED;

        PreviousCombo = preCombo;
        CurrentCombo = preCombo;
    }
}
