using System;
using SCILL.Model;
using UnityEngine;

namespace SCILL.Effects
{
    /// <summary>
    /// Sample script for playing feedback audio clips on Personal challenge realtime events. 
    /// Uses the data provided by the <see cref="AudioSettings"/> scriptable object to play sound effects for events invoked by the <see cref="SCILLManager"/>s Realtime Update Notifications.
    /// </summary>
    public class SCILLPersonalChallengeAudio : SCILLAudioBase
    {
        protected virtual void OnEnable()
        {
            SCILLManager.Instance.StartChallengeUpdateNotifications(OnChallengeUpdated);
        }

        protected virtual void OnDisable()
        {
            SCILLManager.Instance.StopChallengeUpdateNotifications(OnChallengeUpdated);
        }

        /// <summary>
        /// Invoked when a realtime update notification was received from the SCILL server. Will attempt to retrieve
        /// an audio clip from the <see cref="AudioSettings"/> object based on the Challenge <see cref="Challenge.type"/>
        /// supplied by the <see cref="payload"/>.
        /// <remarks>
        /// Will only play a clip on type changes.
        /// </remarks>
        /// </summary>
        /// <param name="payload">The realtime Personal Challenge update payload.</param>
        protected virtual void OnChallengeUpdated(ChallengeWebhookPayload payload)
        {
            if (!audioSettings)
                return;

            Challenge oldChallenge = payload.old_challenge;
            Challenge newChallenge = payload.new_challenge;
            bool typeChanged = oldChallenge.type != newChallenge.type;
            if (typeChanged)
            {
                AudioClip feedbackAudioClip = audioSettings.GetChallengeAudioFromType(newChallenge.type);
                Play(feedbackAudioClip);
            }
            else
            {
                Play(audioSettings.ChallengeUpdatedSound);
            }
        }
    }
}