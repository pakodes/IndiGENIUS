using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI HpCount;

    public Slider hpSlider;


    public void SetHUD(Unit unit)
    {
        Debug.Log("Setting HUD for unit: " + unit.unitName);
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        HpCount.text = unit.currentHP + "/" + unit.maxHP;
        Debug.Log("HUD set: " + nameText.text + ", " + levelText.text + ", " + HpCount.text);


    }

    public void SetHP(int hp)
    {
        Debug.Log("Setting HP: " + hp);
        hpSlider.value = hp;
        HpCount.text = hp + "/" + hpSlider.maxValue;
        Debug.Log("HP set: " + HpCount.text);


    }

}
