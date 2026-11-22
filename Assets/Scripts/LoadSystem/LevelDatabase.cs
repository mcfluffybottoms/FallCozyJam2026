using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Game/Level Database")]
public class LevelDatabase : ScriptableObject
{
    [System.Serializable]
    public class LevelEntry
    {
        public string DisplayName = "Level";
        public GameObject Prefab;
    }

    [SerializeField] private LevelEntry[] _levels;

    public int Count => _levels?.Length ?? 0;

    public LevelEntry Get(int index)
    {
        if (_levels == null || index < 0 || index >= _levels.Length) return null;
        return _levels[index];
    }
}