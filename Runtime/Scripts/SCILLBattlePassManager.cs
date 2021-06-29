using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

public class SCILLBattlePassManager : MonoBehaviour
{
    public static SCILLBattlePassManager Instance { get; private set; }

    private List<BattlePass> _battlePasses;
    public BattlePass SelectedBattlePass;
    public List<BattlePassLevel> BattlePassLevels;
    private int _selectedBattlePassLevelIndex;

    public BattlePassLevel SelectedBattlePassLevel => BattlePassLevels?[_selectedBattlePassLevelIndex];

    public int SelectedBattlePassLevelIndex
    {
        get => _selectedBattlePassLevelIndex;
        set
        {
            _selectedBattlePassLevelIndex = value;
            OnSelectedBattlePassLevelChanged?.Invoke(SelectedBattlePassLevel);
        }
    }

    public delegate void BattlePassUpdatedFromServerAction(BattlePass battlePass);

    public static event BattlePassUpdatedFromServerAction OnBattlePassUpdatedFromServer;

    public delegate void BattlePassLevelsUpdatedFromServerAction(List<BattlePassLevel> battlePassLevels);

    public static event BattlePassLevelsUpdatedFromServerAction OnBattlePassLevelsUpdatedFromServer;

    public delegate void BattlePassChallengeUpdateAction(BattlePassChallengeChangedPayload challengeChangedPayload);

    public static event BattlePassChallengeUpdateAction OnBattlePassChallengeUpdate;

    public delegate void BattlePassLevelRewardClaimedAction(BattlePassLevel level);

    public static event BattlePassLevelRewardClaimedAction OnBattlePassLevelRewardClaimed;

    public delegate void SelectedBattlePassLevelChangedAction(BattlePassLevel selectedBattlePassLevel);

    public static event SelectedBattlePassLevelChangedAction OnSelectedBattlePassLevelChanged;


    private int NumUpdateRequests { get; set; } = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (null == SCILLManager.Instance.SCILLClient)
            yield return null;

        var battlePassesAsync = SCILLManager.Instance.SCILLClient.GetBattlePassesAsync();
        battlePassesAsync.Then(battlePassesList =>
        {
            _battlePasses = battlePassesList;
            // Select a battle pass
            BattlePass selectedBattlePass = SelectBattlePass(_battlePasses);

            if (selectedBattlePass != null)
            {
                this.SelectedBattlePass = selectedBattlePass;

                // Inform delegates that a new battle pass has been selected
                OnBattlePassUpdatedFromServer?.Invoke(selectedBattlePass);


                // Load battle pass levels from SCILL backend
                UpdateBattlePassLevelsFromServer();
            }

            StartRealtimeNotifications();

            SCILLBattlePass.OnBattlePassUnlocked += OnOnBattlePassUnlocked;
        });
    }

    protected virtual BattlePass SelectBattlePass(List<BattlePass> battlePasses)
    {
        BattlePass selectedBattlePass = null;
        for (var i = 0; i < battlePasses.Count; i++)
        {
            var battlePass = battlePasses[i];
            if (battlePass.unlocked_at != null)
            {
                selectedBattlePass = battlePass;
                break;
            }
        }

        if (selectedBattlePass == null)
        {
            if (battlePasses.Count > 0)
            {
                selectedBattlePass = battlePasses[0];
            }
        }

        return selectedBattlePass;
    }

    private void OnOnBattlePassUnlocked(BattlePass battlePass)
    {
        // If the battle pass was unlocked, then reload the levels and challenges
        if (battlePass.battle_pass_id == SelectedBattlePass.battle_pass_id)
        {
            UpdateBattlePassLevelsFromServer();
        }
    }

    public void UpdateBattlePassLevelsFromServer()
    {
        NumUpdateRequests++;

        if (NumUpdateRequests > 1)
        {
            return;
        }

        SendUpdateRequest();
    }

    private void SendUpdateRequest()
    {
        var levelsPromise =
            SCILLManager.Instance.SCILLClient.GetBattlePassLevelsAsync(SelectedBattlePass.battle_pass_id);
        // Debug.Log("Requested BP Update from Server");

        levelsPromise.Then(levels =>
        {
            BattlePassLevels = levels;

            int numBattlePassLevels = null == BattlePassLevels ? 0 : BattlePassLevels.Count;
            Debug.Log($"Received BP Update from server with {numBattlePassLevels} entries");

            // If we have not selected a battle pass level, let's pick the current one
            if (_selectedBattlePassLevelIndex == 0)
            {
                SelectedBattlePassLevelIndex = GetCurrentBattlePassLevel();
            }

            OnBattlePassLevelsUpdatedFromServer?.Invoke(levels);

            NumUpdateRequests--;
            if (NumUpdateRequests > 0)
                SendUpdateRequest();
        });
    }

    public int GetCurrentBattlePassLevel()
    {
        var selectedLevelIndex = 0;
        foreach (var level in BattlePassLevels)
        {
            if (level.level_completed == true)
            {
                selectedLevelIndex++;
            }
            else
            {
                break;
            }
        }

        return Mathf.Min(selectedLevelIndex, BattlePassLevels.Count - 1);
    }

    private void OnBattlePassChangedNotification(BattlePassChallengeChangedPayload payload)
    {
        // The battle pass challenge changed
        if (payload.webhook_type == "battlepass-challenge-changed")
        {
            // Check if the challenge is still in-progress. If not, we need to reload the levels to update
            // current state - as change is not isolated to one challenge
            if (payload.new_battle_pass_challenge.type == "in-progress")
            {
                // Inform all delegates of the challenge update
                OnBattlePassChallengeUpdate?.Invoke(payload);
            }
            else
            {
                // Reload the levels from the server and update UI
                UpdateBattlePassLevelsFromServer();
            }
        }
        else
        {
            // Reload the levels from the server and update UI
            UpdateBattlePassLevelsFromServer();
        }
    }

    private void OnDestroy()
    {
        StopRealtimeNotifications();
    }

    public void StartRealtimeNotifications()
    {
        if (null != SelectedBattlePass)
        {
            // Get notifications from SCILL backend whenever battle pass changes
            SCILLManager.Instance.StartBattlePassUpdateNotifications(SelectedBattlePass.battle_pass_id,
                OnBattlePassChangedNotification);
        }
    }

    public void StopRealtimeNotifications()
    {
        if (SelectedBattlePass != null)
        {
            SCILLManager.Instance.StopBattlePassUpdateNotifications(SelectedBattlePass.battle_pass_id,
                OnBattlePassChangedNotification);
        }
    }

    public void ClaimBattlePassLevelReward(BattlePassLevel level)
    {
        var responsePromise = SCILLManager.Instance.SCILLClient.ClaimBattlePassLevelRewardAsync(level.level_id);

        responsePromise.Then(response =>
        {
            if (response != null && response.message == "OK")
            {
                OnBattlePassLevelRewardClaimed?.Invoke(level);
            }
        });
    }
}