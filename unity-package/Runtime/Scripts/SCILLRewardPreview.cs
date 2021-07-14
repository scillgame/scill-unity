using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         This component will listen to the <see cref="SCILLBattlePassManager.OnSelectedBattlePassLevelChanged" /> event
    ///         to get notified whenever the user clicks on a <see cref="SCILLBattlePassLevel" /> component.
    ///     </para>
    ///     <para>
    ///         When a reward is set for this level (<c>reward_amount</c> of the <see cref="BattlePass" /> object) it will try
    ///         to load <see cref="SCILLReward" />
    ///         asset with that name. If the reward is available it will display its UI and will set image, name and
    ///         description
    ///         on connected UI elements.
    ///     </para>
    ///     <para>
    ///         The reward preview will instantiate the prefab set in the <see cref="SCILLReward" /> asset as child into this
    ///         GameObject. The
    ///         default preview has this hierarchy:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>Reward photo box</item>
    ///         <item>Reward camera</item>
    ///         <item>Photo Box</item>
    ///     </list>
    ///     <para>
    ///         <c>Photo Box</c> is connected to the <c>photoBox</c> property and models will be instantiated into this
    ///         transform. The
    ///         reward camera renders the model into a Render Texture that is used in the Reward Preview UI as a RawImage
    ///         field.
    ///     </para>
    ///     <para>
    ///         <b>Important:</b> You need to set the layer that the reward camera renders to the same layer that you set in
    ///         the prefab. This layer should be included from the main games camera.
    ///     </para>
    ///     <para>
    ///         As every game has a different layer structure we cannot set a specific layer for SCILL.
    ///     </para>
    /// </summary>
    public class SCILLRewardPreview : MonoBehaviour
    {
        /// <summary>
        ///     Connect to a text field to render the name of the reward as set in the <see cref="SCILLReward" /> asset.
        /// </summary>
        [Header("Required connections")] [Tooltip("Connect to a text field to render the reward")]
        public Text rewardName;

        /// <summary>
        ///     Connect to a text field to render the description of the reward as set in the <see cref="SCILLReward" /> asset.
        /// </summary>
        [Tooltip("Connect to a text field to render a description of the reward")]
        public Text rewardDescription;

        /// <summary>
        ///     Connect to an object which should have a Button attached. This item is hidden unless the reward can be claimed
        ///     and has not been yet claimed. If set, the buttons click event will call
        ///     <see cref="OnClaimButtonPassRewardButtonClicked" /> so that
        ///     the reward is claimed.
        /// </summary>
        [Tooltip(
            "Connect to a claim button which should have a Button attached. This item is hidden unless the reward can be claimed and has not been yet claimed")]
        public GameObject claimButton;

        /// <summary>
        ///     Set a container object that will be used as the parent for the 3D model preview prefabs. See Overview for more
        ///     details on this topic.
        /// </summary>
        [Header("Optional connections")]
        [Tooltip("Connect to a Reward Photobox which will be used to render a 3D representation of the reward")]
        public GameObject photoBox;

        private GameObject _rewardModel;

        private SCILLReward _scillReward;
        private BattlePassLevel _selectedBattlePassLevel;

        private void Start()
        {
            ToggleUI(false);

            if (claimButton)
            {
                var button = claimButton.GetComponent<Button>();
                if (button && button.onClick.GetPersistentEventCount() <= 0)
                    button.onClick.AddListener(OnClaimButtonPassRewardButtonClicked);
            }
        }

        private void OnEnable()
        {
            if (SCILLBattlePassManager.Instance)
                OnSelectedBattlePassLevelChanged(SCILLBattlePassManager.Instance.SelectedBattlePassLevel);

            SCILLBattlePassManager.OnSelectedBattlePassLevelChanged += OnSelectedBattlePassLevelChanged;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed += OnBattlePassLevelRewardClaimed;
        }

        private void OnDisable()
        {
            SCILLBattlePassManager.OnSelectedBattlePassLevelChanged -= OnSelectedBattlePassLevelChanged;
            SCILLBattlePassManager.OnBattlePassLevelRewardClaimed -= OnBattlePassLevelRewardClaimed;
        }

        private void OnSelectedBattlePassLevelChanged(BattlePassLevel selectedBattlePassLevel)
        {
            // Debug.Log("SELECT BATTLE PASS LEVEL: " + selectedBattlePassLevel?.level_id);
            _selectedBattlePassLevel = selectedBattlePassLevel;
            if (selectedBattlePassLevel?.reward_amount != null)
                SetRewardId(selectedBattlePassLevel.reward_amount);
            else
                ToggleUI(false);
        }

        private void OnBattlePassLevelRewardClaimed(BattlePassLevel level)
        {
            if (level.level_id == _selectedBattlePassLevel.level_id)
                _selectedBattlePassLevel.reward_claimed = level.reward_claimed;

            UpdateScillReward();
        }

        private void SetRewardId(string rewardId)
        {
            _scillReward = Resources.Load<SCILLReward>(rewardId);
            if (_scillReward)
            {
                UpdateScillReward();
                ToggleUI(true);
            }
            else
            {
                // No reward found
                ToggleUI(false);
            }
        }

        private void UpdateScillReward()
        {
            if (_rewardModel) DestroyImmediate(_rewardModel);

            if (_scillReward.prefab && photoBox)
                _rewardModel = Instantiate(_scillReward.prefab, photoBox.transform);
            //_rewardModel.transform.localPosition = Vector3.zero;

            if (rewardDescription) rewardDescription.text = _scillReward.description;

            if (rewardName) rewardName.text = _scillReward.name;

            if (claimButton)
            {
                if (_selectedBattlePassLevel.activated_at == null || _selectedBattlePassLevel.level_completed == false)
                {
                    claimButton.SetActive(false);
                }
                else
                {
                    if (_selectedBattlePassLevel.reward_claimed == false)
                        claimButton.SetActive(true);
                    else
                        claimButton.SetActive(false);
                }
            }
        }

        private void ToggleUI(bool show)
        {
            transform.GetChild(0).gameObject.SetActive(show);
        }

        /// <summary>
        ///     Should be called when the claim button was clicked. Will call
        ///     <see cref="SCILLBattlePassManager.ClaimBattlePassLevelReward" /> to notify SCILL that the reward was claimed.
        /// </summary>
        public void OnClaimButtonPassRewardButtonClicked()
        {
            SCILLBattlePassManager.Instance.ClaimBattlePassLevelReward(_selectedBattlePassLevel);
        }
    }
}