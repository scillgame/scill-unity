using System;
using SCILL.Model;
using UnityEngine;

namespace SCILL.Effects
{
    public class SCILLPersonalChallengeAudio : SCILLAudioBase
    {
        protected void OnEnable()
        {
            SCILLManager.Instance.StartChallengeUpdateNotifications(OnChallengeUpdated);
        }

        protected void OnDisable()
        {
            SCILLManager.Instance.StopChallengeUpdateNotifications(OnChallengeUpdated);
        }

        protected void OnChallengeUpdated(ChallengeWebhookPayload payload)
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

        private void Play(AudioClip feedbackAudioClip)
        {
            if (feedbackAudioClip && audioSource)
            {
                audioSource.PlayOneShot(feedbackAudioClip);
            }
        }
    }
}