import os
import subprocess
import argparse
from enum import Enum

#定义平台枚举
class Platform(Enum):
    windows = 0
    android = 1
    ios = 2

#更新svn的方法，目前使用的命令行的方式
def UpdateSVN():
    magicEditorPath = os.getcwd()+"/MagicEditor"
    print("start update "+ magicEditorPath )
    os.popen("cd " + magicEditorPath ).read()
    svncommand = "TortoiseProc.exe /command:update /path:" + magicEditorPath+" /closeonend:2"
    process = subprocess.Popen(svncommand)
    out, error = process.communicate()
    process.wait()
    print("Update Svn returns "+ str(out) + " with error "+ str(error))
	
#创建快捷方式
#参数说明：filename是被指向的目标 ， lnkname是链接名（因为工程设定，没有使用.lnk）
def CreateShortCut(filename, lnkname):
    """filename should be abspath, or there will be some strange errors"""

    filename = os.getcwd() + "/" +filename
    lnkname = os.getcwd() + "/" +  lnkname

    lnkname = lnkname.replace("\\","/")
    lnkname = lnkname.replace(".lnk","")
    filename = filename.replace("\\","/")
    
    print("filename = "+filename)
    print("link name = "+lnkname)
    
    if os.path.exists(lnkname):
        os.remove(lnkname)
        
    #如果lnk指向了一个并不能达到的位置，可能会出错 
    if not os.path.exists(filename):
        print("error filename: "+ filename)
        return
        
    os.symlink(filename, lnkname)
    print("link: " + lnkname  +" target: "+ filename)

	
#删除文件
def DeleteShortCut(filename):
    filename = os.getcwd() + "/" + filename
    filename = filename.replace(".lnk","")
    filename = filename.replace("\\","/")
    
    if os.path.exists(filename):
        os.remove(""+filename+"")
        print("delete: "+ filename)
		
		
#创建文件夹
def CreateDir(dirname):
    dirname = os.getcwd() + "/" + dirname
    if not os.path.exists(dirname):
        os.makedirs(dirname)
        
