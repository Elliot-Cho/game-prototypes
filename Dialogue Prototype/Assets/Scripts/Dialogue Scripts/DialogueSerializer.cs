using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

public class DialogueSerializer : MonoBehaviour
{
    private void Start()
    {
       /* Dialogue testDialogue = TestDialogueSerialization();

        XMLOp.Serialize(testDialogue, "Assets/XML/dialogue.xml");*/
    }

    public Dialogue TestDialogueSerialization()
    {
        // Initialize test variables
        Dialogue dialogue = new Dialogue();
        /*InitConversation initConversation = new InitConversation();
        InitConversation initConversation2 = new InitConversation();
        InitConversation.DialogueOption dialogueOption = new InitConversation.DialogueOption();
        InitConversation.DialogueOption dialogueOption2 = new InitConversation.DialogueOption();
        InitConversation.DialogueOption dialogueOption3 = new InitConversation.DialogueOption();

        DialogueResponse dialogueResponse = new DialogueResponse();
        DialogueResponse.ResponseOption responseOption = new DialogueResponse.ResponseOption();
        DialogueResponse.ResponseOption responseOption2 = new DialogueResponse.ResponseOption();
        DialogueResponse.ResponseOption.Option option = new DialogueResponse.ResponseOption.Option();
        DialogueResponse.ResponseOption.Option option2 = new DialogueResponse.ResponseOption.Option();
        DialogueResponse.ResponseOption.Option option3 = new DialogueResponse.ResponseOption.Option();

        // Set up dialogueOption
        dialogueOption.id = "042";
        dialogueOption.text = "Testing dialogue text.";
        dialogueOption2.id = "043";
        dialogueOption2.text = "Testing dialogue text 2.";
        dialogueOption3.id = "044";
        dialogueOption3.text = "Testing dialogue text 3.";

        // Set up initConversation
        initConversation.relationsMin = 10;
        initConversation.relationsMax = 50;
        initConversation.party = new string[2];
        initConversation.var = new string[1];
        initConversation.option = new InitConversation.DialogueOption[2];
        initConversation.party[0] = "Alice";
        initConversation.party[1] = "Mallory";
        initConversation.var[0] = "Test Variable";
        initConversation.option[0] = dialogueOption;
        initConversation.option[1] = dialogueOption2;

        initConversation2.relationsMin = -20;
        initConversation2.relationsMax = 10;
        initConversation2.party = new string[2];
        initConversation2.var = new string[1];
        initConversation2.option = new InitConversation.DialogueOption[1];
        initConversation2.party[0] = "Bob";
        initConversation2.party[1] = "Mallory";
        initConversation2.var[0] = "Test Variable 2";
        initConversation2.option[0] = dialogueOption3;

        // Set up option
        option.rationalMin = "0";
        option.rationalMax = "20";
        option.seriousMin = "-10";
        option.seriousMax = "0";
        option.text = "Test Response.";
        option2.rationalMin = "-80";
        option2.rationalMax = "-60";
        option2.seriousMin = "-30";
        option2.seriousMax = "0";
        option2.text = "Test Response 2.";
        option3.text = "Test Response 3.";

        // Set up responseOption
        responseOption.speaker = "Player";
        responseOption.option = new DialogueResponse.ResponseOption.Option[2];
        responseOption.option[0] = option;
        responseOption.option[1] = option2;
        responseOption2.speaker = "Bob";
        responseOption2.option = new DialogueResponse.ResponseOption.Option[1];
        responseOption2.option[0] = option3;

        // Set up dialogueResponse
        dialogueResponse.id = "042";
        dialogueResponse.responseOption = new DialogueResponse.ResponseOption[2];
        dialogueResponse.responseOption[0] = responseOption;
        dialogueResponse.responseOption[1] = responseOption2;

        // Set up dialogue
        dialogue.id = "test";
        dialogue.init = new InitConversation[2];
        dialogue.init[0] = initConversation;
        dialogue.init[1] = initConversation2;
        dialogue.response = new DialogueResponse[1];
        dialogue.response[0] = dialogueResponse;*/

        return dialogue;
    }
}