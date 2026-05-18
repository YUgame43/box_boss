using System;
using UnityEngine;

public class RockBossAI : MonoBehaviour
{
    // حدث (Event) لإخبار واجهة المستخدم بتغير المستوى
    public static event Action<int> OnLevelChanged;

    public enum BossState { Aiming, Warning, Dashing, Resetting }

    [Header("State")]
    public BossState currentState = BossState.Aiming;
    public int currentLevel = 1;

    [Header("Targeting & References")]
    public Transform player;
    public LineRenderer lineRenderer;

    [Header("Stats")]
    public float aimDuration = 2f; // مدة تتبع اللاعب
    public float dashDelay = 1.5f; // وقت التأخير (التحذير) قبل الانطلاق
    public float dashSpeed = 15f;  // سرعة الانطلاق

    private float stateTimer = 0f;
    private Vector3 dashTargetPosition;
    private Vector3 startPosition; // نقطة العودة
    private float dustTimer = 0f;


    void Start()
    {
        startPosition = transform.position;

        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();

        // إعدادات الخط (عريض)
        lineRenderer.startWidth = 2f;
        lineRenderer.endWidth = 2f;
        lineRenderer.enabled = false;

        // إذا لم تقم بربط اللاعب يدوياً، سيبحث عنه الكود
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        switch (currentState)
        {
            case BossState.Aiming:
                HandleAiming();
                break;
            case BossState.Warning:
                HandleWarning();
                break;
            case BossState.Dashing:
                HandleDashing();
                break;
            case BossState.Resetting:
                HandleResetting();
                break;
        }
    }

    private void HandleAiming()
    {
        lineRenderer.enabled = true;

        // رسم الخط من المكعب باتجاه اللاعب وتمديده للخارج
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // للحفاظ على الخط مسطحاً على الأرض

        // نقطة نهاية الخط (بعيدة جداً لتتجاوز الحلبة)
        dashTargetPosition = transform.position + (direction * 50f);

        // رسم الخط بمستوى منخفض عن الأرض قليلاً لعدم التداخل
        Vector3 groundOffset = new Vector3(0, -transform.localScale.y / 2f + 0.1f, 0);
        lineRenderer.SetPosition(0, transform.position + groundOffset);
        lineRenderer.SetPosition(1, dashTargetPosition + groundOffset);

        stateTimer += Time.deltaTime;
        if (stateTimer >= aimDuration)
        {
            stateTimer = 0f;
            currentState = BossState.Warning; // الانتقال لحالة التحذير
        }
    }

    private void HandleWarning()
    {
        // في هذه الحالة المكعب لا يتحرك والخط يثبت مكانه (يتجمد)
        stateTimer += Time.deltaTime;
        if (stateTimer >= dashDelay)
        {
            stateTimer = 0f;
            lineRenderer.enabled = false; // إخفاء الخط عند الانطلاق
            currentState = BossState.Dashing;
        }
    }

    private void HandleDashing()
    {
        // الاندفاع بقوة نحو الهدف
        transform.position = Vector3.MoveTowards(transform.position, dashTargetPosition, dashSpeed * Time.deltaTime);

        // --- نظام تشغيل الغبار من الـ Pool ---
        dustTimer -= Time.deltaTime;
        if (dustTimer <= 0f)
        {
            // استدعاء غبار من المسبح
            GameObject dust = DustPool.Instance.GetDust();

            // وضعه في مكان المكعب الحالي ولكن على مستوى الأرض (Y=0.1)
            dust.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);

            dustTimer = 0.1f; // إطلاق غبار كل 0.1 ثانية أثناء الركض
        }
        // ------------------------------------

        // إذا وصل للنهاية (أو وقع خارج الحلبة)
        if (Vector3.Distance(transform.position, dashTargetPosition) < 1f || transform.position.y < -5f)
        {
            currentState = BossState.Resetting;
        }
    }

    private void HandleResetting()
    {
        // 1. التأكد إذا كان اللاعب ملتصقاً بالمكعب لرميه خارجاً
        if (player.parent == this.transform)
        {
            player.SetParent(null); // فك الارتباط (اللاعب رح يضل طاير برة الحلبة)

            // إرجاع الفيزياء عشان يسقط اللاعب للأسفل
            var playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.isKinematic = false;
            }
        }

        // 2. إعدادات الترقية وزيادة الصعوبة
        currentLevel++;
        dashSpeed += 5f;
        dashDelay = Mathf.Max(0.5f, dashDelay - 0.2f);

        transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
        startPosition.y += 0.25f; // تعديل الارتفاع عشان ما يغرز

        OnLevelChanged?.Invoke(currentLevel);

        // 3. إرجاع المكعب فقط (بدون اللاعب) لنقطة البداية
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        stateTimer = 0f;
        currentState = BossState.Aiming;
    }

    // نظام الالتصاق عند الاصطدام باللاعب
    private void OnTriggerEnter(Collider other)
    {
        // يجب أن يكون الكائن المصدوم هو اللاعب، ويجب أن يكون المكعب في حالة الهجوم
        if (other.CompareTag("Player") && currentState == BossState.Dashing)
        {
            // جعل اللاعب ابناً للمكعب (ليلصق به)
            other.transform.SetParent(this.transform);

            // تعطيل سكربت حركة اللاعب والفيزياء الخاصة به حتى لا يقاوم
            var playerScript = other.GetComponent<ThirdPersonController>();
            if (playerScript != null) playerScript.enabled = false;

            var playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null) playerRb.isKinematic = true;
        }
    }
}