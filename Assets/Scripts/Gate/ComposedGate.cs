using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComposedGate : MonoBehaviour, IGate
{
    [SerializeField] private SingleGate[] gates;// IGate would be nicer but Interfaces are not serializable
    [SerializeField] private bool open;//Just for testing in editmode

    public bool IsClosed() {
        foreach (var gate in gates)
            if (!gate.IsClosed())
                return false;
        return true;
    }
    public bool IsOpen() {
        foreach (var gate in gates)
            if (gate.IsOpen())
                return true;
        return false;
    }

    public void SetOpening(bool opening) {
        foreach (var gate in gates)
            gate.SetOpening(opening);
        open = opening;
    }

    private void OnValidate() {
        if (gates != null)
            SetOpening(open);
    }
}
