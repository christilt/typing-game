using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGoalPointers : MonoBehaviour
{
    [SerializeField] private PlayerGoalPointer _playerGoalPointerPrefab;
    [SerializeField] private GameObject _centre;

    [SerializeField] private float _radius;
    [SerializeField] private float _minDistance;
    [SerializeField] private float _filterTargetsEverySeconds;

    private Dictionary<int, Collectable> _targetsById;
    private Dictionary<int, PlayerGoalPointer> _goalPointersByTargetId;
    private Vector3 _centreOffset;
    private float _lastTargetFilterSecondsAgo;

    private void Start()
    {
        _centreOffset = _centre.transform.position - transform.position;
        _goalPointersByTargetId = new();
        _targetsById = GetTargetsById();
    }

    private void Update()
    {
        _lastTargetFilterSecondsAgo += Time.deltaTime;

        if (_lastTargetFilterSecondsAgo >= _filterTargetsEverySeconds)
        {
            // Update goal pointer objects based on targets
            _targetsById = GetTargetsById();

            var toRemove = _goalPointersByTargetId.Keys.Except(_targetsById.Keys).ToList();

            foreach (var id in toRemove)
            {
                var goalPointer = _goalPointersByTargetId[id];
                goalPointer.BeDestroyed();
                _goalPointersByTargetId.Remove(id);
            }

            _lastTargetFilterSecondsAgo = 0;
        }

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

    private Vector3 CentreOf(Vector3 position) => position + _centreOffset;

    private Dictionary<int, Collectable> GetTargetsById()
    {
        var scene = SceneManager.GetSceneByName(LevelSettingsManager.Instance.LevelSettings.SceneName);
        return scene.GetRootGameObjects()
            .SelectMany(o => 
                o.GetComponentsInChildren<Collectable>()
                 .Where(c => c.Effects.OfType<CollectableIsGoal>()
                                      .Any(c => !c.IsCompleted)))
            .ToDictionary(x => x.transform.GetInstanceID(), x => x);
    }
}