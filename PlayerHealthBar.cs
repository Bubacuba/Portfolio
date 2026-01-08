using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public GameObject lifeMeter;

    public void ChangeLife(float change)
    {
        var l = lifeMeter.GetComponent<Image>();
        l.fillAmount -= change / 100;
    }


}
