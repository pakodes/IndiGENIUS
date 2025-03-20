using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel, damage, maxHP, currentHP;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer component is missing on the GameObject. Adding SpriteRenderer component.");
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        else
        {
            Debug.Log("SpriteRenderer component found on the GameObject.");
        }
    }

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); // Ensure currentHP does not go below 0

        Debug.Log("TakeDamage called. Current HP: " + currentHP);
        StartCoroutine(FlashDamageEffect());

        return currentHP <= 0;
    }

    private IEnumerator FlashDamageEffect()
    {
        if (spriteRenderer != null)
        {
            Debug.Log("FlashDamageEffect started.");
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color32(0x31, 0x31, 0x31, 0xFF); // Change to hex color #313131 to indicate damage
            yield return new WaitForSeconds(0.07f); // Duration of the flash
            spriteRenderer.color = originalColor; // Revert to original color
            Debug.Log("FlashDamageEffect ended.");
        }
        else
        {
            Debug.LogWarning("SpriteRenderer is null in FlashDamageEffect.");
        }
    }
}