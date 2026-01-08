using UnityEngine;
using UnityEngine.UI;

public class Boss_Health_Bar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private float health;

    private void Awake()
    {
        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }
        if (fillImage == null)
        {
            Debug.LogError("No Image component found for Boss_Health_Bar!");
        }
    }

    public void SetMaxHealth(float maxHealth)
    {
        health = maxHealth;
        SetHealth(health);
    }

    public void SetHealth(float currentHealth)
    {
        if (fillImage != null)
        {
            float fillAmount = Mathf.Clamp01(currentHealth / health);
            fillImage.fillAmount = fillAmount;
        }
    }
}
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class Boss_Health_Bar : MonoBehaviour
//{
//    [SerializeField] private Image fillImage;

//    private float health;

//    public void SetMaxHealth(float maxHealth)
//    {
//        health = maxHealth;
//        SetHealth(health);
//    }

//    public void SetHealth(float currentHealth)
//    {
//        float fillAmount = Mathf.Clamp01(currentHealth / health);
//        fillImage.fillAmount = fillAmount;
//    }
//}
