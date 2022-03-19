﻿using UnityEngine;

public static class Utility {
    #region Keys
    public static string PLAYER_PREF_TRACK_ID = "PlayerPref_Track_ID";
    #endregion

    #region Tap Related Handlings     
    public static string GetTapResultText(TapResult tr) {
        string text = string.Empty;

        if (tr == TapResult.Miss) {
            text = "Miss ..";
        }
        else if (tr == TapResult.Good) {
            text = "Good";
        }
        else if (tr == TapResult.Great) {
            text = "Great !";
        }
        else if (tr == TapResult.Perfect) {
            text = "PERFECT !!!";
        }

        return text;
    }

    public static Color GetTapResultColor(TapResult tr) {
        Color c = Color.white;

        if (tr == TapResult.Miss) {
            c = Color.gray;
        }
        else if (tr == TapResult.Good) {
            c = Color.green;
        }
        else if (tr == TapResult.Great) {
            c = Color.red;
        }
        else if (tr == TapResult.Perfect) {
            c = Color.yellow;
        }

        return c;
    }
    #endregion

    #region Track Handlings
    public static TrackData GetTrackData(int trackID) {
        TrackData td = Resources.Load<TrackData>(string.Format("TrackData/TrackData_{0:000}", trackID));
        return td;
    }
    #endregion

    #region Audio Handlings
    public static AudioClip GetTrack(int trackID) {
        AudioClip ac = Resources.Load<AudioClip>(string.Format("Audio/Track/Track_{0:000}", trackID));
        return ac;
    }

    public static AudioClip GetSEReady() {
        AudioClip ac = Resources.Load<AudioClip>("Audio/SE/SoundEffect_001");
        return ac;
    }

    public static AudioClip GetSEPerfect() {
        AudioClip ac = Resources.Load<AudioClip>("Audio/SE/SoundEffect_001");
        return ac;
    }

    public static AudioClip GetSENormal() {
        AudioClip ac = Resources.Load<AudioClip>("Audio/SE/SoundEffect_002");
        return ac;
    }
    #endregion
}