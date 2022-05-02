using UnityEngine;

public class ISingleton<T> : MonoBehaviour where T : Component {
    #region Internal Fields
    private static T _instance;
    private static bool _isAppPlaying = true;
    #endregion

    #region Properties
    public static T Instance {
        get {
            if (!_isAppPlaying) {
                return null;
            }

            if (_instance == null) {
                T[] instances = FindObjectsOfType<T>();

                if (instances.Length > 1) {
                    Debug.LogErrorFormat("Multi ISingleton component {0} detected", typeof(T).ToString());
                }

                if (instances.Length == 1) {
                    _instance = instances[0];
                    DontDestroyOnLoad(instances[0]);
                    return instances[0];
                }
            }

            if (_instance == null) {
                GameObject newGo = new GameObject();
                _instance = newGo.AddComponent<T>();
                newGo.name = typeof(T).ToString();

                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        if (_instance == null) {
            _instance = this as T;

            DontDestroyOnLoad(_instance);

            return;
        }
        else {
            T instance = this as T;
            if (_instance != instance) {
                Debug.LogErrorFormat("Duplicate ISingleton component {0} detected", typeof(T).ToString());
                Destroy(instance);
            }
        }

        DoAwake();
    }

    private void OnDestroy() {
        T instance = this as T;
        if (_instance == instance) {
            _isAppPlaying = false;
        }        

        DoDestroy();
    }
    #endregion

    #region Virtual Methods
    protected virtual void DoAwake() { 

    }

    protected virtual void DoDestroy() {

    }
    #endregion
}
