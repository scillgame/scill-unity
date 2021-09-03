using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         Battle Passes have a number of levels and each level has challenges attached that need to be achieved before
    ///         the
    ///         level is completed and the next level is unlocked.
    ///     </para>
    ///     <para>
    ///         Use this component to render a list of active challenges, i.e. the challenges from the current level. Set the
    ///         <c>challengePrefab</c> to a prefab that has the <see cref="SCILLBattlePassChallengeItem" /> item attached.
    ///     </para>
    ///     <para>
    ///         Only active challenges will be rendered, i.e. those challenges that have a <c>in-progress</c> type. It is
    ///         intended to
    ///         be
    ///         shown in the games HUD and to be always visible, so gamers can quickly see what they should do next. However,
    ///         you
    ///         can override this class and implement basic logic to your liking, too.
    ///     </para>
    /// </summary>
    public class SCILLBattlePassLevelChallenges : MonoBehaviour
    {
        /// <summary>
        ///     Choose a Challenge Prefab that has the <see cref="SCILLBattlePassChallengeItem" /> script attached. This prefab is
        ///     instantiated
        ///     for each challenge in the current battle pass level and will be either added as a child to this transform or if
        ///     provided, to the <see cref="challengeContainer" /> transform.
        /// </summary>
        [Tooltip(
            "Chosse a Challenge Prefab that has the SCILLBattlePassChallengeItem script attached. This prefab is instantiated for each challenge in the current battle pass level")]
        public SCILLBattlePassChallengeItem challengePrefab;

        /// <summary>
        ///     Choose a Challenge Prefab that has the <see cref="SCILLBattlePassChallengeItem" /> script attached. This prefab is
        ///     instantiated
        ///     for each completed challenge in the current battle pass level and will be either added as a child to this transform
        ///     or if provided, to the <see cref="challengeContainer" /> transform.
        /// </summary>
        [Tooltip(
            "Chosse a Challenge Prefab that has the SCILLBattlePassChallengeItem script attached for completed challenges. This prefab is instantiated for each completed challenge in the current battle pass level")]
        public SCILLBattlePassChallengeItem completedChallengePrefab;

        /// <summary>
        ///     Connect a transform that will be used as the container for the challenge. If left blank, the challengePrefab items
        ///     will be added to this game object. The container will be hidden if no challenges are available.
        /// </summary>
        [Tooltip(
            "Connect a transform that will be used as the container for the challenge. If left blank, the challengePrefab items will be added to this game object. The container will be hidden if no challenges are available.")]
        public Transform challengeContainer;

        /// <summary>
        ///     Hide completed challenges or keep them in the list
        /// </summary>
        [Tooltip("Hide completed challenges or keep them in the list")]
        public bool showCompletedChallenges;

        /// <summary>
        ///     A text field used to display the challenge stats in the format "1/2"
        /// </summary>
        [Tooltip("A text field that will contain the challenge stats in the form 1/2")]
        public Text challengeStats;

        private BattlePassLevel battlePassLevel { get; set; }

        private void Awake()
        {
            
            Assert.IsNotNull(challengePrefab, "SCILL Battle Pass Challenges: You need to assign a prefab to challengePrefab");
            Assert.IsNotNull(completedChallengePrefab, "SCILL Battle Pass Challenges: You need to assign a prefab to completedChallengePrefab");
            
            ClearChallenges();
        }

        // Start is called before the first frame update
        private void Start()
        {
            if(SCILLBattlePassManager.Instance)
                OnBattlePassLevelsUpdatedFromServer(SCILLBattlePassManager.Instance.BattlePassLevels);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnEnable()
        {
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += OnBattlePassLevelsUpdatedFromServer;
        }

        private void OnDestroy()
        {
            SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer -= OnBattlePassLevelsUpdatedFromServer;
        }

        private void OnBattlePassLevelsUpdatedFromServer(List<BattlePassLevel> battlePassLevels)
        {
            if (battlePassLevels == null || battlePassLevels.Count <= 0) return;
            
            var currentLevelIndex = SCILLBattlePassManager.Instance.GetCurrentBattlePassLevelIndex();
            if (currentLevelIndex > -1 && currentLevelIndex < battlePassLevels.Count)
            {
                battlePassLevel = battlePassLevels[currentLevelIndex];
                UpdateChallengeList();
            }
        }

        /// <summary>
        ///     This class will clear all game objects from the <see cref="challengeContainer" /> or this class if no container is
        ///     set. Override this function in your own class if you want to adjust this.
        /// </summary>
        protected virtual void ClearChallenges()
        {
            // Make sure we delete all items from the battle pass levels container
            // This way we can leave some dummy level items in Unity Editor which makes it easier to design UI
            foreach (var child in GetComponentsInChildren<SCILLBattlePassChallengeItem>()) Destroy(child.gameObject);
        }

        /// <summary>
        ///     This class will create instances of the <see cref="challengePrefab" /> and will add them to the
        ///     <see cref="challengeContainer" />
        ///     or this objects transform if no container is set. Only <c>in-progress</c> challenges will be rendered per default.
        /// </summary>
        protected virtual void UpdateChallengeList()
        {

            // Make sure we remove old challenges from the list
            ClearChallenges();

            // If there is no level or it's not activated yet, don't show anything
            if (battlePassLevel == null) return;


            var numberOfChallengesShown = 0;
            var numberOfChallengesCompleted = 0;
            foreach (var challenge in battlePassLevel.challenges)
            {

                numberOfChallengesShown++;

                // Only add active challenges to the list
                if (challenge.type == "finished")
                {
                    numberOfChallengesCompleted++;
                    if (showCompletedChallenges)
                    {
                        var challengeGO =
                            Instantiate(completedChallengePrefab ? completedChallengePrefab : challengePrefab,
                                challengeContainer ? challengeContainer : transform);
                        var challengeItem = challengeGO.GetComponent<SCILLBattlePassChallengeItem>();
                        if (challengeItem)
                        {
                            challengeItem.UpdateChallenge(challenge);
                        }
                    }
                }
                else
                {
                    var challengeGO = Instantiate(challengePrefab, challengeContainer ? challengeContainer : transform);
                    var challengeItem = challengeGO.GetComponent<SCILLBattlePassChallengeItem>();
                    if (challengeItem)
                    {
                        challengeItem.UpdateChallenge(challenge);
                    }
                }
            }

            // Hide the challengeContainer if no challenges are visible. 
            if (challengeContainer)
            {
                if (numberOfChallengesShown <= 0)
                    challengeContainer.gameObject.SetActive(false);
                else
                    challengeContainer.gameObject.SetActive(true);
            }

            if (challengeStats)
                challengeStats.text = numberOfChallengesCompleted + "/" + battlePassLevel.challenges.Count;
        }
    }
}