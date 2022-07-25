using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace AdvancedMobilePaint.Tools
{
	public class VectorBrushesTools {
	
	
		#region VectorBrush
		
		public static void DrawLineWithVectorBrush(Vector2 start, Vector2 end,AdvancedMobilePaint paintEngine)
		{
			int x0=(int)start.x;
			int y0=(int)start.y;
			int x1=(int)end.x;
			int y1=(int)end.y;
			int dx= Mathf.Abs(x1-x0); // TODO: try these? http://stackoverflow.com/questions/6114099/fast-integer-abs-function
			int dy= Mathf.Abs(y1-y0);
			int sx,sy;
			if (x0 < x1) {sx=1;}else{sx=-1;}
			if (y0 < y1) {sy=1;}else{sy=-1;}
			int err=dx-dy;
			bool loop=true;
			//			int minDistance=brushSize-1;
			int minDistanceX=(int)(paintEngine.customBrushWidth>>1); // divide by 2, you might want to set mindistance to smaller value, to avoid gaps between brushes when moving fast
			int minDistanceY=(int)(paintEngine.customBrushHeight>>1);
			int pixelCount=0;
			int e2;
			while (loop) 
			{
				pixelCount++;
				if (pixelCount>minDistanceX || pixelCount>minDistanceY)
				{
					pixelCount=0;
					DrawRectangle(x0,y0,paintEngine);
				}
				if ((x0 == x1) && (y0 == y1)) loop=false;
				e2 = 2*err;
				if (e2 > -dy)
				{
					err = err - dy;
					x0 = x0 + sx;
				}
				if (e2 <  dx)
				{
					err = err + dx;
					y0 = y0 + sy;
				}
			}
		}
		
		public static void DrawRectangle(int px, int py,AdvancedMobilePaint paintEngine)
		{
			//Debug.Log ("DrawRectangle");
			int startX=/*(int)*/(px-paintEngine.customBrushWidth/2);
			int startY=/*(int)*/(py-paintEngine.customBrushHeight/2);
			int pixel = (paintEngine.texWidth*startY+startX)*4;
			//int brushPixel = 0;
			bool skip=false;
			//float yy =0;
			//float xx =0;
			//int pixel2 = 0;
			for (int y = 0; y < paintEngine.customBrushHeight; y++) 
			{
				for (int x = 0; x < paintEngine.customBrushWidth; x++) 
				{
					skip=false;
					if((startX+x)>(paintEngine.texWidth-2) || (startX+x)<-1 ) skip=true;
					if(pixel<0|| pixel>=paintEngine.pixels.Length)
						skip=true;//
					if(!skip)
					{
						//TODO: add more modes
						paintEngine.pixels[pixel] =paintEngine.paintColor.r;
						paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
						paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
						paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
					pixel+= 4;
				}//za x
				
				pixel = (paintEngine.texWidth*(startY==0?-1:startY+y)+startX+1)*4;
			}//za y
		}//DrawRectabgle() end

		public static void DrawPatternRectangle(int px, int py,AdvancedMobilePaint paintEngine)
		{
			Debug.Log ("DrawPatternRectangle");
			int startX=/*(int)*/(px-paintEngine.customBrushWidth/2);
			int startY=/*(int)*/(py-paintEngine.customBrushHeight/2);
			int pixel = (paintEngine.texWidth*startY+startX)*4;
			//int brushPixel = 0;
			bool skip=false;
			float yy =0;
			float xx =0;
			int pixel2 = 0;
			for (int y = 0; y < paintEngine.customBrushHeight; y++) 
			{
				for (int x = 0; x < paintEngine.customBrushWidth; x++) 
				{
					skip=false;
					if((startX+x)>(paintEngine.texWidth-2) || (startX+x)<-1 ) skip=true;
					if(pixel<0|| pixel>=paintEngine.pixels.Length)
						skip=true;//
					if(!skip)
					{
						//TODO: add more modes
						/*float*/ yy = Mathf.Repeat(y+startY,paintEngine.customPatternWidth);
						/*float*/ xx = Mathf.Repeat(x+startX,paintEngine.customPatternWidth);//Debug.Log ("P"+xx+","+yy);
						/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx+yy)*4, paintEngine.patternBrushBytes.Length);
						paintEngine.pixels[pixel] = paintEngine.patternBrushBytes[pixel2];//r
						paintEngine.pixels[pixel+1] = paintEngine.patternBrushBytes[pixel2+1];//g
						paintEngine.pixels[pixel+2] = paintEngine.patternBrushBytes[pixel2+2];//b
						
						paintEngine.pixels[pixel+3] = paintEngine.patternBrushBytes[pixel2+3];//a
					}
					pixel+= 4;
				}//za x
				
				pixel = (paintEngine.texWidth*(startY==0?-1:startY+y)+startX+1)*4;
			}//za y
		}//DrawRectabgle() end
		
		// main painting function, http://stackoverflow.com/a/24453110
		/// <summary>
		/// Draws the circle.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public static void DrawCircle(int x,int y, AdvancedMobilePaint paintEngine)
		{
			if (!paintEngine.canDrawOnBlack)
			{
				
				if (paintEngine.pixels[(paintEngine.texWidth*y+x)*4]==0 && paintEngine.pixels[(paintEngine.texWidth*y+x)*4+1]==0 && paintEngine.pixels[(paintEngine.texWidth*y+x)*4+2]==0 && paintEngine.pixels[(paintEngine.texWidth*y+x)*4+3]!=0) return;
			}
			
			int pixel = 0;
			
			// draw fast circle: 
			int r2 = paintEngine.brushSize * paintEngine.brushSize;
			int area = r2 << 2;
			int rr = paintEngine.brushSize << 1;
			float lerpVal=1f;
			lerpVal=paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength;
			for (int i = 0; i < area; i++)
			{
				int tx = (i % rr) - paintEngine.brushSize;
				int ty = (i / rr) - paintEngine.brushSize;
				if (tx * tx + ty * ty < r2)
				{
					if (x+tx<0 || y+ty<0 || x+tx>=paintEngine.texWidth || y+ty>=paintEngine.texHeight) continue; // temporary fix for corner painting
					
					
					pixel = (paintEngine.texWidth*(y+ty)+x+tx)*4;
					//pixel = ( texWidth*( (y+ty) % texHeight )+ (x+tx) % texWidth )*4;
					
					if (paintEngine.useAdditiveColors)
					{
						// additive over white also
						if (!paintEngine.useLockArea || (paintEngine.useLockArea && paintEngine.lockMaskPixels[pixel]==1))
						{
							//toLerpVal=
							paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel],paintEngine.paintColor.r,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
							paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],paintEngine.paintColor.g,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
							paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],paintEngine.paintColor.b,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
							paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],paintEngine.paintColor.a,lerpVal/*paintEngine.paintColor.a/255*paintEngine.brushAlphaStrength*/);
						}
						
					}else{ // no additive, just paint my colors
						
						if (!paintEngine.useLockArea || (paintEngine.useLockArea && paintEngine.lockMaskPixels[pixel]==1))
						{
							paintEngine.pixels[pixel] = paintEngine.paintColor.r;
							paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
							paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
							paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
						}
						
					} // if additive
				} // if in circle
			} // for area
		} // DrawCircle()
		/// <summary>
		/// Draws the pattern circle.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public static void DrawPatternCircle(int x,int y,AdvancedMobilePaint paintEngine)
		{
			//Debug.Log ("DrawPatternCircle "+ x +" ,"+y);
			
			if (!paintEngine.canDrawOnBlack)
			{
				if (paintEngine.pixels[(paintEngine.texWidth*y+x)*4]==0 && paintEngine.pixels[(paintEngine.texWidth*y+x)*4+1]==0 && paintEngine.pixels[(paintEngine.texWidth*y+x)*4+2]==0 && paintEngine.pixels[(paintEngine.texWidth*y+x)*4+3]!=0) return;
			}
			
			int pixel = 0;
			
			// draw fast circle: 
			int r2 = paintEngine.brushSize * paintEngine.brushSize;//povrsina kruga
			int area = r2 << 2;//sve rgb vrednosti koje cine povrsinu kruga -piksela u krugu
			int rr = paintEngine.brushSize << 1;//precnik kruga
			int tx=0;
			int ty=0;
			float yy=0;
			float xx=0;
			int pixel2=0;
			float lerpVal=1f;
			for (int i = 0; i < area; i++)
			{
				/*int*/ tx = (i % rr) - paintEngine.brushSize;
				/*int*/ ty = (i / rr) - paintEngine.brushSize;
				
				if (tx * tx + ty * ty < r2)//(if in circle) 
				{
					if (x+tx<0 || y+ty<0 || x+tx>=paintEngine.texWidth || y+ty>=paintEngine.texHeight) continue; // temporary fix for corner painting
					
					pixel = (paintEngine.texWidth*(y+ty)+x+tx)*4; // << 2
					//if(pixel<0 || pixel>pixels.Length) continue;
					if (paintEngine.useAdditiveColors)
					{
						// additive over white also
						if (!paintEngine.useLockArea || (paintEngine.useLockArea && paintEngine.lockMaskPixels[pixel]==1))
						{
							
							/*float*/ yy = Mathf.Repeat(y+ty,paintEngine.customPatternWidth);
							/*float*/ xx = Mathf.Repeat(x+tx,paintEngine.customPatternWidth);
							/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx+yy)*4, paintEngine.patternBrushBytes.Length);
							lerpVal=paintEngine.patternBrushBytes[pixel2+3]/255f*paintEngine.brushAlphaStrength;
							paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel], paintEngine.patternBrushBytes[pixel2],lerpVal/*paintEngine.patternBrushBytes[pixel2+3]/255f*paintEngine.brushAlphaStrength*/);
							paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],paintEngine.patternBrushBytes[pixel2+1],lerpVal/*paintEngine.patternBrushBytes[pixel2+3]/255f*paintEngine.brushAlphaStrength*/);
							paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],paintEngine.patternBrushBytes[pixel2+2],lerpVal/*paintEngine.patternBrushBytes[pixel2+3]/255f*paintEngine.brushAlphaStrength*/);
							paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],paintEngine.patternBrushBytes[pixel2+3],lerpVal/*paintEngine.patternBrushBytes[pixel2+3]/255f*paintEngine.brushAlphaStrength*/);
						}
						
					}else{ // no additive, just paint my colors
						
						if (!paintEngine.useLockArea || (paintEngine.useLockArea && paintEngine.lockMaskPixels[pixel]==1))
						{
							// TODO: pattern dynamic scalar value?
							
							/*float*/ yy = Mathf.Repeat(y+ty,paintEngine.customPatternWidth);
							/*float*/ xx = Mathf.Repeat(x+tx,paintEngine.customPatternWidth);//Debug.Log ("P"+xx+","+yy);
							/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx+yy)*4, paintEngine.patternBrushBytes.Length);
							paintEngine.pixels[pixel] = paintEngine.patternBrushBytes[pixel2];//r
							paintEngine.pixels[pixel+1] = paintEngine.patternBrushBytes[pixel2+1];//g
							paintEngine.pixels[pixel+2] = paintEngine.patternBrushBytes[pixel2+2];//b
							
							paintEngine.pixels[pixel+3] = paintEngine.patternBrushBytes[pixel2+3];//a
							//}
						}
						
					} // if additive
				} // if in circle
			} // for area
			
		} // DrawPatternCircle()
		
		
		// draw line between 2 points (if moved too far/fast)
		// http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
		public static void DrawLine(Vector2 start, Vector2 end, AdvancedMobilePaint paintEngine)
		{
			int x0=(int)start.x;
			int y0=(int)start.y;
			int x1=(int)end.x;
			int y1=(int)end.y;
			int dx= Mathf.Abs(x1-x0); // TODO: try these? http://stackoverflow.com/questions/6114099/fast-integer-abs-function
			int dy= Mathf.Abs(y1-y0);
			int sx,sy;
			if (x0 < x1) {sx=1;}else{sx=-1;}
			if (y0 < y1) {sy=1;}else{sy=-1;}
			int err=dx-dy;
			bool loop=true;
			//			int minDistance=brushSize-1;
			int minDistance=(int)(paintEngine.brushSize>>1); // divide by 2, you might want to set mindistance to smaller value, to avoid gaps between brushes when moving fast
			int pixelCount=0;
			int e2;
			while (loop) 
			{
				pixelCount++;
				if (pixelCount>minDistance)
				{
					pixelCount=0;
					DrawCircle(x0,y0,paintEngine);
				}
				if ((x0 == x1) && (y0 == y1)) loop=false;
				e2 = 2*err;
				if (e2 > -dy)
				{
					err = err - dy;
					x0 = x0 + sx;
				}
				if (e2 <  dx)
				{
					err = err + dx;
					y0 = y0 + sy;
				}
			}
		} // drawline
		
		
		public static void DrawLineWithPattern(Vector2 start, Vector2 end,AdvancedMobilePaint paintEngine)
		{
			int x0=(int)start.x;
			int y0=(int)start.y;
			int x1=(int)end.x;
			int y1=(int)end.y;
			int dx= Mathf.Abs(x1-x0); // TODO: try these? http://stackoverflow.com/questions/6114099/fast-integer-abs-function
			int dy= Mathf.Abs(y1-y0);
			int sx,sy;
			if (x0 < x1) {sx=1;}else{sx=-1;}
			if (y0 < y1) {sy=1;}else{sy=-1;}
			int err=dx-dy;
			bool loop=true;
			//			int minDistance=brushSize-1;
			int minDistance=(int)(paintEngine.brushSize>>1); // divide by 2, you might want to set mindistance to smaller value, to avoid gaps between brushes when moving fast
			int pixelCount=0;
			int e2;
			while (loop) 
			{
				pixelCount++;
				if (pixelCount>minDistance)
				{
					pixelCount=0;
					DrawPatternCircle(x0,y0,paintEngine);
				}
				if ((x0 == x1) && (y0 == y1)) loop=false;
				e2 = 2*err;
				if (e2 > -dy)
				{
					err = err - dy;
					x0 = x0 + sx;
				}
				if (e2 <  dx)
				{
					err = err + dx;
					y0 = y0 + sy;
				}
			}
		}
		
		
		
		#endregion VectorBrush

	}
}