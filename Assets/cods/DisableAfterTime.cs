using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    public float lifeTime = 1f; // مدة بقاء الغبار قبل أن يختفي

    // OnEnable تُستدعى كل مرة بنشغل فيها الكائن من المسبح
    void OnEnable()
    {
        Invoke("DisableObject", lifeTime);
    }

    void DisableObject()
    {
        gameObject.SetActive(false); // إطفاء الكائن (إرجاعه للمسبح)
    }

    void OnDisable()
    {
        CancelInvoke(); // تنظيف الأوامر
    }
}