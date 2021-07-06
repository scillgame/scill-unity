using System;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     This class implements the user interface for a personal challenge. Attach it to a game object and connect
    ///     properties
    ///     with Unity UI elements. You need to create a prefab with this script and connect it to the
    ///     <see cref="SCILLPersonalChallenges.challengePrefab" /> in the
    ///     <see cref="SCILLPersonalChallenges" /> component.
    /// </summary>
    public class SCILLChallengeItem : MonoBehaviour
    {
        /// <summary>
        ///     Connect a <c>UnityEngine.UI.Text</c> component which will be set with the <c>challenge_name</c> of the
        ///     <see cref="Challenge" /> object.
        /// </summary>
        [Tooltip(
            "Connect a UnityEngine.UI.Text component which will be set with the challenge_name of the Challenge object.")]
        public Text challengeName;

        /// <summary>
        ///     The <see cref="Challenge" /> has a <c>challenge_icon</c> setting. This is a string value that you can set in the
        ///     Admin Panel. The class
        ///     will try to load a sprite with the same name from your Asset database and will set that as the sprite of the
        ///     connected <c>UnityEngine.UI.Image</c> class.
        /// </summary>
        /// <remarks>
        ///     Please note: The sprite is loaded at runtime and must be within a <c>Resources</c> folder in your projects Assets
        ///     folder.
        /// </remarks>
        [Tooltip(
            "The Challenge has a challenge_icon setting. This is a string value that you can set in the Admin Panel. The class will try to load a sprite with the same name from your Asset database and will set that as the sprite of the connected UnityEngine.UI.Image class.")]
        public Image challengeImage;

        /// <summary>
        ///     The challenge progress will be set in this <c>UnityEngine.UI.Slider</c> component that you can connect to this
        ///     property. Remove the handle from the slider as user interaction is not required.
        /// </summary>
        [Tooltip(
            "The challenge progress will be set in this UnityEngine.UI.Slider component that you can connect to this property. Remove the handle from the slider as this is not required.")]
        public Slider challengeProgressSlider;

        /// <summary>
        ///     Connect a <c>UnityEngine.UI.Text</c> component which will be set with the <c>challenge_name</c> of the
        ///     <see cref="Challenge" /> object.
        /// </summary>
        [Tooltip(
            "Connect a UnityEngine.UI.Text component which will be set with the challenge_name of the Challenge object.")]
        public Text challengeGoal;

        /// <summary>
        ///     Connect a 2D Transform that will host the <see cref="challengeProgressSlider" />. It will be set inactive if the
        ///     challenge is not active to hide the progress bar and will be set to active otherwise.
        /// </summary>
        [Tooltip(
            "Connect a 2D Transform that will host the challengeProgressSlider. It will be set inactive if the challenge is not active to hide the progress bar and will be set to active otherwise.")]
        public RectTransform challengeProgress;

        /// <summary>
        ///     Connect a <c>UnityEngine.UI.Text</c> component which will be used to set the remaining time of the challenge. Per
        ///     default this will default to this format: <c>mm:hh:ss</c>.
        /// </summary>
        [Tooltip(
            "Connect a UnityEngine.UI.Text component which will be used to set the remaining time of the challenge. Per default this will default to this format: mm:hh:ss.")]
        public Text timeRemaining;

        public RectTransform actions;
        public Button unlockButton;
        public Button activateButton;
        public Button claimButton;
        public Button cancelButton;

        public Challenge challenge;

        // Start is called before the first frame update
        private void Start()
        {
            if (challenge == null) return;

            // Set the challange icon by loading from the Resources folder
            if (challengeImage)
            {
                if (!string.IsNullOrEmpty(challenge.challenge_icon))
                {
                    // Load a sprite with name challenge_icon from resources folder
                    var sprite = Resources.Load<Sprite>(challenge.challenge_icon);
                    challengeImage.sprite = sprite;
                    challengeImage.gameObject.SetActive(true);
                }
                else
                {
                    challengeImage.gameObject.SetActive(false);
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (challenge == null) return;

            if (challengeName) challengeName.text = challenge.challenge_name;
            if (challenge.challenge_goal > 0)
                if (challengeProgressSlider)
                    challengeProgressSlider.value = (float) challenge.user_challenge_current_score /
                                                    (float) challenge.challenge_goal;

            if (challengeGoal)
                challengeGoal.text = challenge.user_challenge_current_score + "/" +
                                     challenge.challenge_goal;

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
                    timeText = "+24 hours";
                else
                    timeText = string.Format("{0:00}:{1:00}:{2:00}", diff.Hours, diff.Minutes, diff.Seconds);

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

        private string StrikeThrough(string s)
        {
            var strikethrough = "";
            foreach (var c in s) strikethrough = strikethrough + c + '\u0336';

            return strikethrough;
        }

        public void OnUnlockButtonPressed()
        {
            var personalChallenges = GetComponentInParent<SCILLPersonalChallenges>();
            if (personalChallenges)
                personalChallenges.UnlockPersonalChallenge(challenge);
            else
                Debug.LogError("PersonalChallenge component not found in the parents");
        }

        public void OnActivateButtonPressed()
        {
            var personalChallenges = GetComponentInParent<SCILLPersonalChallenges>();
            if (personalChallenges)
                personalChallenges.ActivatePersonalChallenge(challenge);
            else
                Debug.LogError("PersonalChallenge component not found in the parents");
        }

        public void OnClaimButtonPressed()
        {
            var personalChallenges = GetComponentInParent<SCILLPersonalChallenges>();
            if (personalChallenges)
                personalChallenges.ClaimPersonalChallengeReward(challenge);
            else
                Debug.LogError("PersonalChallenge component not found in the parents");
        }

        public void OnCancelButtonPressed()
        {
            var personalChallenges = GetComponentInParent<SCILLPersonalChallenges>();
            if (personalChallenges)
                personalChallenges.CancelPersonalChallenge(challenge);
            else
                Debug.LogError("PersonalChallenge component not found in the parents");
        }
    }
}