using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericObstacle : Obstacle {

    public string ObsName;

    public override void Initialize()
    {
        base.Initialize();
        transform.position += new Vector3(0, 0, -0.1f);
    }

    public override void MarkAsTargetable()
    {
        SetColor(new Color(1, 0.8f, 0.8f));
    }

    public override void Mark(Color color)
    {
        SetColor(color);
    }

    public override void UnMark()
    {
        SetColor(Color.white);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
