using UnityEngine;

public class CameraFollowWithParallax : MonoBehaviour
{
    [Header("Камера следует за игроком")]
    public Transform target; // Игрок
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothSpeed = 0.125f;

    [Header("Статичный фон (двигается вместе с камерой один в один)")]
    public Transform staticBackground;

    [Header("Параллакс слои (двигаются медленнее)")]
    public Transform[] parallaxLayers;
    public float[] parallaxFactors; // Чем меньше значение — тем медленнее слой

    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = transform.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // === Камера плавно следует за игроком ===
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // === Статичный фон (привязываем к камере) ===
        if (staticBackground != null)
        {
            staticBackground.position = new Vector3(transform.position.x, transform.position.y, staticBackground.position.z);
        }

        // === Параллакс слои ===
        Vector3 deltaMovement = transform.position - lastCameraPosition;

        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i] == null) continue;
            float factor = parallaxFactors.Length > i ? parallaxFactors[i] : 0.5f;
            Vector3 layerMovement = deltaMovement * factor;
            parallaxLayers[i].position += new Vector3(layerMovement.x, layerMovement.y, 0);
        }

        lastCameraPosition = transform.position;
    }
}
