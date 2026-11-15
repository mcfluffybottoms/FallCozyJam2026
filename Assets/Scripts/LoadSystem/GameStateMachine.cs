using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine
{
    private readonly Dictionary<GameState, IGameState> _states = new();
    private IGameState _current;

    public GameState CurrentStateId { get; private set; }
    public event Action<GameState> OnStateChanged;

    public void Register(GameState id, IGameState state) => _states[id] = state;

    public void ChangeState(GameState id)
    {
        if (!_states.TryGetValue(id, out var next))
        {
            Debug.LogError($"State {id} not registered");
            return;
        }
        if (_current == next) return;

        _current?.Exit();
        _current = next;
        CurrentStateId = id;
        OnStateChanged?.Invoke(id);
        _current.Enter();
    }

    public void Tick() => _current?.Tick();
}