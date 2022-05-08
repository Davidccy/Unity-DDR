using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Game Config Data", menuName = "GameConfigData", order = 1)]
public class GameConfigData : ScriptableObject {
    public AudioClip BGMMain;
    public AudioClip BGMResult;

    public AudioClip SoundEffectReady;
    public AudioClip[] SoundEffectPerfect;
    public AudioClip[] SoundEffectNormal;
}
