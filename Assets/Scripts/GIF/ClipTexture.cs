using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Converts and applies textures from a TextAsset to the material of the Renderer in order to emulate a MovieTexture.
/// </summary>
[RequireComponent(typeof(RawImage))]
[AddComponentMenu("Rendering/Clip Texture")]
[ExecuteInEditMode]
public class ClipTexture : MonoBehaviour {
	/// <summary>
	/// The TextAsset, containing the individual frames of the clip.
	/// </summary>
	public TextAsset clipTexture;
	/// <summary>
	/// Should the clip start over if finished.
	/// </summary>
	public bool loop;
	/// <summary>
	/// Should the clip start playing automatically
	/// </summary>
	public bool playAutomatically;
	/// <summary>
	/// Framerate the clip will be played at.
	/// </summary>
	public float fps = 15f;

	Texture2D texture;
	byte[][] frames;

	bool isPlaying = false, isLoading = false;
	/// <summary>
	/// Returns whether the clip is playing or not.
	/// </summary>
	/// <value><c>true</c> if the clip is playing; otherwise, <c>false</c>.</value>
	public bool IsPlaying{
		get{ return isPlaying; }
	}
	/// <summary>
	/// Returns wheter a new clip is being loaded from a TextAsset.
	/// </summary>
	/// <value><c>true</c> if loading the clip from a TextAsset has not finished; otherwise, <c>false</c>.</value>
	public bool IsLoading{
		get{ return isLoading; }
	}

	void Awake () {
		texture = new Texture2D(128, 128);
		GetComponent<RawImage>().texture = texture;
		

		if(clipTexture != null){
			SetClipTexture(ref clipTexture);
		}

		if(playAutomatically){
			PlayClip();
		}
	}

	/// <summary>
	/// Sets the TextAsset containing the individual frames of the clip.
	/// </summary>
	/// <param name="clipTexture">ClipTexture file as TextAsset.</param>
	public void SetClipTexture(ref TextAsset clipTexture){
		if(!isPlaying && !isLoading){
			isLoading = true;
			frames = SplitLinesIntoBytes(clipTexture.text);
			isLoading = false;
		}
	}
		
	/// <summary>
	/// Plays the current clip.
	/// </summary>
	/// <param name="frameIndex">Index of the starting frame in the clip</param>
	public void PlayClip(int frameIndex = 0){
		StartCoroutine(PlayClipAsync(frameIndex));
	}

	/// <summary>
	/// Plays the current clip.
	/// </summary>
	/// <param name="clipTexure">ClipTexture file as TextAsset.</param>
	/// <param name="frameIndex">Index of the starting frame in the clip</param>
	public void PlayClip(ref TextAsset clipTexure, int frameIndex = 0){
		SetClipTexture( ref clipTexure);
		if(!isPlaying && !isLoading){
			StartCoroutine(PlayClipAsync(frameIndex));
		}
	}

	/// <summary>
	/// Stops the playback of the clip.
	/// </summary>
	public void StopClip(){
		StopCoroutine(PlayClipAsync());
	}

	IEnumerator PlayClipAsync(int frameIndex = 0){
		isPlaying = true;
		frameIndex %= frames.Length;
		do{
			for(var i = frameIndex; i < frames.Length; i++){
				texture.LoadImage(frames[i]);
				yield return new WaitForSeconds(1f / fps);
			}
			frameIndex = 0;
		}while(loop);
		isPlaying = false;
	}

	byte[][]SplitLinesIntoBytes(string text){
		var frames = text.Split('\n');
		var res = new byte[frames.Length][];
		for(var i = 0; i < frames.Length; i++){
			res[i] = Convert.FromBase64String(frames[i]);
		}
		return res;
	}
}