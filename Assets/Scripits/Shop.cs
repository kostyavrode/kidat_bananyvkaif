using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] public GameObject buyButton;
    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("Fire"))
        {
            buyButton.SetActive(false);
        }
    }
    public void Buy()
    {
        if (PlayerPrefs.GetInt("Money") >= 300)
        {
            PlayerPrefs.SetInt("Fire", 1);
            PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") - 300);
            PlayerPrefs.Save();
            buyButton.SetActive(false);
        }
    }
}
