using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour {

    public List<string> globalVars;

    public Dialogue pseudoDialogue;

    public List<Entity> entities;

    public DialogueController dialogueController;

    public GameObject debugProgressDay;

    // Use this for initialization
    void Start () {
        pseudoDialogue = XMLOp.Deserialize<Dialogue>("Assets/XML/Dialogue Pseudo.xml");

        // Deserialization testing
        //TestDeserialization();

        // Assign dialogues to their respective characters
        AssignDialogues();

        var progressButton = debugProgressDay.GetComponent<Button>();
        progressButton.onClick.AddListener(dialogueController.ProgressDay);
    }

    void TestDeserialization()
    {
        XMLOp.Serialize(pseudoDialogue, "Assets/XML/dialogue.xml");
        Debug.Log(pseudoDialogue.conversation[0].dialogueChains[1].dialogueOptions[0].textOptions[0].dialogueText[0].text);
        Debug.Log(pseudoDialogue.conversation[0].dialogueChains[1].dialogueOptions[1].textOptions[0].dialogueText[0].text);
    }

    // Assign dialogues to their respective characters
    void AssignDialogues()
    {
        // For each entity, if there is a dialogue with the same id, assign that dialogue to the entity
        foreach (Entity entity in Entity.entityList)
        {
            if (entity.id.Equals(pseudoDialogue.id))
            {
                entity.entityDialogues.Add(pseudoDialogue);
                entity.workingDialogues.Add(ObjectCopier.Clone(pseudoDialogue));
            }

            entity.currentChain = dialogueController.GetDialogueChain(entity);
        }
    }


}

public static class ObjectCopier
{
    /// <summary>
    /// Perform a deep Copy of the object.
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static T Clone<T>(T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        // Don't serialize a null object, simply return the default for that object
        if (System.Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}

public static class ListComparer
{
    public static bool CompareLists<T>(List<T> aListA, List<T> aListB)
    {
        if (aListA == null || aListB == null || aListA.Count != aListB.Count)
            return false;
        if (aListA.Count == 0)
            return true;
        Dictionary<T, int> lookUp = new Dictionary<T, int>();
        // create index for the first list
        for (int i = 0; i < aListA.Count; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(aListA[i], out count))
            {
                lookUp.Add(aListA[i], 1);
                continue;
            }
            lookUp[aListA[i]] = count + 1;
        }
        for (int i = 0; i < aListB.Count; i++)
        {
            int count = 0;
            if (!lookUp.TryGetValue(aListB[i], out count))
            {
                // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                return false;
            }
            count--;
            if (count <= 0)
                lookUp.Remove(aListB[i]);
            else
                lookUp[aListB[i]] = count;
        }
        // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
        return lookUp.Count == 0;
    }

    // Is everything in contained also in container?
    public static bool ContainsAll<T>(List<T> container, List<T> contained)
    {
        if (container == null || contained == null || contained.Count > container.Count)
            return false;
        if (contained.Count == 0)
            return true;
        
        foreach (var containedItem in contained)
        {
            if (!container.Contains(containedItem))
                return false;
        }

        return true;
    }
}
