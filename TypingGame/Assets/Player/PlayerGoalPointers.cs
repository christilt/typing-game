using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGoalPointers : MonoBehaviour
{
    [SerializeField] private GameObject _playerGoalPointerPrefab;
    [SerializeField] private float _radius;
    [SerializeField] private float _minDistance;
    [SerializeField] private GameObject _centre;

    private Transform _target;
    private GameObject _goalPointer;

    private void Start()
    {
        _target = GetRandomTarget();
    }

    private void Update()
    {
        if (_target == null)
        {
            if (_goalPointer != null)
                Destroy(_goalPointer);

            return;
        }
        // TODO: Remove if nearer than min distance

        var direction = (_target.position - _centre.transform.position).normalized;
        var position = _centre.transform.position + direction * _radius;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        if (_goalPointer == null)
        {
            _goalPointer = Instantiate(_playerGoalPointerPrefab, position, rotation, gameObject.transform);
        }
        else
        {
            _goalPointer.gameObject.transform.position = position;
            _goalPointer.gameObject.transform.rotation = rotation;
        }
        
    }

    private Transform GetRandomTarget()
    {
        var scene = SceneManager.GetSceneByName(LevelSettingsManager.Instance.LevelSettings.SceneName);
        return scene.GetRootGameObjects()
            .SelectMany(o => o.GetComponentsInChildren<CollectableIsGoal>())
            .FirstOrDefault()
            ?.transform;
    }
}