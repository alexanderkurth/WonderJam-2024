using System;
using UnityEngine;

public enum SINGLETON_DUPLICATE_HANDLING { WARNING, ERROR, DESTROY_COMPONENT, DESTROY_GAMEOBJECT }
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    [Header("Duplicate Handling")]
    [Tooltip("What to do when a second instance appears ?")]
    [SerializeField]
    private SINGLETON_DUPLICATE_HANDLING duplicationHandling = SINGLETON_DUPLICATE_HANDLING.ERROR;

    [Tooltip("Check to make this gameObect DontDestroyOnLoad (persistent beetween scenes)")]
    [SerializeField] private bool DoNotDestroyOnLoad = true;

    private static string message_dupplicate = "An other instance of " + typeof(T) + " already exists !";
    private static T _instance = null;
    private static int nbInstances = 0;
    public static T Instance
    {
        get { return _instance; }
    }

    protected virtual void Awake()
    {
        ++nbInstances;
        if (_instance == null)
        {
            _instance = (T)this;
        }
        else
        {
            switch (duplicationHandling)
            {
                case SINGLETON_DUPLICATE_HANDLING.WARNING:
                    Debug.LogWarning(message_dupplicate + " " + (nbInstances) + " now present.");
                    break;
                case SINGLETON_DUPLICATE_HANDLING.ERROR:
                    Debug.LogError(message_dupplicate + " " + (nbInstances) + " now present.");
                    break;
                case SINGLETON_DUPLICATE_HANDLING.DESTROY_COMPONENT:
                    Debug.LogWarning(message_dupplicate + " Destroying component " + typeof(T) + " from gameObject " + gameObject.name);
                    Destroy(this);
                    break;
                case SINGLETON_DUPLICATE_HANDLING.DESTROY_GAMEOBJECT:
                    Debug.LogWarning(message_dupplicate + " Destroying GameObject " + gameObject.name);
                    Destroy(gameObject);
                    break;
            }
        }
        if (DoNotDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
        nbInstances--;
    }
}
