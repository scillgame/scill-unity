using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCILL.Model;
using UnityEngine.UI;

public class SCILLCategoryItem : MonoBehaviour
{
    [Tooltip("The prefab used for instantiating SCILLChallengeItem instances. If you leave this blank, the challengePrefab setting of the parent SCILLPersonalChallenges will be used instead.")]
    public SCILLChallengeItem challengePrefab;
    [Tooltip("Categories can be collapsed. With this setting you can set the default state of this category. Per default it will be expanded.")]
    public bool expanded = true;
    [Tooltip("A UnityEngine.UI.Text component that will be used to set the categories name")]
    public Text categoryName;
    [Tooltip("Challenges instantiated will be added as children into this transform if set, otherwise it will be directly added as child to this game object.")]
    public Transform challengesContainer;
    
    private Dictionary<string, GameObject> _challengeObjects = new Dictionary<string, GameObject>();
    private ChallengeCategory _category;

    private void Awake()
    {

    }

    [HideInInspector]
    public ChallengeCategory Category
    {
        get => _category;
        set
        {
            _category = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (challengePrefab == null)
        {
            SCILLPersonalChallenges personalChallenges = GetComponentInParent<SCILLPersonalChallenges>();
            if (personalChallenges)
            {
                challengePrefab = personalChallenges.challengePrefab;
            }            
        }
        
        UpdateChallengeList();           
    }

    public void OnToggleExpanded()
    {
        expanded = !expanded;

        var challengeItems = GetComponentsInChildren<SCILLChallengeItem>(true);
        foreach (var challengeItem in challengeItems)
        {
            challengeItem.gameObject.SetActive(expanded);
        }
    }

    public void UpdateChallenge(Challenge challenge)
    {
        GameObject challengeGO = null;
        if (_challengeObjects.TryGetValue(challenge.challenge_id, out challengeGO))
        {
            var challengeItem = challengeGO.GetComponent<SCILLChallengeItem>();
            if (challengeItem)
            {
                challengeItem.challenge = challenge;
            }
        }
    }

    public void UpdateChallengeList()
    {
        
        Debug.Log("UPDATE CHALLENGE LIST");
        foreach (var challenge in _category.challenges)
        {
            GameObject challengeGO = null;
            if (_challengeObjects.TryGetValue(challenge.challenge_id, out challengeGO))
            {
                var challengeItem = challengeGO.GetComponent<SCILLChallengeItem>();
                if (challengeItem)
                {
                    challengeItem.challenge = challenge;
                }
            }
            else
            {
                challengeGO = Instantiate(challengePrefab.gameObject);
                var challengeItem = challengeGO.GetComponent<SCILLChallengeItem>();
                if (challengeItem)
                {
                    challengeItem.challenge = challenge;
                }

                // Group all challenges in a container if provided
                if (challengesContainer)
                {
                    challengeGO.transform.SetParent(challengesContainer);    
                }
                else
                {
                    challengeGO.transform.SetParent(transform);
                }
                

                _challengeObjects.Add(challenge.challenge_id, challengeGO);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_category == null)
        {
            return;
        } 
        
        categoryName.text = _category.category_name;
    }
}
