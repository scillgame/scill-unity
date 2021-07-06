using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    public class SCILLBattlePassLevelChallenges : MonoBehaviour
    {
        [Tooltip(
            "Chosse a Challenge Prefab that has the SCILLBattlePassChallengeItem script attached. This prefab is instantiated for each challenge in the current battle pass level")]
        public SCILLBattlePassChallengeItem challengePrefab;

        [Tooltip(
            "Chosse a Challenge Prefab that has the SCILLBattlePassChallengeItem script attached for completed challenges. This prefab is instantiated for each completed challenge in the current battle pass level")]
        public SCILLBattlePassChallengeItem completedChallengePrefab;

        [Tooltip(
            "Connect a transform that will be used as the container for the challenge. If left blank, the challengePrefab items will be added to this game object. The container will be hidden if no challenges are available.")]
        public Transform challengeContainer;

        [Tooltip("Hide completed challenges or keep them in the list")]
        public bool showCompletedChallenges = false;

        [Tooltip("A text field that will contain the challenge stats in the form 1/2")]
        public Text challengeStats;

        private BattlePassLevel battlePassLevel;

        private void Awake()
        {
            if (!challengePrefab)
            {
                Debug.LogError("SCILL Battle Pass Challenges: You need to assign a prefab to challengePrefab");
            }

            ClearChallenges();
        }

        // Start is called before the first frame update
        void Start()
        {
            OnBattlePassLevelsUpdatedFromServer(SCILLBattlePassManager.Instance.BattlePassLevels);
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
            if (battlePassLevels == null || battlePassLevels.Count <= 0)
            {
                return;
            }

            int currentLevelIndex = SCILLBattlePassManager.Instance.GetCurrentBattlePassLevelIndex();
            battlePassLevel = battlePassLevels[currentLevelIndex];
            UpdateChallengeList();
        }

        protected virtual void ClearChallenges()
        {
            // Make sure we delete all items from the battle pass levels container
            // This way we can leave some dummy level items in Unity Editor which makes it easier to design UI
            foreach (SCILLBattlePassChallengeItem child in GetComponentsInChildren<SCILLBattlePassChallengeItem>())
            {
                Destroy(child.gameObject);
            }
        }

        protected virtual void UpdateChallengeList()
        {
            // Make sure we remove old challenges from the list
            ClearChallenges();

            // If there is no level or it's not activated yet, don't show anything
            if (battlePassLevel == null || battlePassLevel.activated_at == null)
            {
                return;
            }

            int numberOfChallengesShown = 0;
            int numberOfChallengesCompleted = 0;
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
                            challengeItem.challenge = challenge;
                            challengeItem.UpdateUI();
                        }
                    }
                }
                else
                {
                    var challengeGO = Instantiate(challengePrefab, challengeContainer ? challengeContainer : transform);
                    var challengeItem = challengeGO.GetComponent<SCILLBattlePassChallengeItem>();
                    if (challengeItem)
                    {
                        challengeItem.challenge = challenge;
                        challengeItem.UpdateUI();
                    }
                }
            }

            // Hide the challengeContainer if no challenges are visible. 
            if (challengeContainer)
            {
                if (numberOfChallengesShown <= 0)
                {
                    challengeContainer.gameObject.SetActive(false);
                }
                else
                {
                    challengeContainer.gameObject.SetActive(true);
                }
            }

            if (challengeStats)
            {
                challengeStats.text = numberOfChallengesCompleted + "/" + battlePassLevel.challenges.Count;
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}