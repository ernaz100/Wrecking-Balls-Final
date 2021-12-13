using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenShop);
    }

    private void OpenShop()
    {
        gameManager.OpenShop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
