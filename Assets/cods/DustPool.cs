using System.Collections.Generic;
using UnityEngine;

public class DustPool : MonoBehaviour
{
    public static DustPool Instance; // عشان نقدر نوصله من أي سكربت ثاني بسهولة

    [Header("Pool Settings")]
    public GameObject dustPrefab;
    public int poolSize = 15;

    private List<GameObject> pool;

    void Awake()
    {
        Instance = this;
        pool = new List<GameObject>();

        // 1. بناء المسبح: نصنع الغبار مسبقاً ونطفيه
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(dustPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    // 2. دالة لاستدعاء غبار جاهز من المسبح
    public GameObject GetDust()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy) // إذا كان مطفي (غير مستخدم)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // إذا خلصنا الـ 15 حبة، نصنع واحد جديد للحالات الطارئة
        GameObject newObj = Instantiate(dustPrefab);
        newObj.SetActive(true);
        pool.Add(newObj);
        return newObj;
    }
}