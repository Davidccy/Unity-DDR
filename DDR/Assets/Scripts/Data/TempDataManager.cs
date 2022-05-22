using System.Collections.Generic;

public static class TempDataManager {
    #region Internal Fields
    private static Dictionary<string, object> _dataTable = new Dictionary<string, object>();
    #endregion

    #region APIs
    public static void SaveData(string key, object obj) {
        _dataTable[key] = obj;
    }

    public static T LoadData<T>(string key) {
        if (!_dataTable.ContainsKey(key)) {
            return default(T);
        }

        return (T) _dataTable[key];
    }

    public static bool HasData(string key) {
        return _dataTable.ContainsKey(key);
    }
    #endregion
}
