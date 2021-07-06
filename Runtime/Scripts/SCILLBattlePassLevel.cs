using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     Implements a level component in the Battle Pass User Interface. It will embed
    ///     <see cref="SCILLBattlePassRewardIcon" /> prefabs to
    ///     show available rewards for this level. Create a prefab with this component attached and set as the
    ///     <c>levelPrefab</c>,
    ///     <c>currentLevelPrefab</c> and <c>lockedLevelPrefab</c> in the <see cref="SCILLBattlePassLevels" /> component.
    /// </summary>
    public class SCILLBattlePassLevel : MonoBehaviour
    {
        /// <summary>
        ///     A transform object in that the reward icons will be instantiated. If this is not set, the reward icon prefabs will
        ///     be instantiated as child of this components transform.
        /// </summary>
        [Header("Required connections")] [Tooltip("A transform object in that the reward icons will be instantiated")]
        public Transform rewardContainer;

        /// <summary>
        ///     Set a game object that will be hidden if <c>showLevelInfo</c> of the <see cref="SCILLBattlePassLevels" /> script
        ///     will be false. It typically shows the level number which is rendered in the <c>levelName</c> Text field.
        /// </summary>
        [Header("Optional connections")]
        [Tooltip(
            "Set a game object that will be hidden if showLevelInfo of the SCILLBattlePass script will be false. It typical indicates the level number")]
        public GameObject battlePassLevelInfo;

        /// <summary>
        ///     A textfield that will be used to render the level number.
        /// </summary>
        [Tooltip("A textfield that will be used to render the level number")]
        public Text levelName;

        /// <summary>
        ///     Specify the formatting of the level name. Insert {level} to display the level number.
        /// </summary>
        [Tooltip("Set the level-name formatting, insert {level} to display the level number")]
        public string levelTextFormat = "Level {level}";

        /// <summary>
        ///     A prefab that will be used as a reward icon. It must have a <see cref="SCILLBattlePassRewardIcon" /> component
        ///     attached to the
        ///     root object.
        /// </summary>
        [Header("Prefabs")] [Tooltip("A prefab that will be used as a reward icon")]
        public SCILLBattlePassRewardIcon rewardIconPrefab;

        /// <summary>
        ///     A slider that will be used to render the current progress in this level. Remove the handle so that users cannot
        ///     change the value as this is only a progress bar without user interaction.
        /// </summary>
        [Tooltip("A slider that will be used to render the current progress in this level")]
        public Slider progressSlider;

        /// <summary>
        ///     Automatically disable progress slider if value is 0 or 1.
        /// </summary>
        [Tooltip("Automatically disable progress slider if value is 0 or 1.")]
        public bool autoHideProgressSlider;

        /// <summary>
        ///     A textfield which will display a textual representation of the current level progress, e.g. it will be set with a
        ///     string like 2/4 - which is 2 of 4 challenges completed.
        /// </summary>
        [Tooltip("A text ui object that will be set with a string like 2/4 - which is 2 of 4 challenges completed.")]
        public Text challengeText;

        /// <summary>
        ///     If true, <see cref="battlePassLevelInfo" /> will be shown; otherwise it will remain inactive.
        /// </summary>
        [HideInInspector] public bool showLevelInfo = true;

        [HideInInspector] public Button button;

        private BattlePassLevel _battlePassLevel;

        private SCILLReward _reward;

        public BattlePassLevel battlePassLevel
        {
            get => _battlePassLevel;
            set
            {
                _battlePassLevel = value;
                UpdateUI();
            }
        }

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            UpdateUI();
        }

        private void OnEnable()
        {
            SCILLBattlePassManager.OnBattlePassChallengeUpdate += OnBattlePassChallengeUpdate;
            SCILLBattlePassManager.OnSelectedBattlePassLevelChanged += OnSelectedBattlePassLevelChanged;
            UpdateUI();
        }

        private void OnDestroy()
        {
            SCILLBattlePassManager.OnBattlePassChallengeUpdate -= OnBattlePassChallengeUpdate;
            SCILLBattlePassManager.OnSelectedBattlePassLevelChanged -= OnSelectedBattlePassLevelChanged;
        }

        private void OnSelectedBattlePassLevelChanged(BattlePassLevel selectedBattlePassLevel)
        {
            if (battlePassLevel == null || selectedBattlePassLevel == null)
            {
                Deselect();
            }
            else
            {
                if (battlePassLevel.level_id == selectedBattlePassLevel.level_id)
                    Select();
                else
                    Deselect();
            }
        }

        private void OnBattlePassChallengeUpdate(BattlePassChallengeChangedPayload challengeChangedPayload)
        {
            UpdateUI();
        }

        private void ClearRewardIcons()
        {
            foreach (var child in GetComponentsInChildren<SCILLBattlePassRewardIcon>()) Destroy(child.gameObject);
        }

        /// <summary>
        ///     Trigger a UI update. That is done automatically whenever the level or a challenge within that level changes to
        ///     update progress bar and state. However, you may want to trigger an update in between. It will rebuild the hierarchy
        ///     for the reward icons.
        /// </summary>
        public void UpdateUI()
        {
            if (battlePassLevel == null) return;

            ClearRewardIcons();

            if (rewardContainer && rewardIconPrefab && battlePassLevel.reward_type_name != "Nothing" &&
                !string.IsNullOrEmpty(battlePassLevel.reward_amount))
            {
                var rewardIcon = Instantiate(rewardIconPrefab, rewardContainer, false);
                var rewardIconScript = rewardIcon.GetComponentInChildren<SCILLBattlePassRewardIcon>();
                if (rewardIconScript) rewardIconScript.battlePassLevel = battlePassLevel;
            }

            if (levelName)
                levelName.text = levelTextFormat.Replace("{level}", battlePassLevel.level_priority.ToString());

            if (battlePassLevelInfo) battlePassLevelInfo.SetActive(showLevelInfo);

            float totalProgress = 0;
            var activatedChallenges = 0;
            foreach (var challenge in battlePassLevel.challenges)
            {
                float challengeProgress = 0;
                if (challenge.challenge_goal > 0)
                    challengeProgress = (float) challenge.user_challenge_current_score /
                                        (float) challenge.challenge_goal;

                totalProgress += challengeProgress * (1.0f / battlePassLevel.challenges.Count);

                if (challenge.user_challenge_current_score >= challenge.challenge_goal) activatedChallenges += 1;
            }

            // Update slider
            if (progressSlider)
            {
                progressSlider.value = totalProgress;

                if (autoHideProgressSlider)
                {
                    if (totalProgress <= 0)
                        progressSlider.gameObject.SetActive(false);
                    else
                        progressSlider.gameObject.SetActive(true);
                }

                if (battlePassLevel.level_priority <= 1) progressSlider.gameObject.SetActive(false);
            }

            if (challengeText) challengeText.text = activatedChallenges + "/" + battlePassLevel.challenges.Count;

            if (SCILLBattlePassManager.Instance.SelectedBattlePassLevel?.battle_pass_id ==
                battlePassLevel.level_id) Select();
        }

        /// <summary>
        ///     This class does nothing if the level is selected. However, you may want to implement some sort of selection rect or
        ///     a glowing effect for the selected level. This class is called whenever this level is selected.
        /// </summary>
        public virtual void Select()
        {
            var outline = GetComponentInChildren<Outline>();
            if (outline) outline.enabled = true;
        }

        /// <summary>
        ///     This class does nothing if the level is deselected. However, you may want to implement some sort of selection rect
        ///     or a glowing effect for the selected level. This class is called whenever this level is deselected.
        /// </summary>
        public virtual void Deselect()
        {
            var outline = GetComponentInChildren<Outline>();
            if (outline) outline.enabled = false;
        }
    }
}