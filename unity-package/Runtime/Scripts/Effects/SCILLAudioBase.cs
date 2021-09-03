using UnityEngine;
using UnityEngine.Assertions;

namespace SCILL.Effects
{
    public abstract class SCILLAudioBase : MonoBehaviour
    {
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected SCILLAudioSettings audioSettings;
        
        protected virtual void Awake()
        {
            Assert.IsNotNull(audioSource,
                "SCILLPersonalChallengeAudio on object " + gameObject.name + " is missing the audioSource.");
            Assert.IsNotNull(audioSettings,
                "SCILLPersonalChallengeAudio on object " + gameObject.name + " is missing audioSettings.");
        }
        
        protected virtual void Play(AudioClip feedbackAudioClip)
        {
            if (feedbackAudioClip && audioSource)
            {
                audioSource.PlayOneShot(feedbackAudioClip);
            }
        }
    }
}