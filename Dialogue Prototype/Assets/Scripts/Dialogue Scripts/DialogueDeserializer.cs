using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDeserializer : MonoBehaviour {

    public Dialogue pseudoDialogue;

	// Use this for initialization
	void Start () {
        pseudoDialogue = XMLOp.Deserialize<Dialogue>("Assets/XML/Dialogue Pseudo.xml");

        // Testing
        //XMLOp.Serialize(pseudoDialogue, "Assets/XML/dialogue.xml");
        //Debug.Log(pseudoDialogue.conversation[0].dialogueChains[1].dialogueOptions[0].textOptions[0].dialogueText[0].text);
        //Debug.Log(pseudoDialogue.conversation[0].dialogueChains[1].dialogueOptions[1].textOptions[0].dialogueText[0].text);
    }
}
