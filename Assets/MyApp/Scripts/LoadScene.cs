﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

   public void loadScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }
    private void Update()
    {
        
    }
}
