using UnityEngine;
using UnityEngine.Assertions;

namespace SCILL.Effects
{
    public abstract class SCILLAudioBase : MonoBehaviour
    {
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected SCILLAudioSettings audioSettings;
        
        protected void Start()
        {
            Assert.IsNotNull(audioSource,
                "SCILLPersonalChallengeAudio on object " + gameObject.name + " is missing the audioSource.");
            Assert.IsNotNull(audioSettings,
                "SCILLPersonalChallengeAudio on object " + gameObject.name + " is missing audioSettings.");
        }
    }
}