using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * mouse right click to hide cursor 
 * esc to open in-game menu and show cursor 
*/

public class MainMenu : MonoBehaviour
{

    public GameObject instruction_menu; 
    public GameObject difficulty_selection; 

    public void Start()
    {
        difficulty_selection.SetActive(false); 
        instruction_menu.SetActive(false);  
    }

    public void Instruction_OnClick()
    {
        difficulty_selection.SetActive(false);  
        if (!instruction_menu.activeSelf)
            instruction_menu.SetActive(true);  
        else 
            instruction_menu.SetActive(false); 
    }

    public void NewGame_OnClick()
    {
        // SceneManager.LoadScene("level"); 
        if (!difficulty_selection.activeSelf)
            difficulty_selection.SetActive(true); 
        else 
            difficulty_selection.SetActive(false); 
        instruction_menu.SetActive(false); 
    }

    public void Easy_OnClick()
    {
        GameData.width = 16; 
        GameData.length = 16; 
        GameData.total_time = 50f; 
        // GameData.spider_rest = 7.5f; 
        SceneManager.LoadScene("level"); 
    }

    public void Medium_OnClick()
    {
        GameData.width = 24; 
        GameData.length = 24; 
        GameData.total_time = 100f; 
        // GameData.spider_rest = 6f; 
        SceneManager.LoadScene("level"); 
    }

    public void Hard_OnClick() 
    {
        GameData.width = 32; 
        GameData.length = 32; 
        GameData.total_time = 150f; 
        // GameData.spider_rest = 5f; 
        SceneManager.LoadScene("level"); 
    }

    public void Exit_OnClick()
    {
        SceneManager.LoadScene("Menu 3D");
    }
}
