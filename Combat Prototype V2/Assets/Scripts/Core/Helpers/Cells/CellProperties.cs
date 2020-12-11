using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayfinder {
    [Serializable]
    public class CellProperties {
      public bool altitudeObstruction;
      public bool fullObstruction;
    }

    /// <summary>
    /// Obstructions can be applied to cells to block entities from entering,
    /// or to entites to allow them to enter cells with those obstructions.
    /// </summary>
    [Serializable]
    public class Obstructions {
        public ObstructionDictionary types;

        public Obstructions(params string[] types) {
            foreach (string type in types) {
                if (this.types[type]) {
                    continue;
                } else {
                    this.types[type] = true;
                }
            }
        }

        public override bool Equals(System.Object obj) {
            //Check for null and compare run-time types.
            if (obj == null || !this.GetType().Equals(obj.GetType())) {
                return false;
            } else {
                Obstructions o = (Obstructions) obj;
                return DictionaryComparer.ContentEquals<string, bool>(this.types, o.types);
            }
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return this.types.ToString();
        }
    }

    /// <summary>
    /// A serializable dictionary for obstruction types.
    /// </summary>
    [Serializable]
    public class ObstructionDictionary : SerializableDictionary<string, bool> {}

    /// <summary>
    /// Compares two dictionaries
    /// </summary>
    public static class DictionaryComparer {
        public static bool ContentEquals<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> otherDictionary) {
            return (otherDictionary ?? new Dictionary<TKey, TValue>())
                .OrderBy(kvp => kvp.Key)
                .SequenceEqual((dictionary ?? new Dictionary<TKey, TValue>())
                                   .OrderBy(kvp => kvp.Key));
        }
    }
}
