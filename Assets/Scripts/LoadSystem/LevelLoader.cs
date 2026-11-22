using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private GameObject _instance;

    public GameObject CurrentLevelInstance => _instance;

    public GameObject Load(GameObject prefab)
    {
        Unload();
        if (prefab == null)
        {
            Debug.LogError("Level prefab is null");
            return null;
        }
        _instance = Instantiate(prefab);
        return _instance;
    }

    public void Unload()
    {
        if (_instance != null)
        {
            Destroy(_instance);
            _instance = null;
        }
    }
}