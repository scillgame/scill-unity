using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

public enum SCILLBattlePassVisibility
{
    Visible,
    Hidden,
    DoNothing
}

public class SCILLBattlePassToggleVisibility : MonoBehaviour
{
    public SCILLBattlePassVisibility ifLocked;
    public SCILLBattlePassVisibility ifUnlocked;
    
    private BattlePass _battlePass;
    
    private Image _image;
    
    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
    }
    
    private void OnEnable()
    {
        SCILLBattlePassManager.OnBattlePassUpdatedFromServer += OnBattlePassUpdatedFromServer;
    }
    
    private void OnDisable()
    {
        SCILLBattlePassManager.OnBattlePassUpdatedFromServer -= OnBattlePassUpdatedFromServer;
    }

    private void OnBattlePassUpdatedFromServer(BattlePass battlePass)
    {
        _battlePass = battlePass;
        UpdateUI();
    }

    // Update is called once per frame
    void UpdateUI()
    {
        if (!_image || _battlePass == null)
        {
            return;
        }

        if (ifLocked != SCILLBattlePassVisibility.DoNothing)
        {
            if (_battlePass.unlocked_at == null)
            {
                _image.enabled = (ifLocked == SCILLBattlePassVisibility.Visible);
            }
        }
        
        if (ifUnlocked != SCILLBattlePassVisibility.DoNothing)
        {
            if (_battlePass.unlocked_at != null)
            {
                _image.enabled = (ifUnlocked == SCILLBattlePassVisibility.Visible);
            }
        }
    }
}
