﻿using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ghosting))]
public class LineDrawer: MonoBehaviour
{
    /// <summary>
    /// Distance from furthest point in tiles.
    /// </summary>
    public int distanceTile;
    public int distanceReal;
    public int numInstances;
    public int grid = 2;
    public Vector3 inTilePositon;
    public Vector3 debugPosition;
    public Vector3 debugEnd;
    Vector3[] _ghosts;
    Ghosting _ghoster;
    GameObject _model;

    public Vector3 begin
    {
        get { return _begin; }
        set { _begin = value; }
        
    }
    public Vector3 end
    {
        get { return _end; }
        set { _end = value; }

    }

    public GameObject model
    {
        get { return _model; }
        set { _model = value; }
    }

    Vector3 _begin;
    Vector3 _end;
    Quaternion _direction;
    public Vector3 _diff;

    public void DrawLine()
    {
        Debug.DrawLine(begin, end);

    }
    private void Start()
    {
        _ghoster = GetComponent<Ghosting>();
    }

    private void Update()
    {
        _diff = _end - _begin;
        int furthestAxis = Mathf.RoundToInt(Mathf.Max(Mathf.Abs(_diff.x), Mathf.Abs(_diff.z)));
        distanceTile = furthestAxis / grid;
        distanceReal = furthestAxis;
        debugPosition = _begin + _SnapToCardinal(_end, distanceReal);

        _SetInstances(_begin, debugPosition);
        numInstances = _ghosts.Length;
        _direction = _GetDirection(_begin, debugPosition);
        _ghoster.Ghost(_model, _direction, _ghosts);
        
        print(_end);

    }
    

    private void OnDrawGizmosSelected()
    {
        if (_ghosts == null)
        {
            return;
        }
        Gizmos.DrawCube(begin, Vector3.one);
        Gizmos.DrawCube(end, Vector3.one);
        Gizmos.DrawLine(begin, end);
        
        for (int i = 0; i < _ghosts.Length; i++)
        {
            Gizmos.DrawSphere(_ghosts[i], 0.3f);
        }
    }

    private void _SetInstances(Vector3 begin, Vector3 end)
    {
        if (SharedLibrary.VectorLocationEqual(begin, end))
        {
            _ghosts = new Vector3[1];
            _ghosts[0] = begin;
            return;
        }
        int dist = distanceTile + 1;
        _ghosts = new Vector3[dist];

        for (int tile = 0; tile < dist; tile++)
        {
            float tilePosF = (float)tile / (float)distanceTile;
            Vector3 tilePos = Vector3.Lerp(begin, end, tilePosF);
            _ghosts[tile] = tilePos;

            
        }
    }

    private Quaternion _GetDirection( Vector3 begin, Vector3 end)
    {
        var result = new Quaternion();
        if (SharedLibrary.VectorLocationEqual(begin, end))
        {
            var cardinal = SharedLibrary.CardinalDirection(inTilePositon);
            result.SetLookRotation(cardinal);
            return result;
        }
        else
        {
            result.SetLookRotation(_diff);
            return result;
        }
        
        
    }

    private Vector3 _SnapToCardinal(Vector3 vector, int distance)
    {
        Vector3 cardinal = SharedLibrary.CardinalDirection(vector);
        return cardinal * distance;
    }
}
