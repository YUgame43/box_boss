using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Options")]
    public Transform target; // كائن اللاعب الذي ستدور الكاميرا حوله
    public Vector3 targetOffset = new Vector3(0, 1.5f, 0); // رفع نقطة النظر لتكون عند رأس/أكتاف اللاعب

    [Header("Camera Settings")]
    public float distance = 5f; // المسافة بين الكاميرا واللاعب
    public float mouseSensitivity = 3f; // سرعة دوران الماوس

    // حدود النظر للأعلى والأسفل (عشان الكاميرا ما تنقلب)
    public float minYAngle = -20f;
    public float maxYAngle = 60f;

    private float currentX = 0f;
    private float currentY = 0f;

    void Start()
    {
        // قفل الماوس وإخفاؤه
        Cursor.lockState = CursorLockMode.Locked;
    }

    // نستخدم LateUpdate بدلاً من Update للكاميرا لضمان أن تتحرك الكاميرا بعد أن ينتهي اللاعب من حركته تماماً
    void LateUpdate()
    {
        if (target == null) return;

        // استقبال حركة الماوس
        currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
        currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // تقييد زاوية النظر لفوق وتحت
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

        // حساب الدوران (Rotation) والموقع (Position) الجديد للكاميرا
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // تطبيق الموقع والدوران
        transform.position = target.position + targetOffset + rotation * direction;
        transform.LookAt(target.position + targetOffset);
    }
}