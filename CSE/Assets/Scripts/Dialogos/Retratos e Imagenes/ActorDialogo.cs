using System.Collections.Generic;
using UnityEngine;

namespace Hexstar.Dialogue
{
    [CreateAssetMenu(fileName ="Actor",menuName ="Hexstar/Dialogue/Actor")]
    [System.Serializable]
    public class ActorDialogo : ScriptableObject
    {
        [SerializeField] List<Sprite> portraits;
        [SerializeField] List<string> expressions;

        public Sprite GetExpression(string reference)
        {
            int r = expressions.IndexOf(reference);
            if (r < 0) return null;
            return portraits[r];
        }
    }
}