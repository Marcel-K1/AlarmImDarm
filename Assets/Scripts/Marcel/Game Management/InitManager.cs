/*********************************************************************************************
* Project: Alarm Im Darm
* File   : InitManager
* Date   : 19.10.2022
* Author : Marcel Klein
*
* Initializing the game
* 
* History:
*    19.10.2022    MK    Created
*********************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitManager : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.FirstBoot = true;
        GameManager.Instance.LoadMainMenuFirstTime();
    }
}
