using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Magic.GameEditor
{
    class AtlasSpliter
    {
        [MenuItem("[Framework]/AtlasSpliter")]
        public static void AtlasSplit()
        {

            StartTimeStamp = GetTimeStamp();

            Texture2D image = Selection.activeObject as Texture2D; // 得到选中的图集 该图集事先已经被修改属性
            if (image == null)
            {
                EditorUtility.DisplayDialog("Message", "请选择图集", "known");
                return;
            }
            try
            {
                string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(image)); //文件所在的路径
                string path = rootPath + "/" + image.name + ".PNG"; //Assets/Resources/UI/XXX.png
                TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;
                isReadableSave = texImp.isReadable;
                texImp.isReadable = true;
                AssetDatabase.ImportAsset(path);

                AssetDatabase.CreateFolder(rootPath, image.name);

                // 遍历图集中每一张小图,并使用文件IO写入到同级目录的文件夹中
                for (int i = 0; i < texImp.spritesheet.Length; i++)
                {
                    SpriteMetaData metaData = texImp.spritesheet[i];
                    EditorUtility.DisplayProgressBar("Atlas Split", metaData.name + " in " + image.name, (float)(i) / (float)texImp.spritesheet.Length);
                    Texture2D myimage = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);

                    for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++)
                    {
                        for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                            myimage.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, image.GetPixel(x, y));
                    }
                    if (myimage.format != TextureFormat.ARGB32 && myimage.format != TextureFormat.RGB24)
                    {
                        Texture2D newTexture = new Texture2D(myimage.width, myimage.height);
                        newTexture.SetPixels(myimage.GetPixels(0), 0);
                        myimage = newTexture;
                    }
                    var pngData = myimage.EncodeToPNG();
                    File.WriteAllBytes(rootPath + "/" + image.name + "/" + metaData.name + ".PNG", pngData);
                    AssetDatabase.Refresh();
                }
                EditorUtility.ClearProgressBar();

                texImp.isReadable = isReadableSave;
                AssetDatabase.ImportAsset(path);

                double EndTimeStamp = GetTimeStamp();
                double timeUse = (EndTimeStamp - StartTimeStamp) / 1000;
                EditorUtility.DisplayDialog("Message", "图集" + image.name + "切分完成，耗时" + timeUse.ToString("f0") + "秒", "known");
            }
            catch (Exception E)
            {
                EditorUtility.DisplayDialog("Message", "图集切分出错\n"+E.ToString(), "known");
            }
        }

        private static bool isReadableSave = false;
        private static double StartTimeStamp = 0;
        private static double GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            double timeStamp = ts.TotalMilliseconds;
            return timeStamp;
        }

    }
}
