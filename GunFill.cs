using UnityEngine;
using UnityEngine.UI;

// Controls the fill on the gun depending on the ammo amount/type 
public class GunFill : MonoBehaviour
{
    public Slider slider;
    public Image fill;

    private void Start()
    {
        if (slider == null)
        {
            Debug.LogError("Slider reference is not set in HealthBar script!");
        }
    }

    public void SetMaxAmmo(int ammo)
    {
        slider.maxValue = ammo;
        slider.value = ammo;
    }

    public void SetAmmo(int ammo)
    {
        slider.value = ammo;
    }

    public void FillGreen()
    {
        fill.color = Color.green;
    }

    public void ResetGun()
    {
        fill.color = Color.clear;
    }
}
