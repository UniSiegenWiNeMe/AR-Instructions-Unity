using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSolver : MonoBehaviour
{
    private bool isFixed = false;
    public SolverHandler Solver;

    public void ToggleSolverOnOff()
    {
        isFixed = !isFixed;

        Solver.enabled = !isFixed;
    }
}
