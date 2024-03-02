using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDataHolder : Singleton<GlobalDataHolder>
{
    private int _mCurrentDay = 0;
    public int CurrentDay { get => _mCurrentDay; }

    public void IncreaseDayCount()
    {
        _mCurrentDay++;
    }

}
