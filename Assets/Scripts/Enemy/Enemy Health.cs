using UnityEngine;
using UnityEngine.UI;

// handles the floating health bar for enemies
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Camera cam; // the main camera so the bar can look at it
    [SerializeField] private Transform target;

    // change this to make the bar float higher or lower
    [SerializeField] private float heightAboveHead = 2.0f;

    private Collider targetCollider;

    // getting the collider so we know how tall the enemy is
    void Start()
    {
        if (target != null)
        {
            targetCollider = target.GetComponent<Collider>();
        }
    }

    // changes the slider amount
    public void updateHealthBar(float current, float max)
    {
        healthSlider.value = current / max;
    }

    // moves the bar every frame
    void LateUpdate()
    {
        if (target == null || targetCollider == null) return;
        // make the bar face the camera so the player can always see it
        transform.rotation = cam.transform.rotation;
        // find the center of the enemy
        Vector3 targetPosition = targetCollider.bounds.center;
        // move the position to just above their head
        targetPosition.y = targetCollider.bounds.max.y + heightAboveHead;
        transform.position = targetPosition;
    }
}