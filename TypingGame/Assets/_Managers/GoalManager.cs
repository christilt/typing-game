using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : Singleton<GoalManager>
{
    private readonly HashSet<ICollectableGoal> _collectableGoals = new();

    public bool GoalsCompleted => _collectableGoals.Count == 0;

    public bool TryRegister(ICollectableGoal goal)
    {
        if (_collectableGoals.Contains(goal)) 
            return false;

        _collectableGoals.Add(goal);
        return true;
    }

    public bool Complete(ICollectableGoal goal)
    {
        Debug.Log($"Goal collected");

        var isUnregistered = TryUnregister(goal);
        if (!isUnregistered)
            return isUnregistered;

        MaybeSignalCompletion();
        return true;
    }

    private bool TryUnregister(ICollectableGoal goal)
    {
        if (!_collectableGoals.Contains(goal))
            return false;

        _collectableGoals.Remove(goal);
        return true;
    }

    private void MaybeSignalCompletion()
    {
        if (!GoalsCompleted) 
            return;

        GameplayManager.Instance.LevelWinning();
    }
}
