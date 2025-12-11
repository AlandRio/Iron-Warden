using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Camera cam; // Renamed from 'camera' to avoid Unity warnings
    [SerializeField] private Transform target;

    // Set this to 2.0f if you want it exactly 2 units above the head
    [SerializeField] private float heightAboveHead = 2.0f;

    private Collider targetCollider;

    void Start()
    {
        if (target != null)
        {
            targetCollider = target.GetComponent<Collider>();
        }
    }

    public void updateHealthBar(float current, float max)
    {
        healthSlider.value = current / max;
    }

    void LateUpdate()
    {
        if (target == null || targetCollider == null) return;
        transform.rotation = cam.transform.rotation;
        Vector3 targetPosition = targetCollider.bounds.center;
        targetPosition.y = targetCollider.bounds.max.y + heightAboveHead;
        transform.position = targetPosition;
    }
}