using UnityEngine;

// 1. Data: هيكل البيانات فقط (بدون أي وظائف)
public struct RainDropData
{
    public Vector3 position;
    public float speed;
}

public class RainSystem : MonoBehaviour
{
    [Header("Settings")]
    [Range(100, 1023)]
    public int dropCount = 1000; // عدد القطرات (أقصى حد لدفعة الرسم الواحدة هو 1023)
    public float spawnArea = 20f; // مساحة نزول المطر
    public float fallHeight = 15f; // ارتفاع السحابة (من وين ينزل المطر)
    public float baseSpeed = 15f; // سرعة السقوط

    [Header("Rendering")]
    public Mesh dropMesh; // شكل القطرة
    public Material dropMaterial; // لون/خامة القطرة

    // المصفوفات التي سنعالج فيها البيانات
    private RainDropData[] rainDrops;
    private Matrix4x4[] matrices;

    void Start()
    {
        // تهيئة المصفوفات بحجم عدد القطرات
        rainDrops = new RainDropData[dropCount];
        matrices = new Matrix4x4[dropCount];

        // إعطاء كل قطرة موقعاً وسرعة عشوائية كبداية
        for (int i = 0; i < dropCount; i++)
        {
            rainDrops[i] = new RainDropData
            {
                position = GetRandomPosition(),
                speed = baseSpeed + Random.Range(-3f, 3f) // تفاوت بسيط في سرعة كل قطرة
            };
        }
    }

    void Update()
    {
        // 2. System/Logic: معالجة كل البيانات في حلقة (Loop) واحدة فقط!
        for (int i = 0; i < dropCount; i++)
        {
            // حساب الموقع الجديد (النزول للأسفل)
            rainDrops[i].position.y -= rainDrops[i].speed * Time.deltaTime;

            // إذا ضربت القطرة الأرض (المحور الصادي أقل من 0)، نرجعها للسماء
            if (rainDrops[i].position.y < 0f)
            {
                rainDrops[i].position = GetRandomPosition();
            }

            // تحديث مصفوفة الرسم (لنخبر كرت الشاشة أين يرسم القطرة وبأي حجم)
            matrices[i] = Matrix4x4.TRS(
                rainDrops[i].position,
                Quaternion.identity,
                new Vector3(0.05f, 0.4f, 0.05f) // حجم القطرة (رفيعة وطويلة)
            );
        }

        // 3. Rendering: رسم الـ 1000 قطرة دفعة واحدة بدون أي GameObjects!
        if (dropMesh != null && dropMaterial != null)
        {
            Graphics.DrawMeshInstanced(dropMesh, 0, dropMaterial, matrices, dropCount);
        }
    }

    // دالة مساعدة لتوليد موقع عشوائي في السماء
    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            transform.position.x + Random.Range(-spawnArea, spawnArea),
            transform.position.y + fallHeight + Random.Range(0f, 5f), // تفريق القطرات عامودياً
            transform.position.z + Random.Range(-spawnArea, spawnArea)
        );
    }
}