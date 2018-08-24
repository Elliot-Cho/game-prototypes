using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RedBlueGames.Tools.TextTyper;
using System;

public class DialogueController : MonoBehaviour {

    // Singleton instance; DialogueController should only exist once
    public static DialogueController Instance { get; private set; }

    // Used for dialogue box typing
    [SerializeField]
    public TextMeshProUGUI entityName;
    [SerializeField]
    public TextTyper content;
    [SerializeField]
    public Image image;
    [SerializeField]
    public GameObject choices;

    // Test bool for choosing state; replace later
    [HideInInspector]
    public bool choosing = false;
    public Dialogue.Conversation.Chain.DialogueOptions.ChoiceOptions currentChoiceOptions;

    // Test bool for last state; replace later
    [HideInInspector]
    public bool last = false;

    // Id for choice selections to lead to appropriate response dialogues
    [HideInInspector]
    public string choiceId = "";

    // Called before Start
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start() {
        // Hide dialogue box until it's used
        this.gameObject.SetActive(false);
        choices.gameObject.SetActive(false);
    }

    /// <summary>
    /// Grab entity dialogue and display it to the UI canvas as dialogue.
    /// </summary>
    /// <param name="entity">The activating entity.</param>
    public void Talk(Entity entity)
    {
        if (entity.currentChain.dialogueOptions != null && !choosing)
        {
            // Check if the dialogue is currently typing. If it is, skip the typing. If not, get new dialogue.
            if (!content.IsTyping)
            {
                // If the current chain is empty, get the next chain
                if (entity.currentChain.dialogueOptions.Count.Equals(0))
                {
                    entity.currentChain = GetDialogueChain(entity);
                }

                // Display dialogue on screen
                DisplayDialogue(GetDialogueValuesFromChain(entity));
            }
            else
            {
                // Skip typing of dialogue and display the whole thing
                content.Skip();
            }
        }
    }

    // Simulate a progressed day for debugging purposes. Resets day dialogue so that NPCs say new stuff.
    public void ProgressDay ()
    {
        choices.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        last = false;
        choosing = false;
        choiceId = "";

        // Reset all current dialogue chains
        foreach (Entity entity in Entity.entityList)
        {
            entity.currentChain = GetDialogueChain(entity);
        }

        Debug.Log("Day Progressed");
    }

    /// <summary>
    /// Activate and display the dialogue box UI, showing given DialogueValues.
    /// </summary>
    /// <param name="values">The dialogue content, speaker name and portrait, and choices.</param>
    public void DisplayDialogue (DialogueValues values)
    {
        // If it's the last dialogue option, close the dialogue instead of continuing
        if (last)
        {
            foreach (Transform choice in choices.transform)
                choice.gameObject.SetActive(false);
            choices.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
            last = false;
            return;
        }

        this.gameObject.SetActive(true);
        entityName.text = values.entityName;
        content.TypeText(values.content);
        image.sprite = values.portrait;

        // Display choices if there are any
        if (values.choiceOptions != null)
        {
            // Wait until the content has finished typing before showing the choices
            StartCoroutine(DisplayChoicesAfterDialogue(values));
        }
        else
        {
            // Hide choice menu if there aren't any choices
            foreach (Transform choice in choices.transform)
                choice.gameObject.SetActive(false);
            choices.gameObject.SetActive(false);
        }

        // If last has been flagged, set the state of dialogue to last line
        if (values.last)
        {
            last = true;
        }
    }

    // Show choices only after the dialogue has finished typing
    private IEnumerator DisplayChoicesAfterDialogue (DialogueValues values)
    {
        do
        {
            yield return new WaitForSeconds(0.5f);
        }
        while (content.IsTyping);

        choices.gameObject.SetActive(true);

        // Set dialogue controller choiceOptions
        currentChoiceOptions = values.choiceOptions;

        for (int i = 0; i < values.choiceOptions.choice.Count; i++)
        {
            var choice = choices.transform.GetChild(i);
            var button = choice.GetComponent<Button>();

            choice.gameObject.SetActive(true);
            // Set choice text
            choice.GetComponentInChildren<TextMeshProUGUI>().text = values.choiceOptions.choice[i].select;

            // Add choice selection function to the button's onClick
            var choiceCopy = values.choiceOptions.choice[i];
            button.onClick.AddListener(delegate { SelectChoice(values.entity, choiceCopy); });
        }
    }

