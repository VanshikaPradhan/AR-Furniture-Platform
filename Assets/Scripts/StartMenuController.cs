using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
   public void OnStartClick() {
        SceneManager.LoadSceneAsync(1);
   }
}
