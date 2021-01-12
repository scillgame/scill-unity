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
    [Tooltip("Set the name of the level, use {level} for the level number")]
    public string levelTextFormat = "Level {level}";

    [Header("Prefabs")] 
    [Tooltip("A prefab that will be used as a reward icon")]
    public SCILLBattlePassRewardIcon rewardIconPrefab;
    
    [Tooltip("A slider that will be used to render the current progress in this level")]
    public Slider progressSlider;
    [Tooltip("Automatically disable progress slider if value is 0 or 1.")]
    public bool autoHideProgressSlider = false;

    [Tooltip("A text ui object that will be set with a string like 2/4 - which is 2 of 4 challenges completed.")]
    public Text challengeText;

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
        SCILLBattlePassManager.OnSelectedBattlePassLevelChanged += OnSelectedBattlePassLevelChanged;
        UpdateUI();
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
            {
                Select();
            }
            else
            {
                Deselect();
            }            
        }
    }

    private void OnDestroy()
    {
        SCILLBattlePassManager.OnBattlePassChallengeUpdate -= OnBattlePassChallengeUpdate;
        SCILLBattlePassManager.OnSelectedBattlePassLevelChanged -= OnSelectedBattlePassLevelChanged;
    }

    private void OnBattlePassChallengeUpdate(BattlePassChallengeChangedPayload challengeChangedPayload)
    {
        UpdateUI();
    }

    private void ClearRewardIcons()
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

        if (levelName)
        {
            levelName.text = levelTextFormat.Replace("{level}", battlePassLevel.level_priority.ToString());    
        }

        if (battlePassLevelInfo) battlePassLevelInfo.SetActive(showLevelInfo);

        float totalProgress = 0;
        int activatedChallenges = 0; 
        foreach (var challenge in battlePassLevel.challenges)
        {
            float challengeProgress = 0;
            if (challenge.challenge_goal > 0)
            {
                challengeProgress = (float) challenge.user_challenge_current_score /
                                    (float) challenge.challenge_goal;
            }

            totalProgress += challengeProgress * (1.0f / (float)battlePassLevel.challenges.Count);

            if (challenge.user_challenge_current_score >= challenge.challenge_goal)
            {
                activatedChallenges += 1;
            }
        }
        
        // Update slider
        if (progressSlider)
        {
            progressSlider.value = totalProgress;

            if (autoHideProgressSlider)
            {
                if (totalProgress <= 0)
                {
                    progressSlider.gameObject.SetActive(false);
                }
                else
                {
                    progressSlider.gameObject.SetActive(true);
                }                
            }

            if (battlePassLevel.level_priority <= 1)
            {
                progressSlider.gameObject.SetActive(false);
            }
        }

        if (challengeText)
        {
            challengeText.text = activatedChallenges + "/" + battlePassLevel.challenges.Count;
        }

        if (SCILLBattlePassManager.Instance.SelectedBattlePassLevel?.battle_pass_id == battlePassLevel.level_id)
        {
            Select();
        }
    }

    public virtual void Select()
    {
        var outline = GetComponentInChildren<Outline>();
        if (outline)
        {
            outline.enabled = true;
        }
    }

    public virtual void Deselect()
    {
        var outline = GetComponentInChildren<Outline>();
        if (outline)
        {
            outline.enabled = false;
        }
    }
}
