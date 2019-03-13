using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInputNode : BaseNode {

    // Result of node
	public virtual string GetResult()
    {
        return "None";
    }

    public override void DrawCurves()
    {

    }
}
