using System;
using UnityEngine;

public class BTSolver : MonoBehaviour
{
    public BehaviorTree _tree;

    private void Start()
    {
        _tree = _tree.Clone();
    }

    private void Update()
    {
        _tree.Tick();
    }
}