using UnityEngine;

public class PlatformReset : MonoBehaviour
{
    [SerializeField] private Vector3 carStartPosition = new Vector3(5f, 0.85f, -0.01897728f);
    [SerializeField] private Quaternion carStartRotation = Quaternion.identity;

    private void Start()
    {
        Debug.Log($"[{gameObject.name}] PlatformReset script started. Waiting for triggers...");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Лог для отладки: что именно вошло в триггер
        Debug.Log($"[{gameObject.name}] Trigger entered by: {other.gameObject.name}");

        // Ищем Rigidbody в родителях (чтобы найти «корень» машины)
        Rigidbody rb = other.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            // Если нужно — проверяем, что это именно машина (по тегу или названию)
            // if (!rb.CompareTag("Car")) return;

            Debug.Log($"[{gameObject.name}] Found Rigidbody in parent: {rb.name}. Resetting position...");

            // Телепортируем «корень» (где висит Rigidbody)
            rb.transform.position = carStartPosition;
            rb.transform.rotation = carStartRotation;

            // Сбрасываем физические скорости
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}