using System;
using System.Collections.Generic;
using SCILL;
using SCILL.Model;
using UnityEngine;
using UnityEngine.Events;

namespace SCILL.Effects
{
    public class SCILLPersonalChallengeEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent onUnlocked;
        [SerializeField] private UnityEvent onActivated;
        [SerializeField] private UnityEvent onProgressUpdated;
        [SerializeField] private UnityEvent onCompleted;
        [SerializeField] private UnityEvent onClaimed;

        private Dictionary<SCILLPersonalChallengeModificationType, UnityEvent> TypeToEventMap = new Dictionary<SCILLPersonalChallengeModificationType, UnityEvent>();

        private void Awake()
        {
            TypeToEventMap[SCILLPersonalChallengeModificationType.Unlocked] = onUnlocked;
            TypeToEventMap[SCILLPersonalChallengeModificationType.Activated] = onActivated;
            TypeToEventMap[SCILLPersonalChallengeModificationType.Completed] = onCompleted;
            TypeToEventMap[SCILLPersonalChallengeModificationType.Claimed] = onClaimed;
            TypeToEventMap[SCILLPersonalChallengeModificationType.Progress] = onProgressUpdated;
        }

        void OnEnable()
        {
            SCILLPersonalChallengesManager.OnPersonalChallengeUpdatedFromServer += OnPersonalChallengeUpdated;
        }
        
        private void OnDisable()
        {
            SCILLPersonalChallengesManager.OnPersonalChallengeUpdatedFromServer -= OnPersonalChallengeUpdated;
        }

        private void OnPersonalChallengeUpdated(Challenge challenge, SCILLPersonalChallengeModificationType modificationtype)
        {
            if (TypeToEventMap.ContainsKey(modificationtype))
            {
                UnityEvent unityEvent = TypeToEventMap[modificationtype];
                if (null != unityEvent)
                {
                    unityEvent.Invoke();
                }
            }
        }

       
    }
}
