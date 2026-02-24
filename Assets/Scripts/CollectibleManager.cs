using UnityEngine;
using static Collectible;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance
    {
        get;
        private set;
    }
    [Header("Collectible Counts")]
    [SerializeField] int coinsTotal;
    [SerializeField] int gemsTotal;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Subscribe to the collectible event
        CollectibleEventSystem.OnCollectibleCollected += HandleCollectibleCollected;
    }
    private void OnDisable()
    {
        // Unsubscribe from the event
        CollectibleEventSystem.OnCollectibleCollected -= HandleCollectibleCollected;


    }
    private void HandleCollectibleCollected(CollectibleType
type, int amount)
    {
   //     if (!collectibles.ContainsKey(type))
    //        collectibles[type] = 0;
   //     collectibles[type] += amount;
    //    Debug.Log($"{type}: {collectibles[type]}");
    }
}
