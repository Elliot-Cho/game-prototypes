using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Entity {
    [Range(-100.0f, 100.0f)]
    public float relations;

    public List<string> npcVars;

    private void OnMouseDown()
    {
        DialogueController.Instance.Talk(this);
    }

    /// <summary>
    /// Given a string, checks if the string is a persona and then checks if it matches the player's persona values.
    /// </summary>
    /// <param name="required">The string to be checked.</param>
    /// <returns>True if the string is the player's current persona.</returns>
    public bool MatchPersona (string required)
    {
        var rational = Player.Instance.persona.rational;
        var serious = Player.Instance.persona.serious;

        if (required.Equals("None"))
            return true;

        // If required persona descriptions match with player's persona values, then true
        if ((required.Equals("Neutral") && rational < 33 && rational > -33 && serious < 33 && serious > -33) ||
                (required.Equals("Stiff") && rational <= 33 && rational >= -33 && serious <= 66 && serious >= 33) ||
                (required.Equals("Relaxed") && rational <= 33 && rational >= -33 && serious <= -33 && serious >= -66) ||
                (required.Equals("Practical") && rational <= 66 && rational >= 33 && serious <= 33 && serious >= -33) ||
                (required.Equals("Excitable") && rational <= -33 && rational >= -66 && serious <= 33 && serious >= -33) ||
                (required.Equals("Pragmatic") && (rational <= 100 && rational > 33 && serious < 66 && serious > 33) || (rational < 66 && rational > 33 && serious <= 100 && serious >= 66)) ||
                (required.Equals("Tense") && (rational < -33 && rational >= -100 && serious < 66 && serious > 33) || (rational < -33 && rational > -66 && serious <= 100 && serious >= 66)) ||
                (required.Equals("Idealistic") && (rational < -33 && rational >= -100 && serious < -33 && serious > -66) || (rational < -33 && rational > -66 && serious <= -66 && serious >= -100)) ||
                (required.Equals("Cool") && (rational <= 100 && rational > 33 && serious < -33 && serious > -66) || (rational < 66 && rational > 33 && serious <= -66 && serious >= -100)) ||
                (required.Equals("Rational") && rational <= 100 && rational > 66 && serious <= 33 && serious >= -33) ||
                (required.Equals("Serious") && rational <= 33 && rational >= -33 && serious <= 100 && serious > 66) ||
                (required.Equals("Romantic") && rational < -66 && rational >= -100 && serious <= 33 && serious >= -33) ||
                (required.Equals("Casual") && rational <= 33 && rational >= -33 && serious < -66 && serious >= -100) ||
                (required.Equals("Professional") && rational <= 100 && rational >= 66 && serious <= 100 && serious >= 66) ||
                (required.Equals("Moody") && rational <= -66 && rational >= -100 && serious <= 100 && serious >= 66) ||
                (required.Equals("Adventurous") && rational <= -66 && rational >= -100 && serious <= -66 && serious >= -100) ||
                (required.Equals("Composed") && rational <= 100 && rational >= 66 && serious <= -66 && serious >= -100))
            return true;

        return false;
    }
}
