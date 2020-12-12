using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

public class SCILLBattlePassLevelChallenges : MonoBehaviour
{
    [Tooltip("Chosse a Challenge Prefab that has the SCILLBattlePassChallengeItem script attached. This prefab is instantiated for each challenge in the current battle pass level")]
    public GameObject challengePrefab;

    [Tooltip(
        "Connect a transform that will be used as the container for the challenge. If left blank, the challengePrefab items will be added to this game object. The container will be hidden if no challenges are available.")]
    public Transform challengeContainer;
    
    public BattlePassLevel battlePassLevel;
    

    private void Awake()
    {
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
        
        // Find current level and update the challenges list
        int currentLevelIndex = 0;
        for (int i = 0; i < battlePassLevels.Count; i++)
        {
            if (battlePassLevels[i].level_completed == false)
            {
                currentLevelIndex = i;
                break;
            }
        }
            
        battlePassLevel = battlePassLevels[currentLevelIndex];
        UpdateChallengeList();
    }

    void ClearChallenges()
    {
        // Make sure we delete all items from the battle pass levels container
        // This way we can leave some dummy level items in Unity Editor which makes it easier to design UI
        foreach (SCILLBattlePassChallengeItem child in GetComponentsInChildren<SCILLBattlePassChallengeItem>()) {
            Destroy(child.gameObject);
        }        
    }

    public void UpdateChallengeList()
    {
        // Make sure we remove old challenges from the list
        ClearChallenges();
        
        if (battlePassLevel == null)
        {
            return;
        }

        int numberOfChallengesShown = 0;
        foreach (var challenge in battlePassLevel.challenges)
        {
            // Only add active challenges to the list
            if (challenge.type == "in-progress")
            {
                var challengeGO = Instantiate(challengePrefab, challengeContainer ? challengeContainer : transform);
                var challengeItem = challengeGO.GetComponent<SCILLBattlePassChallengeItem>();
                if (challengeItem)
                {
                    challengeItem.challenge = challenge;
                    challengeItem.UpdateUI();
                }

                numberOfChallengesShown++;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
