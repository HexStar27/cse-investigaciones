using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CSE.Feedback
{
    [System.Serializable]
    public class InscryptionableFeedback : MonoBehaviour
    {
        public static Dictionary<string,InscryptionableFeedback> dictionary { get; protected set; } = new();

        public virtual async Task DirectChange(int value)
        {
            await Task.Yield();
        }

        public virtual void SilentChange(string value) { }
    }
}