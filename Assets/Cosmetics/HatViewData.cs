using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Hat View Data", menuName = "Mouthwash/Hat View Data")]
public class HatViewData : ScriptableObject
{
    public Sprite MainImage;
    public Sprite BackImage;
    public Sprite LeftMainImage;
    public Sprite LeftBackImage;
    public Sprite ClimbImage;
    public Sprite FloorImage;
    public Sprite LeftClimbImage;
    public Sprite LeftFloorImage;
    public Material AltShader;
}