using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RedBlueGames.Tools.TextTyper;

public abstract class Entity : MonoBehaviour {

    // List of all entities instantiated in the game
    public static List<Entity> entityList = new List<Entity>();

    public string id;
    public string entityName;
    public string cond; // Replace outside of prototype with state machine

    public Texture2D rawPortraitSet;

    public List<Sprite> portraitList = new List<Sprite>();

    // All possible dialogues with this entity in all possible entity states
    [HideInInspector]
    public List<Dialogue> entityDialogues = new List<Dialogue>();

    // Dialogue object with used chains removed
    [HideInInspector]
    public List<Dialogue> workingDialogues = new List<Dialogue>();

    // Chain currently being used for dialogue
    [HideInInspector]
    public Dialogue.Conversation.Chain currentChain = new Dialogue.Conversation.Chain();

    [HideInInspector]
    public GameController gameController;
    [HideInInspector]
    public DialogueController dialogueController;
    [HideInInspector]
    public Player player;

    void Start()
    {
        // Grab required object variables
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        dialogueController = gameController.dialogueController;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // Add entity to entity list on instantiation
        entityList.Add(this);
        
        // Cut up portraits
        portraitList = SlicePortrait();

        
    }

    /// <summary>
    /// Slices a portrait sprite into 128x128 pixel portraits for use by the dialogue controller.
    /// </summary>
    /// <returns>A list of sliced portrait sprites.</returns>
    public List<Sprite> SlicePortrait ()
    {
        List<Sprite> result = new List<Sprite>();

        var width = rawPortraitSet.width;
        var height = rawPortraitSet.height;

        if (width % 128f != 0f || height % 128f != 0f)
        {
            Debug.Log("Bad portrait size: not divisible by 128 pixels");
            return result;
        }
        
        int xSlice = width / 128;
        int ySlice = height / 128;

        for (int i = ySlice - 1; i >= 0; i--)
        {
            for (int j = 0; j < xSlice; j++)
            {
                Color[] pix = rawPortraitSet.GetPixels(128 * j, 128 * i, 128, 128);
                Texture2D destTex = new Texture2D(128, 128);
                destTex.SetPixels(pix);
                destTex.Apply();

                Sprite newSprite = Sprite.Create(destTex, new Rect(0, 0, destTex.width, destTex.height), new Vector2(0.5f, 0.5f));
                result.Add(newSprite);
            }
        }

        return result;
    }

    void OnDestroy()
    {
        // Remove entity from entity list on destruction
        entityList.Remove(this);
    }
}
