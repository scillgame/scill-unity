using System;
using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.UI;

public class SCILLBattlePasses : MonoBehaviour
{
    private List<BattlePass> _battlePasses;

    public GameObject battlePassPrefab;
    public SCILLRewardPreview rewardPreview;
    public SCILLBattlePassLevelChallenges activeChallenges;

    public int itemsPerPage = 5;
    public int currentPageIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateBattlePasses();
    }

    void UpdateBattlePasses()
    {
        _battlePasses = SCILLManager.Instance.SCILLClient.GetBattlePasses();
        Debug.Log("Loaded Battle Passes" + _battlePasses.Count);

        for (var i = 0; i < _battlePasses.Count; i++)
        {
            var battlePass = _battlePasses[i];
            var battlePassGO = Instantiate(battlePassPrefab);
            var battlePassScript = battlePassGO.GetComponent<SCILLBattlePass>();
            if (battlePassScript)
            {
                battlePassScript.battlePass = battlePass;
                battlePassScript.showLevelInfo = (i == 0);
                battlePassScript.itemsPerPage = itemsPerPage;
            }
            battlePassGO.transform.SetParent(this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
