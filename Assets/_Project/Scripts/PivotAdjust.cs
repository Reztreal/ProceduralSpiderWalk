using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class PivotAdjust : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Scan scan;
    [SerializeField, Range(0, 1)] float positionWeight = 0;
    [SerializeField, Range(0, 1)] float rotationWeight = 1;
    
    public Transform Pivot => pivot;
    
    void OnDisable()
    {
        pivot.localPosition = Vector3.zero;
        pivot.localRotation = Quaternion.identity;
    }

    private void Update()
    {
        Adjust();
    }

    private void Adjust()
    {
        List<(Vector3 pos, Quaternion rot, float weight)> points = scan.Points();

        Quaternion rotAvg;
        List<Quaternion> rots = new List<Quaternion>();
        List<float> weights = new List<float>();

        Vector3 posAvg = Vector3.zero;
        int nbPoint = 0;

        foreach ((Vector3 pos, Quaternion rot, float weight) point in points)
        {
            rots.Add(point.rot);
            weights.Add(point.weight);
            posAvg += point.pos;
            nbPoint++;
        }

        posAvg /= nbPoint;
        rotAvg = MathExtensions.QuaternionAverage(rots.ToArray(), weights.ToArray());

        pivot.position = Vector3   .Lerp(transform.position, posAvg, positionWeight);
        pivot.rotation = Quaternion.Lerp(transform.rotation, rotAvg, rotationWeight);
    }
}
