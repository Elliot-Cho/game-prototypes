using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityLoader : MonoBehaviour {

    public const string path = "Core/Abilities/Abilities";

	// Use this for initialization
	void Start () {
        AbilityCollection col = AbilityCollection.Load(path);

        foreach (var ability in col.Abilities)
        {
            Debug.Log(ability.Name);
        }
	}
}
