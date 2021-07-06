using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

namespace SCILL
{
    public enum SCILLPersonalChallengeModificationType
    {
        Unknown,
        Unlocked,
        Activated,
        Completed,
        Claimed,
        Progress
    }

    public class SCILLPersonalChallengesManager : MonoBehaviour
    {
        public static SCILLPersonalChallengesManager Instance { get; private set; }

        public List<ChallengeCategory> Categories { get; private set; }

        public delegate void PersonalChallengesUpdatedFromServerAction(List<ChallengeCategory> categories);

        public static event PersonalChallengesUpdatedFromServerAction OnPersonalChallengesUpdatedFromServer;

        public delegate void PersonalChallengeUpdatedFromServerAction(Challenge challenge,
            SCILLPersonalChallengeModificationType modificationType);

        public static event PersonalChallengeUpdatedFromServerAction OnPersonalChallengeUpdatedFromServer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            SCILLManager.OnSCILLManagerReady += OnSCILLManagerReady;
        }

        private void OnSCILLManagerReady()
        {
            UpdatePersonalChallengesList();

            SCILLManager.Instance.StartChallengeUpdateNotifications(OnChallengeWebhookMessage);
        }

        private void OnDestroy()
        {
            if (SCILLManager.Instance)
                SCILLManager.Instance.StopChallengeUpdateNotifications(OnChallengeWebhookMessage);
        }

        private Challenge FindChallengeById(string id)
        {
            foreach (var category in Categories)
            {
                foreach (var challenge in category.challenges)
                {
                    if (challenge.challenge_id == id)
                    {
                        return challenge;
                    }
                }
            }

            return null;
        }

        public void UpdatePersonalChallengesList()
        {
            var categoriesPromise = SCILLManager.Instance.SCILLClient.GetAllPersonalChallengesAsync();
            categoriesPromise.Then(response =>
            {
                Categories = response;
                OnPersonalChallengesUpdatedFromServer?.Invoke(Categories);
            });
        }

        private void UpdateChallenge(ChallengeWebhookPayload payload)
        {
            Challenge challenge = FindChallengeById(payload.new_challenge.challenge_id);
            if (challenge != null)
            {
                challenge.type = payload.new_challenge.type;
                challenge.user_challenge_current_score = payload.new_challenge.user_challenge_current_score;
                challenge.user_challenge_activated_at = payload.new_challenge.user_challenge_activated_at;
                challenge.user_challenge_unlocked_at = payload.new_challenge.user_challenge_unlocked_at;

                SCILLPersonalChallengeModificationType type = SCILLPersonalChallengeModificationType.Unknown;
                if (payload.old_challenge.type != payload.new_challenge.type)
                {
                    if (payload.new_challenge.type == "unlocked")
                    {
                        type = SCILLPersonalChallengeModificationType.Unlocked;
                    }
                    else if (payload.new_challenge.type == "in-progress")
                    {
                        type = SCILLPersonalChallengeModificationType.Activated;
                    }
                    else if (payload.new_challenge.type == "finished")
                    {
                        type = SCILLPersonalChallengeModificationType.Completed;
                    }
                }
                else
                {
                    if (payload.new_challenge.user_challenge_current_score >
                        payload.old_challenge.user_challenge_current_score)
                    {
                        type = SCILLPersonalChallengeModificationType.Progress;
                    }
                }

                OnPersonalChallengeUpdatedFromServer?.Invoke(challenge, type);
            }
        }

        private void OnChallengeWebhookMessage(ChallengeWebhookPayload payload)
        {
            UpdateChallenge(payload);
        }
    }
}