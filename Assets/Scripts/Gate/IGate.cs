using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGate
{
    public bool IsClosed();
    public bool IsOpen();
    public void SetOpening(bool opening);
}
