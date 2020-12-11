using UnityEngine;
using UnityEngine.UI;
using System;

namespace Wayfinder {
  public class AbilityButton : MonoBehaviour {
    public Button button;
    public Ability ability;
    private bool abilitySet = false;

    public event EventHandler AbilityButtonClicked;

    public void SetButtonImage(Sprite sprite) {
      this.button.image.sprite = sprite;
      this.button.image.color = Color.white;
    }

    public void SetAbility(Ability ability) {
      this.ability = ability;
      this.abilitySet = true;
      this.SetButtonImage(ability.icon);
    }

    public void Empty() {
      this.button.image.sprite = null;
      this.button.image.color = new Color(0.45f, 0.45f, 0.45f, 1f);
      this.ability = null;
      this.abilitySet = false;
    }

    // Used on UI button event in Unity component (look at the sidebar in Unity idiot)
    public void OnClick() {
      if (!this.abilitySet) return;

      if (this.AbilityButtonClicked != null) {
        this.AbilityButtonClicked.Invoke(this, new EventArgs());
      }
    }
  }
}
