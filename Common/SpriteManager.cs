using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class SpriteManager : Singleton<SpriteManager>
{
    private SpriteAtlas _uiAtlas;
    private SpriteAtlas _equipmentAtlas;
    private SpriteAtlas _unitAtlas;

    void Start()
    {
        _uiAtlas = Resources.Load("Atlas/UIAtlas", typeof(SpriteAtlas)) as SpriteAtlas;
        _equipmentAtlas = Resources.Load("Atlas/EquipmentAtlas", typeof(SpriteAtlas)) as SpriteAtlas;
        _unitAtlas = Resources.Load("Atlas/UnitAtlas", typeof(SpriteAtlas)) as SpriteAtlas;
    }
    public Sprite GetUISprite(string name)
    {
        return _uiAtlas.GetSprite(name);
    }
    public Sprite GetEquipmentSprite(string name)
    {
        return _equipmentAtlas.GetSprite(name);
    }
    public Sprite GetUnitSprite(int ID)
    {
        string path = string.Format("Unit_{0}_icon", ID);
        return _unitAtlas.GetSprite(path);
    }
}
