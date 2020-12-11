using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wayfinder {
  /// <summary>
  /// Characters are units that are capable of being directly controlled by the player.
  /// </summary>
  public class Character : Unit {
    public static List<Character> CharacterList = new List<Character>();

    public Attributes attributes = new Attributes();
    public Talents talents = new Talents();
    public TalentStats talentStats = new TalentStats();

    [Range(-100f, 100f)]
    public float playerRelations;

    [Range(-100f, 100f)]
    public float loyalty;

    public List<string> npcVars;

    protected override void Start() {
      base.Start();
      CharacterList.Add(this);
    }

    protected override void OnDestroy() {
      CharacterList.Remove(this);
      base.OnDestroy();
    }
  }
}