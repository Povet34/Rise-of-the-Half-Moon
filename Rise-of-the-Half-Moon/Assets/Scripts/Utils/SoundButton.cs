using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : Button
{
    protected override void Awake()
    {
        base.Awake();
        onClick.AddListener(() => SoundManager.Instance.PlaySFX(Definitions.SOUND_UI_BUTTON_CLICK));
    }
}
