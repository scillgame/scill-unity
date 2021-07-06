using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    public class SCILLChallengeItem : MonoBehaviour
    {
        public Text challengeName;
        public Image challengeImage;
        public Slider challengeProgressSlider;
        public Text challengeGoal;
        public RectTransform challengeProgress;
        public Text timeRemaining;
        public RectTransform actions;
        public Button unlockButton;
        public Button activateButton;
        public Button claimButton;
        public Button cancelButton;

        public Challenge challenge;

        // Start is called before the first frame update
        void Start()
        {
            if (challenge == null)
            {
                return;
            }

            // Set the challange icon by loading from the Resources folder
            if (challengeImage)
            {
                if (!string.IsNullOrEmpty(challenge.challenge_icon))
                {
                    // Load a sprite with name challenge_icon from resources folder
                    Sprite sprite = Resources.Load<Sprite>(challenge.challenge_icon);
                    challengeImage.sprite = sprite;
                    challengeImage.gameObject.SetActive(true);
                }
                else
                {
                    challengeImage.gameObject.SetActive(false);
                }
            }
        }

        private string StrikeThrough(string s)
        {
            string strikethrough = "";
            foreach (char c in s)
            {
                strikethrough = strikethrough + c + '\u0336';
            }

            return strikethrough;
        }

        // Update is called once per frame
        void Update()
        {
            if (challenge == null)
            {
                return;
            }

            if (challengeName) challengeName.text = challenge.challenge_name;
            if (challenge.challenge_goal > 0)
            {
                if (challengeProgressSlider)
                {
                    challengeProgressSlider.value = (float) ((float) challenge.user_challenge_current_score /
                                                             (float) challenge.challenge_goal);
                }
            }

            if (challengeGoal)
            {
                challengeGoal.text = challenge.user_challenge_current_score.ToString() + "/" +
                                     challenge.challenge_goal.ToString();
            }

            if (challenge.type == "unlock")
            {
                if (actions) actions.gameObject.SetActive(true);
                if (unlockButton) unlockButton.gameObject.SetActive(true);
                if (activateButton) activateButton.gameObject.SetActive(false);
                if (claimButton) claimButton.gameObject.SetActive(false);
                if (cancelButton) cancelButton.gameObject.SetActive(false);
                if (challengeProgress) challengeProgress.gameObject.SetActive(false);
            }
            else if (challenge.type == "unlocked")
            {
                if (actions) actions.gameObject.SetActive(true);
                if (unlockButton) unlockButton.gameObject.SetActive(false);
                if (activateButton) activateButton.gameObject.SetActive(true);
                if (claimButton) claimButton.gameObject.SetActive(false);
                if (cancelButton) cancelButton.gameObject.SetActive(false);
                if (challengeProgress) challengeProgress.gameObject.SetActive(false);
            }
            else if (challenge.type == "in-progress")
            {
                if (challengeProgress) challengeProgress.gameObject.SetActive(true);
                if (actions) actions.gameObject.SetActive(true);
                if (unlockButton) unlockButton.gameObject.SetActive(false);
                if (activateButton) activateButton.gameObject.SetActive(false);
                if (claimButton) claimButton.gameObject.SetActive(false);
                if (cancelButton) cancelButton.gameObject.SetActive(true);

                var timeText = "";
                var date = DateTime.Parse(challenge.user_challenge_activated_at);
                date = date.AddMinutes((double) challenge.challenge_duration_time);

                var now = DateTime.Now;
                var diff = date.Subtract(now);

                if (diff.Days > 0)
                {
                    timeText = "+24 hours";
                }
                else
                {
                    timeText = String.Format("{0:00}:{1:00}:{2:00}", diff.Hours, diff.Minutes, diff.Seconds);
                }

                if (timeRemaining) timeRemaining.text = timeText;
            }
            else if (challenge.type == "unclaimed")
            {
                if (challengeName) challengeName.text = StrikeThrough(challenge.challenge_name);
                if (challengeProgress) challengeProgress.gameObject.SetActive(false);
                if (actions) actions.gameObject.SetActive(true);
                if (claimButton) claimButton.gameObject.SetActive(true);
                if (unlockButton) unlockButton.gameObject.SetActive(false);
                if (activateButton) activateButton.gameObject.SetActive(false);
                if (cancelButton) cancelButton.gameObject.SetActive(false);
            }
            else if (challenge.type == "finished")
            {
                if (challengeName) challengeName.text = StrikeThrough(challenge.challenge_name);
                if (challengeProgress) challengeProgress.gameObject.SetActive(false);
                if (actions) actions.gameObject.SetActive(false);
                if (claimButton) claimButton.gameObject.SetActive(false);
                if (unlockButton) unlockButton.gameObject.SetActive(false);
                if (activateButton) activateButton.gameObject.SetActive(false);
                if (cancelButton) cancelButton.gameObject.SetActive(false);
            }
            else
            {
                if (challengeProgress) challengeProgress.gameObject.SetActive(false);
                if (actions) actions.gameObject.SetActive(false);
            }
        }

        public void OnUnlockButtonPressed()
        {
            var personalChallenges = GetComponentInParent<SCILLPersonalChallenges>();
            if (personalChallenges)
            {
                personalChallenges.UnlockPersonalChallenge(challenge);
            }
            else
            {
                Debug.LogError("PersonalChallenge component not found in the parents");
            }
        }

        public void OnActivateButtonPressed()
        {
            var personalChallenges = GetComponentInParent<SCILLPersonalChallenges>();
            if (personalChallenges)
            {
                personalChallenges.ActivatePersonalChallenge(challenge);
            }
            else
            {
                Debug.LogError("PersonalChallenge component not found in the parents");
            }
        }

        public void OnClaimButtonPressed()
        {
            var personalChallenges = GetComponentInParent<SCILLPersonalChallenges>();
            if (personalChallenges)
            {
                personalChallenges.ClaimPersonalChallengeReward(challenge);
            }
            else
            {
                Debug.LogError("PersonalChallenge component not found in the parents");
            }
        }

        public void OnCancelButtonPressed()
        {
            var personalChallenges = GetComponentInParent<SCILLPersonalChallenges>();
            if (personalChallenges)
            {
                personalChallenges.CancelPersonalChallenge(challenge);
            }
            else
            {
                Debug.LogError("PersonalChallenge component not found in the parents");
            }
        }
    }

}

