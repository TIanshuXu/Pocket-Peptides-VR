using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    public int      l_number; // level number
    // stores  each level's goal  models' name string array
    public string[] level_1 = new string[4]; 
    public string[] level_2 = new string[4];
    public string[] level_3 = new string[1];

    string[] cur_level; // stores current level
    int     main_l_num; // stores main level num

    private void Start()
    {
        // setup the level_number
        l_number = 0;
        // use num to represent level_1 (dynamic)
        main_l_num = SceneManager.GetActiveScene().buildIndex;
        // set the main level up and update the level label
        SetUpMainLevel();
    }

    public void SetUpMainLevel()
    {
        // go to next scene, start level_2 _3 ... and so on
        switch (main_l_num)
        {
            case 1:
                cur_level = level_1;
                break;
            case 2:
                cur_level = level_2;
                break;
            case 3:
                cur_level = level_3;
                break;
            case 4: // for further levels (work with server and automation)
                    // cur_level = level_4; 
                break;
            default: // use level_1 as default level
                cur_level = level_1;
                break;
        }
        UpdateLevelLabel(); // main_levels
    } 

    public void GoToNextSubLevel()
    {
        // reference the shadow models object, reset the level
        Transform shadows = GameObject.Find("Protein Drop Panel").transform.Find("Shadow Models");
        GameObject.Find("Button Panel").GetComponent<ButtonPanel>().UndoAll(); 
        // hide previous shadow
        shadows.Find(cur_level[l_number]).gameObject.SetActive(false); 
        if (l_number < cur_level.Length - 1)
        {
            l_number += 1;
            // show new level's shadow
            shadows.Find(cur_level[l_number]).gameObject.SetActive(true); 
        }
        else
        {
            // move to next scene
            GoToNextMainLevel(); // this func will change scene
        }
        UpdateLevelLabel(); // sub_levels
    }

    public void GoToNextMainLevel() // main_level (level_1)
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FindObjectOfType<Transition>().FadeToNextLevel(); // use transition code
    }

    public void UpdateLevelLabel()
    {
        // TextMeshProUGUI t = GameObject.Find("Level Panel").transform.Find("Text").GetComponent<TextMeshProUGUI>();
        int level     = main_l_num;
        int sub_level = l_number + 1;
        // t.text = "Level " + level + "-" + sub_level;
    }
}
