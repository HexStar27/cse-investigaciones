using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CSE.Feedback
{
    public class AgentUpdater : InscryptionableFeedback
    {
        [SerializeField] string idName;
        [SerializeField] float animDelay = 0.1f;
        [Header("Required fields")]
        [SerializeField] Animator anim;
        [SerializeField] AudioSource aSource;
        [SerializeField] TMPro.TextMeshProUGUI tmesh;

        private void OnEnable()
        {
            InscryptionableFeedback.dictionary.Add(idName, this);
        }
        private void OnDisable()
        {
            InscryptionableFeedback.dictionary.Remove(idName);
        }

        public override async Task DirectChange(int targetValue)
        {
            int delay = (int)(1000 * animDelay);
            int currentValue = int.Parse(tmesh.text);
            float maxSoundJumps = 10;
            float valuesJumped = 0;

            while (currentValue != targetValue)
            {
                int dir = Math.Sign(targetValue - currentValue);
                currentValue += dir;
                tmesh.text = currentValue.ToString();
                anim.Play("ValueChange");
                aSource.pitch = 1f + Mathf.Min(1, valuesJumped++ / maxSoundJumps);
                aSource.Play();
                await Task.Delay(delay);
            }
        }

        public override void SilentChange(string value) => tmesh.text = value;
    }
}