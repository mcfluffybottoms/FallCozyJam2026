using UnityEngine;

public class TestLvlSwitch : MonoBehaviour
{
    [SerializeField] private KeyCode NextLvl;
    [SerializeField] private KeyCode PrevLvl;
    [SerializeField] private GameManager GameManager;

    private void Update()
    {
        if (GameManager == null) return;
        if (GameManager.CurrentState == GameState.Loading) return;

        if (Input.GetKeyDown(NextLvl))
            GameManager.NextLevelSafe();

        if (Input.GetKeyDown(PrevLvl))
        {
            int prev = GameManager.CurrentLevelIndex - 1;
            if (GameManager.CanGoTo(prev))
                GameManager.LoadLevel(prev);
        }
    }
}