#创建所有的软链接
def Link(platform_type):

    #创建各种资源文件夹的链接
    CreateShortCut( "MagicEditor/Assets/Script/GamePresentation" , "MagicGameCode/Projects/GamePresentation.lnk" )
    CreateShortCut( "MagicEditor/Assets/Script/EditorScripts/SkillEditor" , "MagicGameCode/Projects/GameEditor/SkillEditor.lnk" )

    CreateDir("MagicEditor/Resources")
    CreateShortCut( "MagicGameCode/LuaScripts" , "MagicEditor/Resources/LuaScripts.lnk" )
    CreateDir("MagicClient/Assets")
    CreateShortCut( "MagicEditor/Resources" , "MagicClient/Assets/Resources.lnk" )
    CreateShortCut( "MagicEditor/Resources" , "MagicEditor/Assets/Resources.lnk" )
    print("platform = " + str(Platform(platform_type)))
    if Platform(platform_type) == Platform.windows:
        CreateDir("MagicEditor/AssetBundles/Windows")
        CreateDir("MagicClient/Assets/StreamingAssets")
        CreateShortCut( "MagicEditor/AssetBundles/Windows" , "MagicClient/Assets/StreamingAssets/res.lnk" )
        CreateDir("MagicClient/Assets/StreamingAssets/Audio/GeneratedSoundBanks")
        CreateShortCut( "MagicEditor/WwiseProject/GeneratedSoundBanks/Windows" , "MagicClient/Assets/StreamingAssets/Audio/GeneratedSoundBanks/Windows.lnk" )
    elif platform_type == Platform.android:
        CreateDir("MagicEditor/AssetBundles/Android")
        CreateDir("MagicClient/Assets/StreamingAssets")
        CreateShortCut( "MagicEditor/AssetBundles/Android" , "MagicClient/Assets/StreamingAssets/res.lnk" )
        CreateDir("MagicClient/Assets/StreamingAssets/Audio/GeneratedSoundBanks")
        CreateShortCut( "MagicEditor/WwiseProject/GeneratedSoundBanks/Android" , "MagicClient/Assets/StreamingAssets/Audio/GeneratedSoundBanks/Android.lnk" )
    elif platform_type == Platform.ios:
        CreateDir("MagicEditor/AssetBundles/iOS")
        CreateDir("MagicClient/Assets/StreamingAssets")
        CreateShortCut( "MagicEditor/AssetBundles/iOS" , "MagicClient/Assets/StreamingAssets/res.lnk" )
        CreateDir("MagicClient/Assets/StreamingAssets/Audio/GeneratedSoundBanks")
        CreateShortCut( "MagicEditor/WwiseProject/GeneratedSoundBanks/iOS" , "MagicClient/Assets/StreamingAssets/Audio/GeneratedSoundBanks/iOS.lnk" )
    CreateDir("MagicEditor/StreamingAssets")
    #CreateShortCut( "MagicEditor/StreamingAssets" , "MagicClient\Assets\StreamingAssets.lnk" )
    CreateShortCut( "MagicEditor/StreamingAssets" , "MagicEditor\Assets\StreamingAssets.lnk" )
    CreateDir("MagicEditor/Assets")
    CreateShortCut( "MagicEditor/Wwise" , "MagicEditor\Assets\Wwise.lnk" )

    #创建plugins的链接
    CreateShortCut( "MagicEditor/Assets/Plugins/DOTween" , "MagicClient/Assets/Plugins/DOTween.lnk" )
    CreateShortCut( "MagicEditor/Assets/Plugins/ProtoBufferNet" , "MagicClient/Assets/Plugins/ProtoBufferNet.lnk" )
    CreateShortCut( "MagicEditor/Assets/Plugins/Simplygon" , "MagicClient/Assets/Plugins/Simplygon.lnk" )
    CreateShortCut( "MagicEditor/Assets/Bakery" , "MagicClient/Assets/Bakery.lnk" )
    CreateShortCut( "MagicEditor/Assets/S3Unity" , "MagicClient/Assets/S3Unity.lnk" )
    CreateShortCut( "MagicEditor/Assets/PostProcessing Profiles" , "MagicClient/Assets/PostProcessing Profiles.lnk" )
    CreateShortCut( "MagicEditor/Assets/UniStorm3" , "MagicClient/Assets/UniStorm3.lnk" )
    CreateShortCut( "MagicEditor/Assets/uNature" , "MagicClient/Assets/uNature.lnk" )
    CreateShortCut( "MagicEditor/Assets/ReachableGames" , "MagicClient/Assets/ReachableGames.lnk" )
    CreateShortCut( "MagicEditor/Assets/AmplifyShaderEditor" , "MagicClient/Assets/AmplifyShaderEditor.lnk" )
    CreateShortCut( "MagicEditor/Assets/Thirdparty" , "MagicClient/Assets/Thirdparty.lnk" )
    CreateShortCut( "MagicEditor/Assets/Script/EditorBehaviours" , "MagicClient/Assets/Script/EditorBehaviours.lnk" )
	
	#创建各种代码的链接
    CreateDir("MagicClient/Assets/Script")
    CreateShortCut( "MagicGameCode\Projects\GameBattle" , "MagicClient\Assets\Script\GameBattle.lnk" )
    CreateShortCut( "MagicGameCode\Projects\GameCommon" , "MagicClient\Assets\Script\GameCommon.lnk" )
    CreateShortCut( "MagicGameCode\Projects\GameCougar" , "MagicClient\Assets\Script\GameCougar.lnk" )
    CreateShortCut( "MagicGameCode\Projects\GameFramework" , "MagicClient\Assets\Script\GameFramework.lnk" )
    CreateShortCut( "MagicGameCode\Projects\GameLogic" , "MagicClient\Assets\Script\GameLogic.lnk" )
    CreateShortCut( "MagicEditor/Assets/Script/GamePresentation" , "MagicClient\Assets\Script\GamePresentation.lnk" )
    CreateShortCut( "MagicGameCode\Projects\GameEditor" , "MagicClient\Assets\Script\GameEditor.lnk" )
    CreateShortCut( "MagicGameCode\Projects\GamePersistent" , "MagicClient\Assets\Script\GamePersistent.lnk" )
    CreateShortCut( "MagicGameCode\Projects\GameTest" , "MagicClient\Assets\Script\GameTest.lnk" )
    CreateShortCut( "MagicGameCode\Projects\GameThirdParty" , "MagicClient\Assets\Script\GameThirdParty.lnk" )
    CreateShortCut( "MagicGameCode\Projects\GameServer" , "MagicClient\Assets\Script\GameServer.lnk" )
    CreateShortCut( "MagicGameCode\Projects\GameUpdate" , "MagicClient\Assets\Script\GameUpdate.lnk" )

