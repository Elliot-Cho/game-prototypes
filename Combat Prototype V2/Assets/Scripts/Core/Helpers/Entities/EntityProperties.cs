using UnityEngine;
using System;

namespace Wayfinder {
  /// <summary>
  /// Entity properties determine how the entity behaves in the world.
  /// </summary>
  [Serializable]
  public class EntityProperties {
    public bool attackable;
    public bool obstruction;
    public bool movable;
  }

  /// <summary>
  /// Used for the Player. Determines player personality for dialogue.
  /// </summary>
  public class Persona {
    [Range(-100.0f, 100.0f)]
    public float rational;
    [Range(-100.0f, 100.0f)]
    public float serious;
  }
}