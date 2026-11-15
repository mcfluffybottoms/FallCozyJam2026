using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private LevelDatabase _database;

    [Header("Scene refs")]
    [SerializeField] private LevelLoader _levelLoader;

    [Header("Settings")]
    [SerializeField] private float _minLoadingTime = 0.6f;

    private GameStateMachine _stateMachine;
    private int _currentLevelIndex = -1;
    private int _pendingLevelIndex = -1;

    public int CurrentLevelIndex => _currentLevelIndex;
    public GameState CurrentState => _stateMachine.CurrentStateId;

    public int LevelCount => _database != null ? _database.Count : 0;

    public bool CanGoTo(int index) =>
        _database != null && index >= 0 && index < _database.Count;

    public bool CanGoNext() => CanGoTo(_currentLevelIndex + 1);
    public bool CanGoPrev() => CanGoTo(_currentLevelIndex - 1);

    public event Action<int, GameObject> OnLevelLoaded;
    public event Action<int> OnLevelStarted;
    public event Action<int> OnLevelEnded;
    public event Action OnAllLevelsCompleted;
    public event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (_levelLoader == null) _levelLoader = GetComponentInChildren<LevelLoader>();

        BuildStateMachine();
    }

    private void Start() => _stateMachine.ChangeState(GameState.Boot);
    private void Update() => _stateMachine.Tick();
    private void OnDestroy() { if (Instance == this) Instance = null; }

    private void BuildStateMachine()
    {
        _stateMachine = new GameStateMachine();
        _stateMachine.Register(GameState.Boot, new BootState(this));
        _stateMachine.Register(GameState.Loading, new LoadingState(this));
        _stateMachine.Register(GameState.Gameplay, new GameplayState(this));
        _stateMachine.Register(GameState.LevelEnd, new LevelEndState(this));
        _stateMachine.OnStateChanged += s =>
        {
            OnStateChanged?.Invoke(s);
        };
    }

    public void NotifyLevelCompleted()
    {
        if (_stateMachine.CurrentStateId != GameState.Gameplay) return;
        _stateMachine.ChangeState(GameState.LevelEnd);
    }

    public void RestartLevel()
    {
        if (_stateMachine.CurrentStateId == GameState.Loading) return;
        if (!CanGoTo(_currentLevelIndex)) return;
        _pendingLevelIndex = _currentLevelIndex;
        _stateMachine.ChangeState(GameState.Loading);
    }

    public void LoadLevel(int index)
    {
        if (!CanGoTo(index)) return;
        _pendingLevelIndex = index;
        _stateMachine.ChangeState(GameState.Loading);
    }

    public void NextLevel()
    {
        _pendingLevelIndex = _currentLevelIndex + 1;
        _stateMachine.ChangeState(GameState.Loading);
    }

    public void NextLevelSafe()
    {
        if (!CanGoNext()) return;
        NextLevel();
    }

    internal IEnumerator LoadLevelRoutine(int index)
    {
        if (!CanGoTo(index))
        {
            Debug.LogError($"LoadLevelRoutine: invalid index {index}");
            yield break;
        }

        _currentLevelIndex = index;

        var entry = _database.Get(index);
        if (entry == null || entry.Prefab == null)
        {
            Debug.LogError($"No level at index {index}");
            yield break;
        }

        _levelLoader.Unload();

        float startTime = Time.realtimeSinceStartup;

        var instance = _levelLoader.Load(entry.Prefab);
        yield return null;

        float elapsed = Time.realtimeSinceStartup - startTime;
        float wait = Mathf.Max(0f, _minLoadingTime - elapsed);
        if (wait > 0f) yield return new WaitForSecondsRealtime(wait);

        OnLevelLoaded?.Invoke(index, instance);

        _stateMachine.ChangeState(GameState.Gameplay);
    }

    internal bool HasNextLevel() => _database != null && _currentLevelIndex + 1 < _database.Count;

    internal int NextLevelIndex() => _currentLevelIndex + 1;
    internal int CurrentLevelIndexInternal() => _currentLevelIndex;

    internal void FireLevelEnded(int index) => OnLevelEnded?.Invoke(index);
    internal void FireLevelStarted(int index) => OnLevelStarted?.Invoke(index);
    internal void FireAllLevelsCompleted() => OnAllLevelsCompleted?.Invoke();

    private class BootState : IGameState
    {
        private readonly GameManager _gm;
        public BootState(GameManager gm) => _gm = gm;

        public void Enter()
        {
            _gm._stateMachine.ChangeState(GameState.Loading);
        }
        public void Exit() { }
        public void Tick() { }
    }

    private class LoadingState : IGameState
    {
        private readonly GameManager _gm;
        public LoadingState(GameManager gm) => _gm = gm;

        public void Enter()
        {
            int target = _gm._pendingLevelIndex;

            if (target < 0)
                target = _gm._currentLevelIndex + 1;

            _gm._pendingLevelIndex = -1;

            if (!_gm.CanGoTo(target))
            {
                _gm.FireAllLevelsCompleted();
                return;
            }

            _gm.StartCoroutine(_gm.LoadLevelRoutine(target));
        }

        public void Exit() { }
        public void Tick() { }
    }

    private class GameplayState : IGameState
    {
        private readonly GameManager _gm;
        public GameplayState(GameManager gm) => _gm = gm;

        public void Enter() => _gm.FireLevelStarted(_gm.CurrentLevelIndexInternal());
        public void Exit() { }
        public void Tick() { }
    }

    private class LevelEndState : IGameState
    {
        private readonly GameManager _gm;
        public LevelEndState(GameManager gm) => _gm = gm;

        public void Enter()
        {
            _gm.FireLevelEnded(_gm.CurrentLevelIndexInternal());
            _gm.StartCoroutine(NextRoutine());
        }

        public void Exit() { }
        public void Tick() { }

        private IEnumerator NextRoutine()
        {
            yield return new WaitForSecondsRealtime(0.3f);
            _gm._stateMachine.ChangeState(GameState.Loading);
        }
    }
}