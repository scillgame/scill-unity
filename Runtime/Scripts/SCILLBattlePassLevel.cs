using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

public class SCILLBattlePassLevel : MonoBehaviour
{
    [Header("Required connections")]
    [Tooltip("A transform object in that the reward icons will be instantiated")]
    public Transform rewardContainer;
    
    [Header("Optional connections")]   
    [Tooltip("Set a game object that will be hidden if showLevelInfo of the SCILLBattlePass script will be false. It typical indicates the level number")]
    public GameObject battlePassLevelInfo;
    [Tooltip("A textfield that will be used to render the level number")]
    public Text levelName;

    [Header("Prefabs")] [Tooltip("A prefab that will be used as a reward icon")]
    public GameObject rewardIconPrefab;
    
    [Tooltip("A slider that will be used to render the current progress in this level")]
    public Slider progressSlider;

    [HideInInspector]
    public bool showLevelInfo = true;
    [HideInInspector]
    public Button button;
    
    private BattlePassLevel _battlePassLevel;
    
    public BattlePassLevel battlePassLevel
    {
        get => _battlePassLevel;
        set
        {
            _battlePassLevel = value;
            UpdateUI();
        }
    }

    private SCILLReward _reward;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }
    
    private void OnEnable()
    {
        SCILLBattlePassManager.OnBattlePassChallengeUpdate += OnBattlePassChallengeUpdate;
        UpdateUI();
    }

    private void OnDestroy()
    {
        SCILLBattlePassManager.OnBattlePassChallengeUpdate -= OnBattlePassChallengeUpdate;
    }

    private void OnBattlePassChallengeUpdate(BattlePassChallengeChangedPayload challengeChangedPayload)
    {
        UpdateUI();
    }

    void ClearRewardIcons()
    {
        foreach (SCILLBattlePassRewardIcon child in GetComponentsInChildren<SCILLBattlePassRewardIcon>()) {
            Destroy(child.gameObject);
        }      
    }
    
    public void UpdateUI()
    {
        if (battlePassLevel == null)
        {
            return;
        }
        
        ClearRewardIcons();

        if (rewardContainer && rewardIconPrefab && battlePassLevel.reward_type_name != "Nothing" && !string.IsNullOrEmpty(battlePassLevel.reward_amount))
        {
            var rewardIcon = Instantiate(rewardIconPrefab, rewardContainer, false);
            var rewardIconScript = rewardIcon.GetComponentInChildren<SCILLBattlePassRewardIcon>();
            if (rewardIconScript)
            {
                rewardIconScript.battlePassLevel = battlePassLevel;
            }
        }

        levelName.text = battlePassLevel.level_priority.ToString();
        battlePassLevelInfo.SetActive(showLevelInfo);

        // Update slider
        if (progressSlider)
        {
            float totalProgress = 0;
            foreach (var challenge in battlePassLevel.challenges)
            {
                float challengeProgress = 0;
                if (challenge.challenge_goal > 0)
                {
                    challengeProgress = (float) challenge.user_challenge_current_score /
                                        (float) challenge.challenge_goal;
                }

                totalProgress += challengeProgress * (1.0f / (float)battlePassLevel.challenges.Count);
            }
            
            if (totalProgress <= 0)
            {
                progressSlider.gameObject.SetActive(false);
            }
            else
            {
                progressSlider.value = totalProgress;
                progressSlider.gameObject.SetActive(true);
            }
        }
    }

    public void Select()
    {
        
    }

    public void Deselect()
    {
        
    }
}
