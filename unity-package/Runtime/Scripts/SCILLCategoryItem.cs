using System;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SCILL
{
    /// <summary>
    ///     <para>
    ///         Challenges are grouped into categories. Attach this script to a prefab that you set as the
    ///         <c>categoryPrefab</c> in
    ///         the <see cref="SCILLPersonalChallenges" /> component. Categories instantiate <see cref="challengePrefab" />
    ///         objects that are either set
    ///         directly in this class or using the prefab set in the parent <see cref="SCILLPersonalChallenges" /> component.
    ///     </para>
    ///     <para>
    ///         Category UI support expanding and collapsing the UI. Use the <c>expanded</c> setting to set the expansion state
    ///         at start.
    ///     </para>
    /// </summary>
    public class SCILLCategoryItem : MonoBehaviour
    {
        /// <summary>
        ///     The prefab used for instantiating <see cref="SCILLChallengeItem" /> instances. If you leave this blank, the
        ///     <c>challengePrefab</c> setting of the parent <see cref="SCILLPersonalChallenges" /> will be used instead.
        /// </summary>
        [Tooltip(
            "The prefab used for instantiating SCILLChallengeItem instances. If you leave this blank, the challengePrefab setting of the parent SCILLPersonalChallenges will be used instead.")]
        public SCILLChallengeItem challengePrefab;

        /// <summary>
        ///     Categories can be collapsed. With this setting you can set the default state of this category. Per default it will
        ///     be expanded.
        /// </summary>
        [Tooltip(
            "Categories can be collapsed. With this setting you can set the default state of this category. Per default it will be expanded.")]
        public bool expanded = true;

        /// <summary>
        ///     A <c>UnityEngine.UI.Text</c> component that will be used to display the categories name
        /// </summary>
        [Tooltip("A UnityEngine.UI.Text component that will be used to display the categories name")]
        public Text categoryName;

        /// <summary>
        ///     Challenges instantiated will be added as children into this transform if set, otherwise it will be directly added
        ///     as child to this game object.
        /// </summary>
        [Tooltip(
            "Challenges instantiated will be added as children into this transform if set, otherwise it will be directly added as child to this game object.")]
        public Transform challengesContainer;

        protected readonly Dictionary<string, SCILLChallengeItem> _challengeObjects =
            new Dictionary<string, SCILLChallengeItem>();

        protected ChallengeCategory _category;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            if (challengePrefab == null)
            {
                TryGetChallengePrefab();

                Assert.IsNotNull(challengePrefab, "Challenge Prefab in SCILLCategoryItem is null.");
            }
        }

        private void TryGetChallengePrefab()
        {
            var personalChallenges = GetComponentInParent<SCILLPersonalChallenges>();
            if (personalChallenges) challengePrefab = personalChallenges.challengePrefab;
        }

        /// <summary>
        ///     Call this function to update the UI if category data has been changed. The <see cref="SCILLPersonalChallenges" /> script always
        ///     calls this function once data has been changed, either because of user interactions or incoming real time update
        ///     messages.
        /// </summary>
        public virtual void UpdateCategory(ChallengeCategory updatedCategoryData)
        {
            if (updatedCategoryData == null) return;
            
            if(categoryName)
                categoryName.text = updatedCategoryData.category_name;
            
            UpdateChallengeList(updatedCategoryData.challenges);
            _category = updatedCategoryData;
        }

        /// <summary>
        ///     Connect a click event of a toggle button to this function to hide all children of this category object (or the
        ///     <see cref="challengesContainer" />) if <see cref="expanded" /> is <c>false</c>, otherwise show them.
        /// </summary>
        public virtual void OnToggleExpanded()
        {
            expanded = !expanded;

            var challengeItems = GetComponentsInChildren<SCILLChallengeItem>(true);
            foreach (var challengeItem in challengeItems) challengeItem.gameObject.SetActive(expanded);
        }

        /// <summary>
        ///     Call this function to update an individual challenge in the categories challenges list.
        /// </summary>
        /// <param name="updatedChallengeData">The challenge to update</param>
        public virtual void UpdateChallenge(Challenge updatedChallengeData)
        {
            string challengeID = updatedChallengeData.challenge_id;
            if (_challengeObjects.ContainsKey(challengeID))
            {
                SCILLChallengeItem challengeItem = _challengeObjects[challengeID];
                if (challengeItem) challengeItem.UpdateChallenge(updatedChallengeData);
            }
            else
            {
                InstantiateNewChallengeItem(updatedChallengeData);
            }
        }

        protected virtual void InstantiateNewChallengeItem(Challenge updatedChallengeData)
        {
            string challengeID = updatedChallengeData.challenge_id;
            if (!challengePrefab)
                TryGetChallengePrefab();

            if (challengePrefab)
            {
                GameObject go = Instantiate(challengePrefab.gameObject,
                    challengesContainer ? challengesContainer : transform, false);
                if (go)
                {
                    SCILLChallengeItem challengeItem =
                        go.GetComponent<SCILLChallengeItem>();
                    if (challengeItem) challengeItem.UpdateChallenge(updatedChallengeData);

                    _challengeObjects.Add(challengeID, challengeItem);
                }
            }
        }

        [Obsolete("Please use UpdateChallengeList(List<Challenge>) instead.")]
        public void UpdateChallengeList(ChallengeCategory updatedCategoryData)
        {
            UpdateChallengeList(updatedCategoryData.challenges);
        }

        /// <summary>
        ///     Call this function to update the UI if challenge data has been changed. The <see cref="SCILLPersonalChallenges" /> script always
        ///     calls this function once data has been changed, either because of user interactions or incoming real time update
        ///     messages.
        /// </summary>
        public virtual void UpdateChallengeList(List<Challenge> updatedList)
        {
            if (null != updatedList)
            {
                foreach (Challenge challenge in updatedList)
                {
                    UpdateChallenge(challenge);
                }
            }
        }

        /// <summary>
        /// Determines whether the Category contains the <see cref="SCILLChallengeItem"/> with the given <see cref="challengeId"/>.
        /// </summary>
        /// <param name="challengeId">The id to search for.</param>
        /// <returns>True, if the <see cref="SCILLChallengeItem"/> with the given <see cref="challengeId"/> belongs to this Category object.</returns>
        public bool ContainsChallenge(string challengeId)
        {
            return _challengeObjects.ContainsKey(challengeId);
        }
    }
}