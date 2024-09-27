using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSaveMB : BaseMenuButton
{
    [SerializeField] private CurrentStats save;

    public override void Click()
    {
        save.ResetStats(BootLoadManager.instance.GetDefaultSave());
        BootLoadManager.instance.SaveToFile(save);
    }
}
