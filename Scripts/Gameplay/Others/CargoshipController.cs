using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CargoshipController : MonoBehaviour
{
    [SerializeField] private Transform shipWaypoints_;

    [SerializeField] private float wayDuration_ = 60f;
    
    void Start ()
    {
        List<Vector3> waypoints = new List<Vector3> ();
        foreach (Transform waypoint in shipWaypoints_)
            waypoints.Add (waypoint.position);

        this.transform.DOPath (waypoints.ToArray(), wayDuration_, PathType.CatmullRom, PathMode.Full3D, 1000).SetLookAt (0.01f).SetLoops (-1);
    }
}
