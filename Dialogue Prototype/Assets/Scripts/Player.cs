using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {

    public Persona persona;

    public List<string> party;

    public List<string> playerVars;
}

[System.Serializable]
public class Persona
{
    [Range(-100.0f, 100.0f)]
    public float rational;

    [Range(-100.0f, 100.0f)]
    public float serious;
}