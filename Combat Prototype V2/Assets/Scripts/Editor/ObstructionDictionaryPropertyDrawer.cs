using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wayfinder {
    /// <summary>
    /// Needed for serializable dictionary inspector UI
    /// </summary>
    [CustomPropertyDrawer(typeof(ObstructionDictionary))]
    public class ObstructionDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {} // VSCode may mark this line incorrect. It's fine.
}