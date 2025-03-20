using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI HpCount;
    public Slider hpSlider;
    public Image fillImage; // Reference to the Fill Image

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        HpCount.text = unit.currentHP + "/" + unit.maxHP;
        UpdateHealthBarColor(unit.currentHP, unit.maxHP);
    }

    public void SetHP(int hp, int maxHP)
    {
        hpSlider.value = hp;
        HpCount.text = hp + "/" + maxHP;
        UpdateHealthBarColor(hp, maxHP);
    }

    private void UpdateHealthBarColor(int currentHP, int maxHP)
    {
        float healthPercentage = (float)currentHP / maxHP;

        Color healthColor;
        if (healthPercentage > 0.5f)
        {
            healthColor = new Color(0f, 1f, 0f, 1f); // Green
        }
        else if (healthPercentage > 0.2f)
        {
            healthColor = new Color(1f, 1f, 0f, 1f); // Yellow
        }
        else
        {
            healthColor = new Color(1f, 0f, 0f, 1f); // Red
        }

        // Apply the color to the fillImage
        if (fillImage != null)
        {
            fillImage.color = healthColor;
        }
        else
        {
            Debug.LogError("Fill Image component is not assigned.");
        }
    }
}
