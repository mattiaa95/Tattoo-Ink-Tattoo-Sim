using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace AdvancedMobilePaint
{
	public class PaintUtils {
		
		#region TextureTransforms
		//within this region all texture2d transformation utilities functions will be implemented
		
		/// <summary>
		/// Rotates the texture.
		/// </summary>
		/// <returns>The texture.</returns>
		/// <param name="tex">Texture which bitmap will be rotated</param>
		/// <param name="angle">Roatation angle in degrees.</param>
		public static Texture2D RotateTexture(Texture2D tex, float angle)
		{
			Debug.Log("rotating");
			Texture2D rotImage = new Texture2D(tex.width, tex.height,tex.format,false);
			int  x,y;
			float x1, y1, x2,y2;
			
			int w = tex.width;
			int h = tex.height;
			float x0 = rot_x (angle, -w/2.0f, -h/2.0f) + w/2.0f;
			float y0 = rot_y (angle, -w/2.0f, -h/2.0f) + h/2.0f;
			
			float dx_x = rot_x (angle, 1.0f, 0.0f);
			float dx_y = rot_y (angle, 1.0f, 0.0f);
			float dy_x = rot_x (angle, 0.0f, 1.0f);
			float dy_y = rot_y (angle, 0.0f, 1.0f);
			
			
			x1 = x0;
			y1 = y0;
			Color32 [] pixels= tex.GetPixels32(0);
			Color32 [] result=new Color32[pixels.Length];
			Color c;
			int pixX=0;
			int pixY=0;
//			int pixelPos=0;
			for (x = 0; x < tex.width; x++) {
				x2 = x1;
				y2 = y1;
				for ( y = 0; y < tex.height; y++) {
					//rotImage.SetPixel (x1, y1, Color.clear);          
					
					x2 += dx_x;//rot_x(angle, x1, y1);
					y2 += dx_y;//rot_y(angle, x1, y1);
					//
					pixX = (int) Mathf.Floor(x2);
					pixY = (int) Mathf.Floor(y2);
					if(pixX >= tex.width || pixX < 0 ||
					   pixY >= tex.height || pixY < 0) {
						c = Color.clear;
					} else {
						c=(Color)pixels[pixX*w+pixY];// = tex.GetPixel(x1,y1);
					}
					//
					//rotImage.SetPixel ( (int)Mathf.Floor(x), (int)Mathf.Floor(y), /*c*/getPixel(tex,x2, y2));
					result[(int)Mathf.Floor(x)*w+(int)Mathf.Floor(y)]=(Color32)c;
					//pixelPos++;
				}
				
				x1 += dy_x;
				y1 += dy_y;
				
			}
			rotImage.SetPixels32(result);
			rotImage.Apply();
			return rotImage;
		}
		
//		private static  Color getPixel(Texture2D tex, float x, float y)
//		{
//			Color pix;
//			int x1 = (int) Mathf.Floor(x);
//			int y1 = (int) Mathf.Floor(y);
//			
//			if(x1 > tex.width || x1 < 0 ||
//			   y1 > tex.height || y1 < 0) {
//				pix = Color.clear;
//			} else {
//				pix = tex.GetPixel(x1,y1);
//			}
//			
//			return pix;
//		}
		
		private static float rot_x (float angle, float x, float y) {
			float cos = Mathf.Cos(angle/180.0f*Mathf.PI);
			float sin = Mathf.Sin(angle/180.0f*Mathf.PI);
			return (x * cos + y * (-sin));
		}
		private static float rot_y (float angle, float x, float y) {
			float cos = Mathf.Cos(angle/180.0f*Mathf.PI);
			float sin = Mathf.Sin(angle/180.0f*Mathf.PI);
			return (x * sin + y * cos);
		}
		  
		  
		
		#endregion TextureTransforms
		
		#region Misc
		//miscelanious stuff will be implemented here 
		/// <summary>
		/// Reads the unreadable texture.
		/// </summary>
		/// <returns>The readable texture.</returns>
		/// <param name="texture">Texture whiich data will be read.</param>
		public static Texture2D ReadUnreadableTexture(Texture2D texture/*, Texture2D outputTexture*/)
		{
			//https://support.unity3d.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-
			// Create a temporary RenderTexture of the same size as the texture
			RenderTexture tmp = RenderTexture.GetTemporary(texture.width,texture.height,0,RenderTextureFormat.Default,RenderTextureReadWrite.Linear);
			// Blit the pixels on texture to the RenderTexture
			Graphics.Blit(texture, tmp);
			// Backup the currently set RenderTexture
			RenderTexture previous = RenderTexture.active;
			// Set the current RenderTexture to the temporary one we created
			RenderTexture.active = tmp;
			// Create a new readable Texture2D to copy the pixels to it
			Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
			// Copy the pixels from the RenderTexture to the new Texture
			myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
			myTexture2D.Apply();
			// Reset the active RenderTexture
			RenderTexture.active = previous;
			// Release the temporary RenderTexture
			RenderTexture.ReleaseTemporary(tmp);
			
			
			// "myTexture2D" now has the same pixels from "texture" and it's readable.
			Debug.Log ("READ DONE!");
			/*outputTexture=*/ return myTexture2D;
		
		}
		
		/// <summary>
		/// Converts the Sprite to Texture2D.
		/// </summary>
		/// <returns>o Texture2D.</returns>
		/// <param name="sr">Sprite</param>
		public static Texture2D ConvertSpriteToTexture2D(Sprite sr)
		{
			
			//sr.sprite = patterns [index];
			Texture2D texTex = new Texture2D ((int)sr.textureRect.width,(int) sr.textureRect.height, TextureFormat.ARGB32, false);
			Color[] tmp = sr.texture.GetPixels((int)sr.textureRect.x, 
			                                   (int)sr.textureRect.y, 
			                                   (int)sr.textureRect.width, 
			                                   (int)sr.textureRect.height,0 );
			texTex.SetPixels( tmp,0 );
			texTex.Apply (false, false);
			return texTex;
		}
		
		/// <summary>
		/// Generates the drawing mask based on texture.
		/// Funkcija koja generise odgovarajucu masku na osnovu izvorisne teksture.
		/// </summary>
		/// <param name="source">Source.</param>
		public static Texture2D GenerateDrawingMaskBasedOnTexture(Texture2D source)
		{
//			Color t = Color.clear;
//			Color b = Color.black;
//			b.a=1;
//			t.a=0;
			Texture2D drawingMask =  Texture2D.blackTexture;
			drawingMask.Resize(source.width, source.height, TextureFormat.ARGB32, false) ;
			drawingMask.filterMode = FilterMode.Bilinear;
			Color[] colorArray=source.GetPixels();
			for (int i=0; i<source.width; i++)
			for (int j=0; j<source.height; j++) {
				//Color c= source.GetPixel(i,j);
				
				if(/*c.a*/colorArray[j*source.width+i].a>0.5f)
				{
					//drawingMask.SetPixel(i,j,t);
					colorArray[j*source.width+i].a=0;
				}
				else
					//drawingMask.SetPixel(i,j,b);
					colorArray[j*source.width+i].a=1;
			}
			drawingMask.Apply (false);
			return drawingMask;		
		}
		
		#endregion Misc
		
		
	}
}
