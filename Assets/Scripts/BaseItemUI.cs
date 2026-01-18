using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseItemUI : MonoBehaviour
{
    public string name;
    public int amount;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;

    public void Add(int value)
    {
        amount += value;
        UpdateUI();

    }
    public void UpdateUI()
    {
        nameText.text = name;
        amountText.text = amount.ToString();

    }
    public void Subtract(int value)
    {
        amount -= value;
        if (amount < 0) amount = 0;
        UpdateUI();
    }
}
