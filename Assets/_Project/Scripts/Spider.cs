using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spider : MonoBehaviour
{
    private Player3D player3D;

    [SerializeField] private Transform[] targetGroup1;
    [SerializeField] private Transform[] orbits, targets, legs;

    [SerializeField] private float[] orbitLegDistance; 

    [SerializeField, Range(0, 360)] private float arcRange = 270;
    [SerializeField] private int arcResolution = 4;
    [SerializeField] private float arcRadius = 0.3f;
    [SerializeField] private LayerMask arcLayer;
    
    [SerializeField] private float stepTime = 0.25f;
    [SerializeField] private float stepHeight = 0.6f;
    [SerializeField] private AnimationCurve stepHeightCurve;
    [SerializeField, Range(0, 1)] float stepDelayRand = 0f;
    [SerializeField, Range(0, 1)] float desyncG2 = 1;

    private void OnValidate()
    {
        Cache();
    }

    private void OnEnable()
    {
        StartCoroutine(StepLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < orbits.Length; i++)
        {
            UpdateOrbitLegDistance(orbits[i], orbits[i].parent, legs[i], orbitLegDistance[i], true);
        }
        
        EndStepGizmos();
    }

    private void Cache()
    {
        player3D = GetComponent<Player3D>();
        orbitLegDistance = new float[orbits.Length];
        
        for (int i = 0; i < orbits.Length; i++)
        {
            orbitLegDistance[i] = (orbits[i].transform.position - legs[i].transform.position).magnitude;
        }
    }

    private void Update()
    {
        for (int i = 0; i < orbits.Length; i++)
        {
            UpdateOrbitLegDistance(orbits[i], orbits[i].parent, legs[i], orbitLegDistance[i]);
            CalculateStepEndPoint(orbits[i]);
        }
    }

    private void UpdateOrbitLegDistance(Transform orbit, Transform orbitParent, Transform leg, float orbitLegDist, bool gizmos= false)
    {
        Vector3 pos = orbitParent.position;
        Quaternion rot = orbitParent.rotation;
        float dist = (pos - leg.position).magnitude;

        Quaternion relativeRotation = Quaternion.Inverse(transform.rotation) * rot;

        bool checkSup = dist < orbitLegDist;

        for (int i = 0; i < 50 && dist < orbitLegDist * 1.5f; i++)
        {
            if (PhysicsExtensions.ArcCast(pos, rot, arcRange, arcRadius, arcResolution, arcLayer, out RaycastHit hit))
            {
                Vector3 nextPos = hit.point;
                Quaternion nextRot = MathExtensions.RotationMatchUp(rot, hit.normal);
                float nextDist = (nextPos - leg.position).magnitude;
                
                if (gizmos)
                {
                    Gizmos.color = new Color(0.9f, 0.5f, 0.5f, 0.3f);
                    Gizmos.DrawSphere(pos, 0.05f);
                }

                if (checkSup == nextDist > orbitLegDist)
                {
                    float progress = Mathf.InverseLerp(dist, nextDist, orbitLegDist);

                    if (gizmos)
                    {
                        Gizmos.color = new Color(0.9f, 0.5f, 0.5f, 1);
                        Gizmos.DrawSphere(Vector3.Lerp(pos, nextPos, progress), 0.1f);
                    }

                    else
                    {
                        orbit.position = Vector3.Lerp(pos, nextPos, progress);
                        orbit.rotation = Quaternion.Lerp(rot * Quaternion.Inverse(relativeRotation),
                            nextRot * Quaternion.Inverse(relativeRotation), progress);
                    }

                    checkSup = !checkSup;
                }

                pos = nextPos;
                rot = nextRot;
                dist = nextDist;
            }
        }
    }

    public void EndStepGizmos()
    {
        for (int i = 0; i < orbits.Length; i++)
        {
            CalculateStepEndPoint(orbits[i], true);
        }
    }


    private (Vector3, Quaternion) CalculateStepEndPoint(Transform orbit, bool gizmos = false)
    {
        if (player3D.Velocity == Vector2.zero)
        {
            return (orbit.position, orbit.rotation);
        }
        
        Vector3 playerVelocity = orbit.TransformVector(player3D.Velocity3);

        if (playerVelocity == Vector3.zero || playerVelocity == orbit.up)
        {
            return (orbit.position, orbit.rotation);
        }
        
        Vector3 pos = orbit.position;
        Quaternion rot = Quaternion.LookRotation(playerVelocity, orbit.up);

        float distance = player3D.Speed * stepTime / 2;
        
        if (gizmos)
            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);

        for (int i = 0; i < 100; i++)
        {
            if (PhysicsExtensions.ArcCast(pos, rot, arcRange, arcRadius, arcResolution, arcLayer, out RaycastHit hit))
            {
                if (gizmos)
                    Gizmos.DrawSphere(pos, 0.05f);

                float currentDistance = (hit.point - pos).magnitude;

                if (currentDistance >= distance)
                {
                    Vector3 nextPos = hit.point;
                    Quaternion nextRot = MathExtensions.RotationMatchUp(rot, hit.normal);

                    float progress = Mathf.InverseLerp(0, currentDistance, distance);

                    pos = Vector3.Lerp(pos, nextPos, progress);
                    rot = Quaternion.Lerp(rot, nextRot, progress);

                    if (gizmos)
                    {
                        Gizmos.color = new Color(1f, 0f, 0f, 1f);
                        Gizmos.DrawSphere(pos, 0.1f);
                    }

                    break;
                }

                distance -= currentDistance;

                pos = hit.point;
                rot.MatchUp(hit.normal);
            }
            
            else break;
        }

        return (pos, rot);
    }

    private IEnumerator StepLoop()
    {
        bool G1 = true;

        while (true)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targetGroup1.Contains(targets[i]) == G1)
                {
                    StartCoroutine(DelayStep(i, stepTime * stepDelayRand * Random.value));
                }
            }

            yield return new WaitForSeconds(stepTime * (G1 ? desyncG2 : 2 - desyncG2));

            G1 = !G1;
        }
    }

    private IEnumerator DelayStep(int index, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        StartCoroutine(Step(index));
    }

    private IEnumerator Step(int idx)
    {
        float t = 0, progress;

        Transform target = targets[idx];
        Transform orbit = orbits[idx];
        Transform leg = legs[idx];

        Vector3    targetStartPosProj = leg.InverseTransformPoint(target.position);
        Quaternion targetStartRotProj = Quaternion.Inverse(leg.rotation) * target.rotation;

        while (t < stepTime)
        {
            t += Time.deltaTime;
            progress = Mathf.Clamp01(t / stepTime);

            Vector3    startPos, endPos;
            Quaternion startRot, endRot;
            startPos = leg.TransformPoint(targetStartPosProj);
            startRot = leg.rotation * targetStartRotProj;
            (endPos, endRot) = CalculateStepEndPoint(orbit);

            target.position = Vector3.Lerp(startPos, endPos, progress);
            target.position += stepHeight * player3D.SpeedProgress * stepHeightCurve.Evaluate(progress) * leg.up;

            target.rotation = Quaternion.Lerp(startRot, endRot, progress);

            if (t < stepTime)
                yield return new WaitForEndOfFrame();
        }
    }
}
