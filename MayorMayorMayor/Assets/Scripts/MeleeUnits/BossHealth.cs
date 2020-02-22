using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossHealth : Health
{
    public WinWindow winWindow;

    protected override void Die()
    {
        winWindow.Win();
    }
}