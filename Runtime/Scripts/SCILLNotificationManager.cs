using System.Collections;
using System.Collections.Generic;
using SCILL.Model;
using UnityEngine;

public enum SCILLNotificationType
{
    Success,
    Error
}

abstract class SCILLCenterNotification
{ 
    public GameObject Prefab;
    public float PreferredTimeout = 2f;

    public SCILLCenterNotification(GameObject prefab)
    {
        Prefab = prefab;
    }

    public abstract GameObject Show(Transform container);
}

class SCILLCenterTextNotification : SCILLCenterNotification
{
    public string Text;

    public SCILLCenterTextNotification(GameObject prefab, string text) : base(prefab)
    {
        Text = text;
        PreferredTimeout = 2f;
    }

    public override GameObject Show(Transform container)
    {
        GameObject notificationGo = Object.Instantiate(Prefab, container, false);
        SCILLNotification notification = notificationGo.GetComponent<SCILLNotification>();
        if (notification)
        {
            notification.message.text = Text;
        }

        return notificationGo;
    }
}

class SCILLCenterChallengeNotification : SCILLCenterTextNotification
{
    public Challenge Challenge;
    
    public SCILLCenterChallengeNotification(GameObject prefab, Challenge challenge, string text) : base(prefab, text)
    {
        Challenge = challenge;
        PreferredTimeout = 3;
    }
    
    public override GameObject Show(Transform container)
    {
        GameObject challengeGo = Object.Instantiate(Prefab, container, false);
        SCILLChallengeItem challengeItem = challengeGo.GetComponent<SCILLChallengeItem>();
        if (challengeItem)
        {
            challengeItem.challenge = Challenge;
        }
        
        SCILLNotification notification = challengeGo.GetComponent<SCILLNotification>();
        if (notification)
        {
            notification.message.text = Text;
        }

        return challengeGo;
    }
}

public class SCILLNotificationManager : MonoBehaviour
{
	public static SCILLNotificationManager Instance; // **<- reference link to SCILL

    public SCILLNotification successNotificationPrefab;
    public SCILLNotification errorNotificationPrefab;
    public SCILLNotification centerNotificationPrefab;
    public SCILLChallengeItem challengeNotificationPrefab;

    public Transform container;
    public Transform centerNotification;

    private Queue<SCILLCenterNotification> centerNotifications = new Queue<SCILLCenterNotification>();

    public void AddNotification(SCILLNotificationType type, string text, string image = null)
    {
        var prefab = successNotificationPrefab.gameObject;
        if (type == SCILLNotificationType.Error)
        {
            prefab = errorNotificationPrefab.gameObject;
        }
        
        GameObject notificationGo = Instantiate(prefab, container, false);
        SCILLNotification notification = notificationGo.GetComponent<SCILLNotification>();
        if (notification)
        {
            notification.message.text = text;
        }

        StartCoroutine(SelfDestruct(notificationGo, 5));
    }

    private IEnumerator SelfDestruct(GameObject gameObject, float timeout = 2f)
    {
        yield return new WaitForSeconds(timeout);
        Destroy(gameObject);
    }

    public void AddCenterNotification(string text, string image = null)
    {
        var notification = new SCILLCenterTextNotification(centerNotificationPrefab.gameObject, text);
        centerNotifications.Enqueue(notification);
    }
    
    public void AddChallengeUpdate(Challenge challenge, string text)
    {
        var notification = new SCILLCenterChallengeNotification(challengeNotificationPrefab.gameObject, challenge, text);
        centerNotifications.Enqueue(notification);
    }

	void Awake() 
	{
        if (Instance == null) {
            Instance = this;
            
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void ShowNextCenterNotification()
    {
        if (centerNotifications.Count <= 0)
        {
            return;
        }
        
        var notification = centerNotifications.Dequeue();
        GameObject go = notification.Show(centerNotification);
        StartCoroutine(SelfDestruct(go, notification.PreferredTimeout));
    }

    // Update is called once per frame
    void Update()
    {
        if (centerNotification.childCount <= 0)
        {
            if (centerNotifications.Count > 0)
            {
                ShowNextCenterNotification();
            }
        }
    }
}
