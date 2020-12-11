using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wayfinder {
  /// <summary>
  /// The player character. Has unique values compared to other units and characters.
  /// </summary>
  public class PlayerCharacter : Unit {
    // Current player instance, for singleton
    public static PlayerCharacter Instance { get; private set; }

    // Player stats
    public Attributes attributes = new Attributes();
    public Talents talents = new Talents();
    public TalentStats talentStats = new TalentStats();

    /// <summary>
    /// Player personality values.
    /// </summary>
    public Persona persona;

    /// <summary>
    /// Characters that are currently in the player's party
    /// </summary>
    public List<Character> party;

    /// <summary>
    /// Player flags that have been set.
    /// </summary>
    public List<string> flags;

    protected override void Awake() {
      base.Awake();

      // Allow only a single instance of the game manager to exist (Singleton)
      if (!Instance) {
        Instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
        Destroy(gameObject);
    }

    protected override void Start() {
      base.Start();
    }

    void Update() {
      this.state.Execute();
    }

    protected override void OnDestroy() {
      base.OnDestroy();
    }
  }
}
