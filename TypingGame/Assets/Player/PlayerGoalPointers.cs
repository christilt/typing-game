using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGoalPointers : MonoBehaviour
{
    [SerializeField] private PlayerTypingMovement _typingMovement;
    [SerializeField] private PlayerGoalPointer _playerGoalPointerPrefab;
    [SerializeField] private GameObject _centre;

    [SerializeField] private float _radius;
    [SerializeField] private float _minDistance;

    private Dictionary<int, Collectable> _targetsById;
    private Dictionary<int, PlayerGoalPointer> _goalPointersByTargetId;
    private Boundary _boundaryFurthest;
    private Boundary _boundaryNewest;
    private HashSet<Boundary> _boundariesEntered;
    private HashSet<Boundary> _boundariesPassed;
    private Vector3 _centreOffset;

    private void OnEnable()
    {
        _centreOffset = _centre.transform.position - transform.position;
        _goalPointersByTargetId = new();
        _boundariesEntered = new();
        _boundariesPassed = new();
        _targetsById = GetTargetsById();
    }

    private void Update()
    {
        foreach (var (id, target) in _targetsById)
        {
            var direction = (CentreOf(target.transform.position) - _centre.transform.position).normalized;
            var position = _centre.transform.position + direction * _radius;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            if (!_goalPointersByTargetId.TryGetValue(id, out var existingTarget))
            {
                var goalPointer = PlayerGoalPointer.Instantiate(_playerGoalPointerPrefab, position, rotation, gameObject.transform, target.Color);
                _goalPointersByTargetId.Add(id, goalPointer);
            }
            else
            {
                existingTarget.gameObject.transform.position = position;
                existingTarget.gameObject.transform.rotation = rotation;
            }
        }
    }

    private void OnDisable()
    {
        foreach (var (id, goalPointer) in _goalPointersByTargetId)
        {
            goalPointer.BeDestroyed();
        }
    }

    // TODO: Do not add targets if they are close at moment of updating collection
    // TODO: Check performant
    // TODO: Refresh targets after some time (they can enter existing boundaries themselves)

    /*
     * OnTriggerEnter2D and OnTriggerExit2D code:
     * Unity docs state that OnTriggerEnter2D code is run first
     * We need to establish that player is "passing" a boundary
     * A boundary is considered "passed" when they exit it whilst
     * the furthest boundary they have entered is also their newest one.
     * To check this, we store the furthest and newest boundaries as part of OnTriggerEnter2D code,
     * then look at these when the player next exits a boundary.
     * (Player can overlap boundaries so may not enter and exit boundaries at the same time).
     */

    // OnTriggerEnter2D is called first, then OnTriggerExit2D
    // Note this can get called when unpausing
    public void IncludeEnteredBoundary(Boundary boundary)
    {
        if (!enabled) return;

        if (!_boundariesEntered.Contains(boundary))
        {
            _boundaryFurthest = boundary;
            _boundariesEntered.Add(boundary);
        }

        _boundaryNewest = boundary;
    }

    // OnTriggerEnter2D is called first, then OnTriggerExit2D
    public void IncludeExitedBoundary(Boundary boundary)
    {
        if (!enabled) return;

        var isPassingThisBoundary = _boundaryNewest == _boundaryFurthest;
        if (isPassingThisBoundary)
        {

            _boundariesPassed.Add(boundary);
            UpdateTargetCollection();
        }
    }

    // TODO: Allow player to only remove objects from the target collection, not necessarily add them
    public void UpdateTargetCollection()
    {
        _targetsById = GetTargetsById();

        var toRemove = _goalPointersByTargetId.Keys.Except(_targetsById.Keys).ToArray();

        foreach (var id in toRemove)
        {
            var goalPointer = _goalPointersByTargetId[id];
            goalPointer.BeDestroyed();
            _goalPointersByTargetId.Remove(id);
        }
    }

    private Vector3 CentreOf(Vector3 position) => position + _centreOffset;

    private Dictionary<int, Collectable> GetTargetsById()
    {
        var scene = SceneManager.GetSceneByName(LevelSettingsManager.Instance.LevelSettings.SceneName);
        return scene.GetRootGameObjects()
            .SelectMany(o => 
                o.GetComponentsInChildren<Collectable>()
                 .Where(c => c.Effects.OfType<CollectableIsGoal>()
                                      .Any(c => !c.IsCompleted)))
            .Where(c => _boundariesPassed.Any(b => b.Collider.bounds.Contains(c.transform.position)))
            .ToDictionary(x => x.transform.GetInstanceID(), x => x);
    }
}