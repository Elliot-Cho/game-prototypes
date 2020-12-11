using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Wayfinder {
  public class Faction : MonoBehaviour {
    public string factionName;
    public bool controllable = false;

    public List<Entity> members = new List<Entity>();

    public List<Faction> allies = new List<Faction>();
    public List<Faction> enemies = new List<Faction>();

    public static Faction Find(string name) {
      var factions = GameObject.FindGameObjectsWithTag("Faction");

      GameObject foundFaction = factions.Single(
        faction => faction.GetComponent<Faction>().factionName == name
      );

      return foundFaction.GetComponent<Faction>();
    }

    protected void Awake() {
      this.gameObject.tag = "Faction";
    }

    protected void Start() {
      foreach (Transform child in this.gameObject.transform) {
        this.members.Add(child.GetComponent<Entity>());
      }
    }

    public bool IsAlly(Entity entity) {
      return this.allies.Any(allyFaction => allyFaction.members.Contains(entity));
    }

    public bool IsEnemy(Entity entity) {
      return this.enemies.Any(enemyFaction => enemyFaction.members.Contains(entity));
    }

    public bool IsNeutral(Entity entity) {
      if (this.members.Contains(entity)) return false;

      return !IsAlly(entity) && !IsEnemy(entity);
    }

    public bool IsFriendly(Entity entity) {
      return this.members.Contains(entity) || IsAlly(entity);
    }
  }
}
