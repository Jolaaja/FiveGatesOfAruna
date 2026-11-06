using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y + 1f, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform followTarget)
    {
        target = followTarget;
    }
}
