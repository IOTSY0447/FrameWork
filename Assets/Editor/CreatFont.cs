using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreatFont {
    static string fontPath = "Assets/ResStatic/ArtFont/addscore.fontsettings";
    static string targetString = "km+0123456789";
    static int texWidth = 50;
    static int texHeight = 52;

    static int padding = -10;

    [MenuItem ("Tools/生成数字字体")]
    public static void GetAllFileNameInOneDir () {
        Font font = AssetDatabase.LoadAssetAtPath<Font> (fontPath);
        if (font == null) {
            Debug.LogError ("字体文件不存在：" + fontPath);
            return;
        }

        int num = targetString.Length;
        int fontWidth = texWidth;
        int fontHeight = texHeight;
        float uvWidth = 1f / num;
        List<CharacterInfo> charInfoList = new List<CharacterInfo> ();
        for (int i = 0; i < num; i++) {
            CharacterInfo charInfo = new CharacterInfo ();
            charInfo.index = (int) targetString[i];
            charInfo.uvBottomLeft = new Vector2 (uvWidth * i, 0);
            charInfo.uvBottomRight = new Vector2 (uvWidth * i + uvWidth, 0);
            charInfo.uvTopLeft = new Vector2 (uvWidth * i, 1);
            charInfo.uvTopRight = new Vector2 (uvWidth * i + uvWidth, 1);
            charInfo.minX = 0;
            charInfo.maxX = fontWidth - 0;
            charInfo.minY = 0;
            charInfo.maxY = fontHeight;
            charInfo.advance = fontWidth + padding;

            charInfoList.Add (charInfo);
        }
        font.characterInfo = charInfoList.ToArray ();
        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh ();
    }

}