    /// <summary>
    /// Called when a dialogue choice is made. Write the resulting text attached to the choice in the dialogue box with proper DialogueValues.
    /// </summary>
    /// <param name="entity">The entity that is being talked to.</param>
    /// <param name="choice">The choice object in the DialogueObject.</param>
    public void SelectChoice (Entity entity, Dialogue.Conversation.Chain.DialogueOptions.ChoiceOptions.Choice choice)
    {
        choiceId = choice.id;

        DialogueValues dialogueValues = new DialogueValues();

        foreach (Dialogue.Conversation.Chain.DialogueOptions.TextOptions option in choice.textOptions)
        {
            if (entity is NPC)
            {
                var npc = entity as NPC;

                if (npc.MatchPersona(option.persona))
                {
                    dialogueValues.entityName = Player.Instance.entityName;
                    dialogueValues.portrait = Player.Instance.portraitList[GetPortraitNumber(option.portraitValue)];

                    dialogueValues.content = option.dialogueText[UnityEngine.Random.Range(0, option.dialogueText.Count)].text;
                    break;
                }
            }
        }

        DisplayDialogue(dialogueValues);

        choosing = false;
    }

    /// <summary>
    /// Retrieve next set of DialogueValues from an entity's current dialogue chain.
    /// </summary>
    /// <param name="entity">The entity to take a dialogue chain from.</param>
    /// <returns>The chain's DialogueValues.</returns>
    public DialogueValues GetDialogueValuesFromChain(Entity entity)
    {
        DialogueValues result = new DialogueValues();

        var currChain = entity.currentChain;

        result.entity = entity;

        for (int i = 0; i < currChain.dialogueOptions.Count; i++)
        {
            if (entity is NPC)
            {
                var npc = entity as NPC;

                // If a choice has been made and its Id doesn't equal the dialogue option id, then skip this option
                if (!choiceId.Equals(currChain.dialogueOptions[i].id))
                    continue;

                // If this dialogue option has already been spoke, skip it
                if (currChain.dialogueOptions[i].type.Equals("used"))
                    continue;
                
                foreach (Dialogue.Conversation.Chain.DialogueOptions.TextOptions textOption in currChain.dialogueOptions[i].textOptions)
                {
                    // Check for persona restraints
                    if (!npc.MatchPersona(textOption.persona))
                    {
                        continue;
                    }

                    // Check which entity is doing the talking
                    foreach(Entity speaker in Entity.entityList)
                    {
                        if (currChain.dialogueOptions[i].speaker.Equals(speaker.id))
                        {
                            // Set the name of the speaker
                            result.entityName = speaker.entityName;
                            // Set the portrait of the speaker
                            result.portrait = speaker.portraitList[GetPortraitNumber(textOption.portraitValue)];

                            break;
                        }
                    }

                    // Select a random line of text from the text options and set it as the dialogue content
                    result.content = textOption.dialogueText[UnityEngine.Random.Range(0, textOption.dialogueText.Count)].text;

                    // If this dialogue option has a choice in it, send the choice as part of the value
                    if (currChain.dialogueOptions[i].type.Equals("choice"))
                    {
                        result.choiceOptions = currChain.dialogueOptions[i].choiceOptions;
                        choosing = true;
                    }

                    // Flag the current dialogue option as used (to advance the chain)
                    // However, shift repeating dialogue options over eachother so they repeat forever (until the day progresses and new dialogue can be spoken)
                    if (!currChain.dialogueOptions[i].type.Equals("repeater") && !currChain.dialogueOptions[i].type.Equals("last") && !currChain.dialogueOptions[i].type.Equals("uniquelast"))
                    {
                        currChain.dialogueOptions[i].type = "used";

                        // If the next dialogue in the chain is marked as uniquelast, mark the current one as last as dialogue value
                        if (i < currChain.dialogueOptions.Count)
                        {
                            if (currChain.dialogueOptions[i + 1].type.Equals("uniquelast"))
                                result.last = true;
                        }
                    }
                    else
                    {
                        // If a dialogue option is marked as last or uniquelast, send it as a dialogue value
                        if (currChain.dialogueOptions[i].type.Equals("last") || currChain.dialogueOptions[i].type.Equals("uniquelast"))
                        {
                            result.last = true;
                        }
                        
                        if (!last)
                        {
                            // Move repeating dialogue to back of list
                            currChain.dialogueOptions.Add(currChain.dialogueOptions[i]);
                            currChain.dialogueOptions.RemoveAt(i);
                        }
                    }

                    break;
                }
                break;
            }
            else
            {
                // Implement interaction with NON-NPC here
                Debug.Log("Non-NPC GetChainValues unimplemented");
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Retrieve a dialogue chain from an entity's list of valid chains.
    /// </summary>
    /// <param name="entity">The entity to grab a dialogue chain from.</param>
    /// <returns>A valid dialogue chain.</returns>
    public Dialogue.Conversation.Chain GetDialogueChain(Entity entity)
    {
        var result = new Dialogue.Conversation.Chain();

        if (entity.validChains.Count.Equals(0))
        {
            entity.validChains = GetValidChains(entity);
            if (entity.validChains.Count.Equals(0))
                return result;
        }

        // Pick a random chain in a list of chains
        result = entity.validChains[UnityEngine.Random.Range(0, entity.validChains.Count)];

        // Remove the chain from list of valid chains
        entity.validChains.Remove(result);

        return result;
    }

    /// <summary>
    /// Retrieve a list of all valid dialogue chains from an entity, given that they satisfy NPC relations, conditions, variables, and party members.
    /// </summary>
    /// <param name="entity">The entity to grab chains from.</param>
    /// <returns>A lsit of dialogue chains.</returns>
    public List<Dialogue.Conversation.Chain> GetValidChains(Entity entity)
    {
        var result = new List<Dialogue.Conversation.Chain>();
        foreach (Dialogue dialogue in entity.entityDialogues)
        {
            if (dialogue.cond.Equals(entity.cond))
            {
                foreach (Dialogue.Conversation conversation in dialogue.conversation)
                {
                    // If the entity is an NPC, then check for relation when pulling valid chains
                    if (entity is NPC)
                    {
                        var npc = entity as NPC;
                        if (conversation.relationsMin <= npc.relations && conversation.relationsMax >= npc.relations
                                                && MatchVars(conversation.vars, npc) && MatchParty(conversation.party))
                        {
                            result.AddRange(ObjectCopier.Clone(conversation.dialogueChains));
                            continue;
                        }
                    }
                    else
                    {
                        result.AddRange(ObjectCopier.Clone(conversation.dialogueChains));
                        continue;
                    }
                }
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Check if dialogue variables match current Global + Player + NPC variables.
    /// </summary>
    /// <param name="dialogueVars">The list of dialogue variables to check.</param>
    /// <param name="npc">The NPC to check.</param>
    /// <returns>Returns true if all dialogue variables are fulfilled.</returns>
    private bool MatchVars(List<string> dialogueVars, NPC npc)
    {
        List<string> currentVars = new List<string>();

        currentVars.AddRange(npc.npcVars);
        currentVars.AddRange(Player.Instance.playerVars);
        currentVars.AddRange(GameController.Instance.globalVars);

        return ListComparer.ContainsAll(currentVars, dialogueVars);
    }

    /// <summary>
    /// Check if dialogue party member requirements match the player's current party.
    /// </summary>
    /// <param name="dialoguePartyMembers">The list of dialogue party member ids to check.</param>
    /// <returns>Returns true if the player's party contains the required members.</returns>
    private bool MatchParty(List<string> dialoguePartyMembers)
    {
        return ListComparer.ContainsAll(Player.Instance.party, dialoguePartyMembers);
    }

    /// <summary>
    /// Given a string, check if the string corresponds to a portrait number, and return the number.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>Returns the portrait number.</returns>
    private int GetPortraitNumber (string value)
    {
        int result = 0;

        if (int.TryParse(value, out result))
        {
            return result;
        }

        switch (value.Trim().ToLower())
        {
            case "neutral":
                result = 0;
                break;
            case "content":
                result = 1;
                break;
            case "happy":
                result = 2;
                break;
            case "serious":
                result = 3;
                break;
            case "annoyed":
                result = 4;
                break;
            case "angry":
                result = 5;
                break;
            case "shock":
                result = 6;
                break;
            case "sad":
                result = 7;
                break;
            case "hurt":
                result = 8;
                break;
            default:
                result = 0;
                break;
        }

        return result;
    }
}

public class DialogueValues
{
    public Entity entity;
    public string entityName;
    public string content;
    public Sprite portrait;
    public bool last = false;

    public Dialogue.Conversation.Chain.DialogueOptions.ChoiceOptions choiceOptions = null;
}
