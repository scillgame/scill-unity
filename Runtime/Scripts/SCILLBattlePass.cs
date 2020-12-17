using System;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

public class SCILLBattlePass : SCILLThreadSafety
{
    [HideInInspector]
    public BattlePass battlePass;

    [Header("Required connections")]
    [Tooltip("Connect a game object in your hierarchy that shows a UI with a button to unlock the Battle Pass. It will be hidden if the battle pass is already unlocked")]
    public GameObject unlockGroup;
    
    [Tooltip("A text field that will be set with the current active level. Just a number like 2 or 99")]
    public Text currentLevel;
    [Tooltip("A text UI element that is used to render the name of the battle pass")]
    public Text battlePassNameText;

    [Header("Optional connections")]
    [Tooltip("An image that will be set with the image set for the battle pass. It will be loaded as a Sprite with the name you set in Admin Panel. Make sure this Sprite is in a Resources folder - otherwise it will not be loaded at runtime")]
    public Image imageXS;
    [Tooltip("An image that will be set with the image set for the battle pass. It will be loaded as a Sprite with the name you set in Admin Panel. Make sure this Sprite is in a Resources folder - otherwise it will not be loaded at runtime")]
    public Image imageS;
    [Tooltip("An image that will be set with the image set for the battle pass. It will be loaded as a Sprite with the name you set in Admin Panel. Make sure this Sprite is in a Resources folder - otherwise it will not be loaded at runtime")]
    public Image imageM;
    [Tooltip("An image that will be set with the image set for the battle pass. It will be loaded as a Sprite with the name you set in Admin Panel. Make sure this Sprite is in a Resources folder - otherwise it will not be loaded at runtime")]
    public Image imageL;
    [Tooltip("An image that will be set with the image set for the battle pass. It will be loaded as a Sprite with the name you set in Admin Panel. Make sure this Sprite is in a Resources folder - otherwise it will not be loaded at runtime")]
    public Image imageXL;

    private List<BattlePassLevel> _levels;
    private SCILLBattlePassLevel _selectedBattlePassLevel;
    private Dictionary<int, GameObject> _levelObjects = new Dictionary<int, GameObject>();

    public delegate void BattlePassUnlockedAction(BattlePass battlePass);
    public static event BattlePassUnlockedAction OnBattlePassUnlocked;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        Debug.Log("SHOW BATTLE PASS");
        if (SCILLBattlePassManager.Instance)
        {
            battlePass = SCILLBattlePassManager.Instance.SelectedBattlePass;
            _levels = SCILLBattlePassManager.Instance.BattlePassLevels;
            Debug.Log("SELECTED BATTLE PASS" + battlePass);
        }

        SCILLBattlePassManager.OnBattlePassUpdatedFromServer += OnBattlePassUpdatedFromServer;
        SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer += OnBattlePassLevelsUpdatedFromServer;
        
        UpdateUI();
    }
    
    private void OnDestroy()
    {
        SCILLBattlePassManager.OnBattlePassUpdatedFromServer -= OnBattlePassUpdatedFromServer;
        SCILLBattlePassManager.OnBattlePassLevelsUpdatedFromServer -= OnBattlePassLevelsUpdatedFromServer;
    }

    private void OnBattlePassUpdatedFromServer(BattlePass battlePass)
    {
        this.battlePass = battlePass;
        UpdateUI();
    }
    
    private void OnBattlePassLevelsUpdatedFromServer(List<BattlePassLevel> battlePassLevels)
    {
        _levels = battlePassLevels;
        
        if (currentLevel)
        {
            int currentLevelIndex = 0;
            for (int i = 0; i < _levels.Count; i++)
            {
                if (_levels[i].level_completed == true)
                {
                    currentLevelIndex = i;
                }
                else
                {
                    break;
                }
            }

            currentLevel.text = (currentLevelIndex+1).ToString();
        }
    }

    private void UpdateUI()
    {
        if (battlePass == null)
        {
            return;
        }

        if (battlePassNameText)
        {
            battlePassNameText.text = battlePass.battle_pass_name;
        }
        
        if (battlePass.image_xs != null && imageXS)
        {
            Sprite sprite = Resources.Load<Sprite>(battlePass.image_xs);
            imageXS.sprite = sprite;
        }
        
        if (battlePass.image_s != null && imageS)
        {
            Sprite sprite = Resources.Load<Sprite>(battlePass.image_s);
            imageS.sprite = sprite;
        }
        
        if (battlePass.image_m != null && imageM)
        {
            Sprite sprite = Resources.Load<Sprite>(battlePass.image_m);
            imageM.sprite = sprite;
        }      
        
        if (battlePass.image_l != null && imageL)
        {
            Sprite sprite = Resources.Load<Sprite>(battlePass.image_l);
            imageL.sprite = sprite;
        }    
        
        if (battlePass.image_xl != null && imageXL)
        {
            Sprite sprite = Resources.Load<Sprite>(battlePass.image_xl);
            imageXL.sprite = sprite;
        }            

        if (battlePass.unlocked_at != null)
        {
            // This battle pass is unlocked
            unlockGroup.SetActive(false);
        }
        else
        {
            unlockGroup.SetActive(true);
        }
    }

    public async void OnBattlePassUnlockButtonPressed()
    {
        var purchaseInfo = new BattlePassUnlockPayload(0, "EUR");
        var unlockInfo = await SCILLManager.Instance.SCILLClient.UnlockBattlePassAsync(battlePass.battle_pass_id, purchaseInfo);
        if (unlockInfo != null)
        {
            battlePass.unlocked_at = unlockInfo.purchased_at;
            OnBattlePassUnlocked?.Invoke(battlePass);
            UpdateUI();
        }
    }
}
