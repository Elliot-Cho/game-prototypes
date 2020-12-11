using UnityEngine;

namespace Wayfinder {
  public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    public BattleUI battleUI;

    void Awake() {
      if (!Instance) {
        Instance = this;
      }
      else
        Destroy(gameObject);
    }
  }
}
