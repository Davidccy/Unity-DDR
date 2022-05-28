﻿using UnityEngine;

public enum GameEventTypes {
    NONE,
    TEST,

    // Game playing
    NODE_PRESSED,
    NODE_GENERATED,

    BUMP,
    NODE_RESULT,
    FINAL_NODE_FINISHED,
    TRACK_LOADED,
    TRACK_ACHIEVEMENT,

    SCORE_CHANGED,
    COMBO_CHANGED,
}
