using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {

    public static Player Instance { get; private set; }

    public Persona persona;

    public List<string> party;

    public List<string> playerVars;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}

[System.Serializable]
public class Persona
{
    [Range(-100.0f, 100.0f)]
    public float rational;

    [Range(-100.0f, 100.0f)]
    public float serious;
}