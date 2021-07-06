using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         This component will handle UI for a challenge of a battle pass level. It will update progress whenever the
    ///         challenge progress changes.
    ///     </para>
    ///     <para>
    ///         It listens on the <see cref="SCILLBattlePassManager.OnBattlePassChallengeUpdate" /> delegate of the
    ///         <see cref="SCILLBattlePassManager" /> and will update the
    ///         progress accordingly.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     This class is intended to be used with the <see cref="SCILLBattlePassLevels" /> component (set as prefab for a
    ///     challenge). As
    ///     <see cref="SCILLBattlePassLevels" /> does hide all challenges that are not in progress, this class does not handle
    ///     challenges that
    ///     are not <c>in-progress correctly</c>, i.e. the progress bar is always visible, as the goal. If you want to
    ///     implement
    ///     special behavior, derive from this class and override the <see cref="UpdateUI" /> function to adjust UI
    ///     accordingly.
    /// </remarks>
    public class SCILLBattlePassChallengeItem : MonoBehaviour
    {
        /// <summary>
        ///     Text Field that is used to render the challenge name.
        /// </summary>
        [Tooltip("Text UI that is used to render the challenge name")]
        public Text challengeName;

        /// <summary>
        ///     Progress slider that is used to show the progress of the challenge. The handle of the slider should be removed as
        ///     this is a non-interactive
        ///     display of the current challenges progress. The slider will be automatically updated whenever the challenge
        ///     progress
        ///     changes.
        /// </summary>
        [Tooltip("Progress slider that is used to show the progress of the challenge")]
        public Slider challengeProgressSlider;

        /// <summary>
        ///     Text field used to display the challenge progress and goal in the format {current}/{goal}, e.g. 34/500.
        /// </summary>
        [Tooltip("Text used to render the challenge goal as 34/500")]
        public Text challengeGoal;

        /// <summary>
        ///     The <see cref="BattlePassLevelChallenge" /> has a <c>challenge_icon</c> setting. This is a string value that you
        ///     can set in the Admin Panel. The class will try to load a sprite with the same name from your Asset database and
        ///     will set that as the sprite of the connected UnityEngine.UI.Image class.
        /// </summary>
        /// <remarks>
        ///     The sprite is loaded at runtime and must be within a <c>Resources</c> folder in your Asset database so that Unity
        ///     exposes that asset so that it can be loaded dynamically.
        /// </remarks>
        [Tooltip("An image component that will be used to render the challenge icon")]
        public Image challengeIcon;

        public BattlePassLevelChallenge challenge;

        // Start is called before the first frame update
        private void Start()
        {
            UpdateUI();
        }

        private void OnEnable()
        {
            SCILLBattlePassManager.OnBattlePassChallengeUpdate += OnBattlePassChallengeUpdate;
        }

        private void OnDestroy()
        {
            SCILLBattlePassManager.OnBattlePassChallengeUpdate -= OnBattlePassChallengeUpdate;
        }

        private void OnBattlePassChallengeUpdate(BattlePassChallengeChangedPayload challengeChangedPayload)
        {
            // Update local challenge object if it has the same id
            if (challengeChangedPayload.new_battle_pass_challenge.challenge_id == challenge.challenge_id)
            {
                challenge.type = challengeChangedPayload.new_battle_pass_challenge.type;
                challenge.user_challenge_current_score =
                    challengeChangedPayload.new_battle_pass_challenge.user_challenge_current_score;
                UpdateUI();
            }
        }

        /// <summary>
        ///     Trigger a UI update. This is done automatically whenever the challenge changes. However, you might also want to trigger an
        ///     update of the UI manually.
        /// </summary>
        public virtual void UpdateUI()
        {
            if (challenge == null) return;

            // Update name of challenge
            if (challengeName) challengeName.text = challenge.challenge_name;

            // Update progress slider
            if (challengeProgressSlider)
            {
                if (challenge.challenge_goal > 0 && challenge.user_challenge_current_score > 0)
                {
                    if (challenge.challenge_goal_condition == 0)
                        // If the challenge_goal_condition is 0 then counter must be greater than the goal, so progress is the
                        // relation between the counter and the goal

                        challengeProgressSlider.value = (float) challenge.user_challenge_current_score /
                                                        (float) challenge.challenge_goal;
                    else if (challenge.challenge_goal_condition == 1)
                        // If the challenge_goal_condition is 1 then counter must be smaller than the goal, so progress is the
                        // inverted relation between the counter and the goal

                        challengeProgressSlider.value = 1.0f / ((float) challenge.user_challenge_current_score /
                                                                (float) challenge.challenge_goal);
                }
                else
                {
                    challengeProgressSlider.value = 0;
                }
            }

            if (challengeGoal)
                challengeGoal.text = challenge.user_challenge_current_score + "/" +
                                     challenge.challenge_goal;

            if (challengeIcon)
            {
                var sprite = Resources.Load<Sprite>(challenge.challenge_icon);
                if (sprite) challengeIcon.sprite = sprite;
            }
        }
    }
}