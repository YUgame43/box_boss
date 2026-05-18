using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rb;
    public Transform cameraTransform;
    public Animator animator;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    // تخزين الـ Hashes لتحسين الأداء (Optimization)
    private int speedHash;
    private int isBlockingHash;
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip footstepSound;

    // متغير لحفظ وقت آخر خطوة (لعمل نظام تبريد)
    private float lastFootstepTime = 0f;
    [Header("Procedural Animation (Look-At)")]
    public Transform lookTarget; // الكائن الذي سينظر إليه اللاعب
    [Range(0f, 1f)]
    public float lookWeight = 1f; // قوة الالتفات (1 يعني يلتفت بالكامل، 0 يعني لا يلتفت)

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();

        // إذا كنت تستخدم الكاميرا اليدوية
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;

        speedHash = Animator.StringToHash("Speed");
        isBlockingHash = Animator.StringToHash("IsBlocking"); // تسجيل متغير الحماية
    }
    public void PlayFootstepSound()
    {
        // الشرط الإضافي: يجب أن يمر 0.25 ثانية على الأقل بين كل خطوة والتي تليها لمنع التكرار
        if (footstepSound != null && audioSource != null && Time.time - lastFootstepTime > 0.25f)
        {
            lastFootstepTime = Time.time; // تحديث وقت الخطوة

            // وضع الصوت في المشغل
            audioSource.clip = footstepSound;

            // تغيير النبرة قليلاً لعدم الملل
            audioSource.pitch = Random.Range(0.8f, 1.1f);

            // استخدام Play بدلاً من PlayOneShot لقطع أي صوت خطوة سابق وتشغيل الجديد فوراً
            audioSource.Play();
        }
    }

    void Update()
    {
        // 1. استقبال أوامر الحركة
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentTargetSpeed = isRunning ? runSpeed : walkSpeed;

        // 2. استقبال أمر الدفاع (الكليك اليمين للماوس)
        bool isBlocking = Input.GetMouseButton(1);
        animator.SetBool(isBlockingHash, isBlocking);

        // 3. معالجة الحركة
        if (direction.magnitude >= 0.1f)
        {
            // إبطاء سرعة اللاعب إذا كان في وضعية الدفاع
            if (isBlocking) currentTargetSpeed = walkSpeed / 1.5f;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.MovePosition(transform.position + moveDirection.normalized * currentTargetSpeed * Time.deltaTime);

            float animSpeedValue = isRunning && !isBlocking ? 2f : 1f;
            animator.SetFloat(speedHash, animSpeedValue, 0.15f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat(speedHash, 0f, 0.15f, Time.deltaTime);
        }


    }
    private void OnAnimatorIK(int layerIndex)
    {
        // التأكد من وجود الـ Animator والهدف
        if (animator != null)
        {
            if (lookTarget != null)
            {
                // تحديد وزن (قوة) الالتفات
                // المعاملات: (الوزن الكلي، وزن الجسم، وزن الرأس، وزن العينين، الحد الأقصى للالتفاف)
                // زودنا مرونة الجسم (Body Weight) لـ 0.5 عشان يلف كتافه شوي مع راسه وتبين الحركة طبيعية
                animator.SetLookAtWeight(lookWeight, 0.5f, 1f, 1f, 0.7f);

                // توجيه الرأس نحو موقع الهدف
                animator.SetLookAtPosition(lookTarget.position);
                Debug.Log("IK is Running!");
            }
            else
            {
                // إذا لم يكن هناك هدف، نلغي الالتفات
                animator.SetLookAtWeight(0);
            }
        }
    }
}