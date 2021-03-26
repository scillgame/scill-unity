using System.Collections;
using System.Collections.Generic;
using System.Xml;
using SCILL.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SCILLLeaderboardRankingItem : MonoBehaviour
{
    public Text username;
    public Text rank;
    public Text score;
    public Image image;

    private LeaderboardRanking _ranking;
    public LeaderboardRanking ranking
    {
        get => _ranking;
        set
        {
            _ranking = value;
            UpdateUI();
        }
    }

    private int _numberOfDecimals = 0;

    public int numberOfDecimals
    {
        get => _numberOfDecimals;
        set
        {
            _numberOfDecimals = value;
            UpdateUI();
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void UpdateUI()
    {
        if (ranking == null)
        {
            return;
        }
        
        if (username)
        {
            if (ranking.additional_info != null && !string.IsNullOrEmpty(ranking.additional_info.username))
            {
                username.text = ranking.additional_info.username;
            }
            else
            {
                username.text = "Guest";
            }
        }

        if (image)
        {
            if (ranking.additional_info != null && !string.IsNullOrEmpty(ranking.additional_info.avatarImage))
            {
                Sprite sprite = Resources.Load<Sprite>("Avatars/" + ranking.additional_info.avatarImage);
                if (sprite)
                {
                    image.sprite = sprite;
                }
            }
        }

        if (rank)
        {
            rank.text = ranking.rank.ToString() + ".";
        }

        if (score)
        {
            score.text = ranking.score.ToString();
            if (numberOfDecimals > 0 && score.text.Length > numberOfDecimals)
            {
                score.text = score.text.Insert(score.text.Length - numberOfDecimals, ".");
            }
        }
    }
}
