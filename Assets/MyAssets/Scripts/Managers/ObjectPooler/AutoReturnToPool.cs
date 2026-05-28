using UnityEngine;

public class AutoReturnToPool : MonoBehaviour
{
    [SerializeField] private float returnDelay = 1f;

    private void OnEnable()
    {
        CancelInvoke(nameof(ReturnToPool));
        Invoke(nameof(ReturnToPool), returnDelay);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ReturnToPool));
    }

    private void ReturnToPool()
    {
        if(PoolRouter.Instance != null)
            PoolRouter.Instance.ReturnToPool(gameObject);
        else
            gameObject.SetActive(false);    
    }
}
