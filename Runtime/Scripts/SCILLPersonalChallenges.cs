using System;
using System.Collections;
using System.Collections.Generic;
using SCILL;
using SCILL.Model;
using UnityEngine;

public class SCILLPersonalChallenges : SCILLThreadSafety
{
    [Tooltip("A prefab to be used as a category item. This will be instantiated for each category in the response")]
    public SCILLCategoryItem categoryPrefab;
    [Tooltip("A prefab that will be used for each challenge. It will be instantiated and added as child to the category game object")]
    public SCILLChallengeItem challengePrefab;
 
    private List<ChallengeCategory> _categories;
    private Dictionary<string, GameObject> _categoryObjects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        // Remove any dummies
        foreach (SCILLCategoryItem child in GetComponentsInChildren<SCILLCategoryItem>()) {
            Destroy(child.gameObject);
        }      
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdatePersonalChallengesList();

        SCILLManager.Instance.SCILLClient.StartChallengeUpdateNotifications(OnChallengeWebhookMessage);
    }

    private void OnDestroy()
    {
        SCILLManager.Instance.SCILLClient.StopChallengeUpdateNotifications(OnChallengeWebhookMessage);
    }

    private Challenge FindChallengeById(string id)
    {
        foreach (var category in _categories)
        {
            foreach (var challenge in category.challenges)
            {
                if (challenge.challenge_id == id)
                {
                    return challenge;
                }
            }
        }

        return null;
    }

    public async void UpdatePersonalChallengesList()
    {
        var categories = await SCILLManager.Instance.SCILLClient.GetPersonalChallengesAsync();
        _categories = categories;
        UpdateCategories(categories);
    }

    private void UpdateCategories(List<ChallengeCategory> categories)
    {
        foreach (var category in categories)
        {
            GameObject categoryGO = null;
            if (_categoryObjects.TryGetValue(category.category_id, out categoryGO))
            {
                var categoryItem = categoryGO.GetComponent<SCILLCategoryItem>();
                categoryItem.Category = category;
                categoryItem.UpdateChallengeList();
            }
            else
            {
                categoryGO = Instantiate(categoryPrefab.gameObject, transform, false);
                var categoryItem = categoryGO.GetComponent<SCILLCategoryItem>();
                if (categoryItem)
                {
                    categoryItem.Category = category;
                }

                _categoryObjects.Add(category.category_id, categoryGO);
            }
        }
    }

    private void UpdateChallenge(Challenge newChallenge)
    {
        Challenge challenge = FindChallengeById(newChallenge.challenge_id);
        if (challenge != null)
        {
            challenge.type = newChallenge.type;
            challenge.user_challenge_current_score = newChallenge.user_challenge_current_score;
            challenge.user_challenge_activated_at = newChallenge.user_challenge_activated_at;
            challenge.user_challenge_unlocked_at = newChallenge.user_challenge_unlocked_at;            
        }
    }

    private void OnChallengeWebhookMessage(ChallengeWebhookPayload payload)
    {
        // Make sure we run this code in Unitys "main" thread, i.e. in the update function
        RunOnMainThread.Enqueue(() =>
        {
            UpdateChallenge(payload.new_challenge); 
        });
    }

    public void UnlockPersonalChallenge(Challenge challenge)
    {
        var response = SCILLManager.Instance.SCILLClient.UnlockPersonalChallenge(challenge.challenge_id);
        if (response.status >= 200 && response.status < 300)
        {
            if (response.challenge != null)
            {
                UpdateChallenge(response.challenge);
            }
        }
    }
    
    public void ActivatePersonalChallenge(Challenge challenge)
    {
        var response = SCILLManager.Instance.SCILLClient.ActivatePersonalChallenge(challenge.challenge_id);
        if (response.status >= 200 && response.status < 300)
        {
            if (response.challenge != null)
            {
                UpdateChallenge(response.challenge);
            }
        }
    }
    
    public void ClaimPersonalChallengeReward(Challenge challenge)
    {
        var response = SCILLManager.Instance.SCILLClient.ClaimPersonalChallengeReward(challenge.challenge_id);
        if (response.status >= 200 && response.status < 300)
        {
            if (response.challenge != null)
            {
                UpdateChallenge(response.challenge);
            }
        }
    }    

    public void CancelPersonalChallenge(Challenge challenge)
    {
        var response = SCILLManager.Instance.SCILLClient.CancelPersonalChallenge(challenge.challenge_id);
        if (response.status >= 200 && response.status < 300)
        {
            if (response.challenge != null)
            {
                // In this case we need to reload the list from the server as we don't know if this challenge will
                // be available as it's set to repeatable or not. 
                UpdatePersonalChallengesList();
            }
        }
    }

    protected virtual void OnPersonalChallengeRewardClaimed(Challenge challenge)
    {
        
    }
}