#删除所有的软链接
def UnLink():

  #删除各种代码的链接
  DeleteShortCut("MagicClient\Assets\Script\GameBattle.lnk" )
  DeleteShortCut("MagicClient\Assets\Script\GameCommon.lnk" )
  DeleteShortCut( "MagicClient\Assets\Script\GameCougar.lnk" )
  DeleteShortCut("MagicClient\Assets\Script\GameFramework.lnk" )
  DeleteShortCut("MagicClient\Assets\Script\GameLogic.lnk" )
  DeleteShortCut( "MagicClient\Assets\Script\GamePresentation.lnk" )
  DeleteShortCut( "MagicClient\Assets\Script\GameEditor.lnk" )
  DeleteShortCut( "MagicClient\Assets\Script\GamePersistent.lnk" )
  DeleteShortCut("MagicClient\Assets\Script\GameTest.lnk" )
  DeleteShortCut( "MagicClient\Assets\Script\GameThirdParty.lnk" )
  DeleteShortCut( "MagicClient\Assets\Script\GameServer.lnk" )
  DeleteShortCut("MagicClient\Assets\Script\GameUpdate.lnk" )

  #删除各种资源文件夹的链接
  DeleteShortCut("MagicGameCode/Projects/GamePresentation.lnk")
  DeleteShortCut("MagicGameCode/Projects/GameEditor/SkillEditor.lnk")
  DeleteShortCut("MagicEditor/Resources/LuaScripts.lnk")
  DeleteShortCut("MagicClient/Assets/Resources.lnk")
  DeleteShortCut("MagicEditor/Assets/Resources.lnk")
  DeleteShortCut("MagicClient/Assets/StreamingAssets/res.lnk")
  DeleteShortCut("MagicClient/Assets/StreamingAssets/Audio/GeneratedSoundBanks/Windows.lnk")
  DeleteShortCut("MagicClient/Assets/StreamingAssets/Audio/GeneratedSoundBanks/Android.lnk")
  DeleteShortCut("MagicClient/Assets/StreamingAssets/Audio/GeneratedSoundBanks/iOS.lnk")
  #DeleteShortCut("MagicClient/Assets/StreamingAssets.lnk")
  DeleteShortCut("MagicEditor/Assets/StreamingAssets.lnk")
  DeleteShortCut("MagicEditor/Assets/Wwise.lnk")
  #删除plugins的链接
  DeleteShortCut("MagicClient/Assets/Plugins/DOTween.lnk")
  DeleteShortCut("MagicClient/Assets/Plugins/ProtoBufferNet.lnk")
  DeleteShortCut("MagicClient/Assets/Plugins/Simplygon.lnk")
  DeleteShortCut("MagicClient/Assets/Bakery.lnk")
  DeleteShortCut("MagicClient/Assets/S3Unity.lnk")
  DeleteShortCut("MagicClient/Assets/PostProcessing Profiles.lnk")
  DeleteShortCut("MagicClient/Assets/UniStorm3.lnk")
  DeleteShortCut("MagicClient/Assets/uNature.lnk")
  DeleteShortCut("MagicClient/Assets/ReachableGames.lnk")
  DeleteShortCut("MagicClient/Assets/AmplifyShaderEditor.lnk")
  DeleteShortCut("MagicClient/Assets/Thirdparty.lnk")
  DeleteShortCut("MagicClient/Assets/Script/EditorBehaviours.lnk")

def mainOperate():
  #参数获取
  parser = argparse.ArgumentParser(u"工程部署脚本入口")
  parser.add_argument('-p', '--platform', help=u"平台选择：0代表windows 1代表android 2代表ios", dest="platform", default=[0], choices=[0,1,2], type=int, action='store', nargs=1)
  parser.add_argument('-sb', '--svnbranch', help=u"svnbranch", dest="svn_branch", action='store', default= "trunk/MagicEditor")
  parser.add_argument('-ol', '--only-link', help=u"link res", dest="only_link", action='store_true', default=False)
  parser.add_argument('-ou', '--only-unlink', help=u"unlink res", dest="only_unlink", action='store_true', default=False)
  args = parser.parse_args()

  #创建链接
  if args.only_link:
    Link(int(args.platform[0])) 
    return

  if args.only_unlink:
    UnLink()
    return

  #操作执行
  #UpdateSVN() #更新svn
  Link(int(args.platform[0])) #创建链接
  
if __name__ == "__main__":
  print("new Main.py")
  mainOperate()
  print('over')
  os.system("pause")
