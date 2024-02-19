using Pathfinding;
using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class UnitBrain : MonoBehaviour
{
    [SerializeField] private Transform _centre;
    [SerializeField] private float _waypointMarginDistance = 0.2f;
    [SerializeField] private UnitBrainMode _mode;
    [SerializeField] private Collider2D _fieldOfVisionCollider;

    private Seeker _seeker;
    private Path _path;
    private int _waypointIndex;
    private Vector3 _pathTargetPosition;
    private Vector2? _targetDirection;
    private bool _playerWithinFieldOfVision;
    private UnitBrainMode _currentMode;

    private Vector3 SourceCentre => _centre.position;
    private Vector3 TargetPosition => Player.Instance.Centre;

    private void Awake()
    {
        _seeker = GetComponent<Seeker>();
        _currentMode = _mode;
    }

    private void OnEnable()
    {
        _fieldOfVisionCollider.enabled = true;
    }

    private void OnDisable()
    {
        _fieldOfVisionCollider.enabled = false;
    }

    public Vector2Int ChooseDirection(Vector2Int[] directionOptions)
    {
        if (!directionOptions.Any())
            throw new InvalidOperationException("Must be at least one direction option");

        if (_targetDirection == null)
        {
            //Debug.Log("Target direction not yet chosen - choosing random direction");
            return ChooseDirection_RandomMode(directionOptions);
        }

        switch (_currentMode)
        {
            case UnitBrainMode.Random:
                return ChooseDirection_RandomMode(directionOptions);
            case UnitBrainMode.ChasePlayer:
                return ChooseDirection_ChaseMode(directionOptions);
            case UnitBrainMode.EvadePlayer:
                return ChooseDirection_EvadeMode(directionOptions);
            default:
                //Debug.LogError($"Mode {_currentMode} not implemented - choosing random direction");
                return ChooseDirection_RandomMode(directionOptions);
        }
    }

    public void MaybeEvadePlayer()
    {
        if (!_mode.IsIntelligent())
            return;

        _currentMode = UnitBrainMode.EvadePlayer;
    }

    public void SetInitialMode()
    {
        _currentMode = _mode;
    }


    private Vector2Int ChooseDirection_ChaseMode(Vector2Int[] directionOptions)
    {
        var directionClosestToTarget = directionOptions
            .OrderBy(d => Vector2.Angle(d, _targetDirection.Value))
            .First();

        //Debug.Log($"Direction {directionClosestToTarget} is closest to target of {_targetDirection} - choosing as chase direction");
        return directionClosestToTarget;
    }

    private Vector2Int ChooseDirection_EvadeMode(Vector2Int[] directionOptions)
    {
        var directionFurthestFromTarget = directionOptions
            .OrderByDescending(d => Vector2.Angle(d, _targetDirection.Value))
            .First();

        //Debug.Log($"Direction {directionFurthestFromTarget} is furthest from target of {_targetDirection} - choosing as evade direction");
        return directionFurthestFromTarget;
    }

    private Vector2Int ChooseDirection_RandomMode(Vector2Int[] directionOptions)
    {
        return directionOptions[UnityEngine.Random.Range(0, directionOptions.Length)];
    }

    // See https://arongranberg.com/astar/documentation/dev_4_1_9_b355d2bd/custom_movement_script.php
    private void Update()
    {
        if (!_playerWithinFieldOfVision)
        {
            return;
        }

        if (_currentMode == UnitBrainMode.Random)
        {
            return;
        }

        MaybeRepath();

        if (_path == null)
            return;

        if (_path.error)
        {
            Debug.LogError(_path.errorLog);
            return;
        }

        if (_waypointIndex > _path.vectorPath.Count)
            return;

        if (_waypointIndex == _path.vectorPath.Count)
        {
            _waypointIndex++;
            return;
        }

        var destinationPosition = _path.vectorPath[_waypointIndex];
        _targetDirection = (destinationPosition - _centre.position).normalized;
        if (Vector2.Distance(_centre.position, destinationPosition) < _waypointMarginDistance)
        {
            if (MaybeRepath())
            {
                return;
            }

            _waypointIndex++;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Player>(out _))
        {
            _playerWithinFieldOfVision = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetRigidbodyComponent<Player>(out _))
        {
            _playerWithinFieldOfVision = false;
            ResetPathAndTargets();
        }
    }

    private bool MaybeRepath()
    {
        if (_seeker.IsDone() && TargetPosition != _pathTargetPosition)
        {
            Repath();
            return true;
        }

        return false;
    }

    private void Repath()
    {
        _pathTargetPosition = TargetPosition;
        _seeker.StartPath(SourceCentre, _pathTargetPosition, path =>
        {
            _waypointIndex = 0;
            _path = path;
            //Debug.Log($"Path updated to {Print(path)}");
        });
    }

    private void ResetPathAndTargets()
    {
        _pathTargetPosition = default;
        _targetDirection = null;
        _waypointIndex = 0;
        _path = null;
    }

    private string Print(Path path) => string.Join("; ", path.vectorPath.Select(p => p.ToString()));

}

public enum UnitBrainMode
{
    Random,
    ChasePlayer,
    EvadePlayer
}

public static class ModeExtensions
{
    public static bool IsIntelligent(this UnitBrainMode mode)
    {
        return mode != UnitBrainMode.Random;
    }
}
