#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;

public class ClipTextureEditor : EditorWindow {
	[MenuItem ("Assets/Create/Clip Texture", false, 304)]
	static void ConvertGifToClipTexture(){
		var projectFolder = String.Empty;
		foreach(var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)){
			projectFolder = AssetDatabase.GetAssetPath(obj);
			if(!string.IsNullOrEmpty(projectFolder) && File.Exists(projectFolder)){
				projectFolder = Path.GetDirectoryName(projectFolder);
				break;
			}
		}

		var path = EditorUtility.OpenFilePanel("Choose a gif to convert into a Clip Texture", "", "gif");
		if(path.Length == 0){ Debug.LogError("No file selected!"); return; }

		var window = GetWindow(typeof(ClipTextureEditor));
		window.Show();

		var res = string.Empty;
		var img = Image.FromFile(path);
		var dimension = new FrameDimension(img.FrameDimensionsList[0]);
		var frameCount = img.GetFrameCount(dimension);

		for(var currentFrame = 0; currentFrame < frameCount; currentFrame++){
			using(var memory = new MemoryStream()){
				EditorUtility.DisplayProgressBar("Processing GIF", string.Format("Processing frame {0}/{1}", currentFrame +1, frameCount), (float)(currentFrame +1)/frameCount);
				img.SelectActiveFrame(dimension, currentFrame);
				img.Save(memory, ImageFormat.Png);
				res += '\n' + Convert.ToBase64String(memory.ToArray());
			}
		}
		res = res.Substring(1); //remove the first newline character

		if(!projectFolder.Equals(String.Empty) && !projectFolder.Equals("Assets")){
			projectFolder = projectFolder.Substring(6);
		}
		else{
			projectFolder = String.Empty;
		}

		var nn = path.Split('/');
		nn = nn[nn.Length -1].Split('\\');
		var resFileName = nn[nn.Length -1].Substring(0, nn[nn.Length -1].Length -4);

		path = Application.dataPath + projectFolder + '/' + resFileName; //"/New ClipTexture";
		if(File.Exists(path + ".txt")){
			var i = 1;
			while(File.Exists(path + i + ".txt")){
				i++;
			}
			path += i;
		}
		path += ".txt";
		File.WriteAllText(path, res);
		AssetDatabase.Refresh();
		EditorUtility.ClearProgressBar();
		window.Close();
		Debug.Log(String.Format("Clip Texture file created from {0} frames: {1}", img.GetFrameCount(dimension), path));
		img.Dispose();
	}
}
#endif