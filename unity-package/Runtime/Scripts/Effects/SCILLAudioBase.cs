using UnityEngine;
using UnityEngine.Assertions;

namespace SCILL.Effects
{
    /// <summary>
    /// <para>
    /// Abstract base class for SCILL Audio behaviours, provides access to an Unity <c>AudioSource</c> and to a <see cref="SCILLAudioSettings"/> scriptable object.
    /// </para>
    /// <para>
    /// You can use this to implement your own SCILL related audio behavior - example
    /// implementations for playing feedback on Personal Challenge related, Battle Pass related or Leaderboard related realtime events can be seen in the
    /// <see cref="SCILLPersonalChallengeAudio"/>, <see cref="SCILLBattlePassAudio"/> and <see cref="SCILLLeaderboardAudio"/> scripts.
    /// </para>
    /// </summary>
    public abstract class SCILLAudioBase : MonoBehaviour
    {
        /// <summary>
        /// Audiosource to play feedback from.
        /// </summary>
        [SerializeField] protected AudioSource audioSource;
        /// <summary>
        /// The audio settings containing e.g. the audio clips that should be played for certain SCILL related realtime events.
        /// </summary>
        [SerializeField] protected SCILLAudioSettings audioSettings;
        
        protected virtual void Awake()
        {
            Assert.IsNotNull(audioSource,
                "SCILLPersonalChallengeAudio on object " + gameObject.name + " is missing the audioSource.");
            Assert.IsNotNull(audioSettings,
                "SCILLPersonalChallengeAudio on object " + gameObject.name + " is missing audioSettings.");
        }
        
        /// <summary>
        /// Plays the given <see cref="feedbackAudioClip"/> using the <see cref="audioSource"/>.
        /// </summary>
        /// <param name="feedbackAudioClip">The <c>AudioClip</c> to play.</param>
        protected virtual void Play(AudioClip feedbackAudioClip)
        {
            if (feedbackAudioClip && audioSource)
            {
                audioSource.PlayOneShot(feedbackAudioClip);
            }
        }
    }
}