using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Seeker), typeof(Rigidbody2D))]
public class AstarTest : MonoBehaviour
{
    [SerializeField] private Transform _centre;
    [SerializeField] private float _moveSpeed = 1;
    [SerializeField] private float _waypointMarginDistance = 0.2f;
    [SerializeField] private float _repathRateTime;
    [SerializeField] private Tilemap _tilemap;

    private Seeker _seeker;
    private Path _path;
    private int _waypointIndex;
    private Rigidbody2D _rigidbody;
    private float _lastRepathTime;

    private Vector3 SourceCentre => _tilemap.GetCellCenterWorld(_tilemap.WorldToCell(_centre.position));
    private Vector3 TargetPosition => Player.Instance.Centre;


    private void Awake()
    {
        _seeker = GetComponent<Seeker>();
        _rigidbody = GetComponent<Rigidbody2D>();   
    }

    // See https://arongranberg.com/astar/documentation/dev_4_1_9_b355d2bd/custom_movement_script.php
    private void FixedUpdate()
    {
        if (_seeker.IsDone() && Time.time >  (_lastRepathTime + _repathRateTime))
        {
            _lastRepathTime = Time.time;
            _seeker.StartPath(SourceCentre, TargetPosition, path =>
            {
                _waypointIndex = 0;
                _path = path;
                Debug.Log($"Path updated to {Print(path)}");
            });
        }

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
            Debug.Log("End of path reached");
            _waypointIndex++;
            return;
        }

        var destinationPosition = _path.vectorPath[_waypointIndex];
        var direction = (destinationPosition - SourceCentre).normalized;
        var velocity = direction * _moveSpeed;
        var newPosition = transform.position + (Time.deltaTime * velocity);
        _rigidbody.MovePosition(newPosition);
        if (Vector2.Distance(SourceCentre, destinationPosition) < _waypointMarginDistance)
        {
            _waypointIndex++;
            return;
        }
    }

    private string Print(Path path) => string.Join("; ", path.vectorPath.Select(p => p.ToString()));
}
