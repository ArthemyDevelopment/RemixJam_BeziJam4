using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public enum MainMenuScreens
{
    Main,
    HowToPlay,
    Credits,
}

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject[] HowToPlayScreens;
    [SerializeField]private Animator animations;
    [FoldoutGroup("DEBUG"), SerializeField,ReadOnly]private int curHowToPlayScreen;
    [FoldoutGroup("DEBUG"), SerializeField,ReadOnly]private MainMenuScreens curScreen = MainMenuScreens.Main;
    private PlayerInputs inputActions;
    
    
    void Awake()
    {
        inputActions = new PlayerInputs();
    }
    
    
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.UI.Navigate.performed += ChangeHowToPlayScreen;
        inputActions.Player.EffectAction.performed += BackToMainMenu;
    }
    
   
    
    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.UI.Navigate.performed -= ChangeHowToPlayScreen;
        inputActions.Player.EffectAction.performed -= BackToMainMenu;
    }


    public void ChangeScreen(int i)
    {
        switch (i)
        {
            case 0:
                curScreen = MainMenuScreens.Main;
                animations.Play("OpenMainMenu");
                break;
            case 1:
                curScreen = MainMenuScreens.HowToPlay;
                animations.Play("OpenHowToPlay");
                break;
            case 2:
                curScreen = MainMenuScreens.Credits;
                animations.Play("OpenCredits");
                break;
        }
    }

    void ChangeHowToPlayScreen(InputAction.CallbackContext callbackContext)
    {
        if (curScreen != MainMenuScreens.HowToPlay) return;
       float input = callbackContext.ReadValue<Vector2>().x;
       int temp = curHowToPlayScreen + (int)input;
       
       if (temp < HowToPlayScreens.Length && temp>=0)
       {
           foreach (var screen in HowToPlayScreens)
           {
               screen.SetActive(false);
           }

           curHowToPlayScreen = temp;
           HowToPlayScreens[curHowToPlayScreen].SetActive(true);
       }

       
    }

    void BackToMainMenu(InputAction.CallbackContext callbackContext)
    {
        if (curScreen == MainMenuScreens.Main) return;
        
        
        ChangeScreen(0);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene((int)GameScenes.GameScene);
    }
    
    
    
    
}
