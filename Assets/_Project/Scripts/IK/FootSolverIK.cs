using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class FootSolverIK : MonoBehaviour
{
    private Vector3 _rayPoint;

    private RaycastHit _hit;
    private RaycastHit _newHit;

    [Header("Transformations")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform relativeBodyPos;
    [SerializeField] private Transform body;

    [SerializeField] private Vector3 offset;
    private Quaternion tiltedAngle;

    [Header("Variables")] 
    private float stepSize = 0.7f;
    private float stepHeight = 1f;
    private float stepDuration = 0.2f;

    private bool _isLerping = false;
    private float _lerpTime = 0f;

    private Vector3 _oldPos;
    private Vector3 _newPos;
    private Vector3 _targetPos;

    private void Start()
    {
        _oldPos = _newPos = transform.position;
        
    }

    private void Update()
    {
        _rayPoint = _oldPos + Vector3.up;

        Debug.DrawLine(_rayPoint, _rayPoint + -_hit.normal, Color.red);
        Debug.DrawLine(relativeBodyPos.transform.position, relativeBodyPos.transform.position + tiltedAngle * -body.transform.up, Color.blue);
        
        if (Physics.Raycast(_rayPoint, -body.transform.up, out _hit, 10f, ground))
        {
            transform.position = _hit.point;
        }
        
        if (Physics.Raycast(relativeBodyPos.transform.position, tiltedAngle * -body.transform.up, out _newHit, 10f, ground))
        {
            if (Vector3.Distance(transform.position, _newHit.point) > stepSize && _isLerping == false)
            {
                _oldPos = _newHit.point;
            }
        }
        
    }

    private IEnumerator TakeStep(Vector3 target)
    {
        _isLerping = true;
        _targetPos = target;
        _lerpTime = 0f;

        Vector3 startPos = transform.position;
        Vector3 midPoint = (startPos + _targetPos) / 2;
        midPoint += Vector3.up * stepHeight;

        while (_lerpTime < stepDuration)
        {
            _lerpTime += Time.deltaTime;
            float t = _lerpTime / stepDuration;
            transform.position = Vector3.Lerp(Vector3.Lerp(startPos, midPoint, t), Vector3.Lerp(midPoint, _targetPos, t), t);
            yield return null;
        }
        
        transform.position = _targetPos;
        _oldPos = _targetPos;
        _isLerping = false;
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.magenta;
        // Gizmos.DrawSphere(_hit.point, 0.1f);
        //
        // Gizmos.color = Color.green;
        // Gizmos.DrawSphere(_newHit.point, 0.1f);
    }
}
