using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace AdvancedMobilePaint.Tools
{
	public /*static*/ class BitmapBrushesTools {
	
		public static void DrawLineWithBrush(Vector2 start, Vector2 end,AdvancedMobilePaint paintEngine)
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
					DrawCustomBrush2(x0,y0,paintEngine);
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
		
		
		/// <summary>
		/// Draws the custom brush (v2).
		/// </summary>
		/// <param name="px">Px.</param>
		/// <param name="py">Py.</param>
		public static void DrawCustomBrush2(int px,int py,AdvancedMobilePaint paintEngine)
		{

			// TODO: this function needs comments/info..
			//Debug.Log ("DrawCustomBrush2");
			// get position where we paint
			int startX=/*(int)*/(px-paintEngine.customBrushWidth/2);
			int startY=/*(int)*/(py-paintEngine.customBrushHeight/2);
			int pixel = (paintEngine.texWidth*startY+startX)*4;
			int brushPixel = 0;
			bool skip=false;
			float yy =0;
			float xx =0;
			int pixel2 = 0;
			float lerpVal=1f;
			lerpVal=paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength;
			for (int y = 0; y < paintEngine.customBrushHeight; y++) 
			{
				for (int x = 0; x < paintEngine.customBrushWidth; x++) 
				{
					//brushColor = (customBrushPixels[x*customBrushWidth+y]);
					//FIX
					brushPixel = (paintEngine.customBrushWidth*(y)+x)*4;
					skip=false;
					if((startX+x)>(paintEngine.texWidth-2) || (startX+x)<-1 ) skip=true;
					//if((startY+y)>(texWidth+2) || (startY+y)<-1 ) skip=true;
					if(pixel<0|| pixel>=paintEngine.pixels.Length)
						skip=true;//
					if(brushPixel<0 || brushPixel>paintEngine.customBrushBytes.Length) skip=true;
					if(!paintEngine.canDrawOnBlack && !skip)
					{
						if(paintEngine.pixels[pixel+3]!=0 && paintEngine.pixels[pixel]==0 && paintEngine.pixels[pixel+1]==0 && paintEngine.pixels[pixel+2]==0) skip=true;
					}

				 	if(paintEngine.lockMaskPixels.Length==0) return; //----novo


					// brush alpha is over 0 in this pixel?
					if ( paintEngine.customBrushBytes[brushPixel+3]!=0 && !skip)
						//END FIX
					{
						
						// take alpha from brush?
						if (paintEngine.useCustomBrushAlpha)
						{
							if (paintEngine.useAdditiveColors)
							{
								
								// additive over white also
								if((paintEngine.useLockArea && paintEngine.lockMaskPixels[pixel]==1) || !paintEngine.useLockArea){//this enables custom brushes using lock mask
									lerpVal=paintEngine.customBrushBytes[brushPixel+3]/255f;
									switch(paintEngine.brushMode){
									case BrushProperties.Clear:
										//TODO
										paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel],paintEngine.clearColor.r,lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],paintEngine.clearColor.g,lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],paintEngine.clearColor.b,lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],paintEngine.clearColor.a,lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										break;
									case BrushProperties.Default:
										//TODO
										paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel],paintEngine.paintColor.r,lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],paintEngine.paintColor.g,lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],paintEngine.paintColor.b,lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],paintEngine.paintColor.a,lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										break;
									case BrushProperties.Simple:
										paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel],paintEngine.customBrushBytes[brushPixel],lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],paintEngine.customBrushBytes[brushPixel+1],lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],paintEngine.customBrushBytes[brushPixel+2],lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],paintEngine.customBrushBytes[brushPixel+3],lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										break;
									case BrushProperties.Pattern:
										//TODO
										/*float*/ yy = Mathf.Repeat(/*py+y*/py+(y-paintEngine.customBrushHeight/2f),paintEngine.customPatternHeight);
										/*float*/ xx = Mathf.Repeat(/*px+x*/px+(x-paintEngine.customBrushWidth/2f),paintEngine.customPatternWidth);
										/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx +yy)*4, paintEngine.patternBrushBytes.Length);
										paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel],paintEngine.patternBrushBytes[pixel2],lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],paintEngine.patternBrushBytes[pixel2+1],lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],paintEngine.patternBrushBytes[pixel2+2],lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],paintEngine.patternBrushBytes[pixel2+3],lerpVal/*paintEngine.customBrushBytes[brushPixel+3]/255f*/);
										break;
										
									}
								}
								
							}else{ 
								//TODO
								// no additive colors
								if((paintEngine.useLockArea && paintEngine.lockMaskPixels[pixel]==1) || !paintEngine.useLockArea){
									switch (paintEngine.brushMode){
									case BrushProperties.Clear :
										paintEngine.pixels[pixel] =paintEngine.clearColor.r;
										paintEngine.pixels[pixel+1] = paintEngine.clearColor.g;
										paintEngine.pixels[pixel+2] = paintEngine.clearColor.b;
										paintEngine.pixels[pixel+3] = paintEngine.clearColor.a;
										break;
									case BrushProperties.Default:
										paintEngine.pixels[pixel] =paintEngine.paintColor.r;
										paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
										paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
										paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
										break;
									case BrushProperties.Simple:
										paintEngine.pixels[pixel] =paintEngine.customBrushBytes[brushPixel];
										paintEngine.pixels[pixel+1] = paintEngine.customBrushBytes[brushPixel+1];
										paintEngine.pixels[pixel+2] = paintEngine.customBrushBytes[brushPixel+2];
										paintEngine.pixels[pixel+3] = paintEngine.customBrushBytes[brushPixel+3];
										break;
									case BrushProperties.Pattern:
										/*float*/ yy = Mathf.Repeat(/*py+y*/py+(y-paintEngine.customBrushHeight/2f),paintEngine.customPatternHeight);
										/*float*/ xx = Mathf.Repeat(/*px+x*/px+(x-paintEngine.customBrushWidth/2f),paintEngine.customPatternWidth);
										/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx+yy)*4, paintEngine.patternBrushBytes.Length);
										
										//OVDE TODO
										//add ignore transparent pixels
										paintEngine.pixels[pixel] =paintEngine.patternBrushBytes[pixel2];
										paintEngine.pixels[pixel+1] = paintEngine.patternBrushBytes[pixel2+1];
										paintEngine.pixels[pixel+2] = paintEngine.patternBrushBytes[pixel2+2];
										paintEngine.pixels[pixel+3] = paintEngine.patternBrushBytes[pixel2+3];
										break;
									}
								}
							}
							
						}else{ // use paint color alpha
							
							if (paintEngine.useAdditiveColors)
							{
								if((paintEngine.useLockArea && paintEngine.lockMaskPixels[pixel]==1) || !paintEngine.useLockArea){
									switch (paintEngine.brushMode)
									{
									case BrushProperties.Clear:
										paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel],paintEngine.clearColor.r,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],paintEngine.clearColor.g,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],paintEngine.clearColor.b,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],paintEngine.clearColor.a,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										break;
									case BrushProperties.Default:
										paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel],paintEngine.paintColor.r,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],paintEngine.paintColor.g,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],paintEngine.paintColor.b,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],paintEngine.paintColor.a,lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										break;
									case BrushProperties.Simple:
										paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel],paintEngine.customBrushBytes[brushPixel],lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],paintEngine.customBrushBytes[brushPixel+1],lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],paintEngine.customBrushBytes[brushPixel+2],lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],paintEngine.customBrushBytes[brushPixel+3],lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										break;
									case BrushProperties.Pattern:
										/*float*/ yy = Mathf.Repeat(/*py+y*/py+(y-paintEngine.customBrushHeight/2f),paintEngine.customPatternHeight);
										/*float*/ xx = Mathf.Repeat(/*px+x*/px+(x-paintEngine.customBrushWidth/2f),paintEngine.customPatternWidth);
										/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx+yy)*4, paintEngine.patternBrushBytes.Length);
										paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel],paintEngine.patternBrushBytes[pixel2],lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],paintEngine.patternBrushBytes[pixel2+1],lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],paintEngine.patternBrushBytes[pixel2+2],lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],paintEngine.patternBrushBytes[pixel2+3],lerpVal/*paintEngine.paintColor.a/255f*paintEngine.brushAlphaStrength*/);
										break;
										
										
									}
								}
								
							}else{ // no additive colors
								if((paintEngine.useLockArea && paintEngine.lockMaskPixels[pixel]==1) || !paintEngine.useLockArea){ 
									
									switch (paintEngine.brushMode){
									case BrushProperties.Clear :
										paintEngine.pixels[pixel] =paintEngine.clearColor.r;
										paintEngine.pixels[pixel+1] = paintEngine.clearColor.g;
										paintEngine.pixels[pixel+2] = paintEngine.clearColor.b;
										paintEngine.pixels[pixel+3] = paintEngine.clearColor.a;
										break;
									case BrushProperties.Default:
										paintEngine.pixels[pixel] =paintEngine.paintColor.r;
										paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
										paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
										paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
										break;
									case BrushProperties.Simple:
										paintEngine.pixels[pixel] =paintEngine.customBrushBytes[brushPixel];
										paintEngine.pixels[pixel+1] = paintEngine.customBrushBytes[brushPixel+1];
										paintEngine.pixels[pixel+2] = paintEngine.customBrushBytes[brushPixel+2];
										paintEngine.pixels[pixel+3] = paintEngine.customBrushBytes[brushPixel+3];
										break;
									case BrushProperties.Pattern:
										/*float*/ yy = Mathf.Repeat(/*py+y*/py+(y-paintEngine.customBrushHeight/2f),paintEngine.customPatternHeight);
										/*float*/ xx = Mathf.Repeat(/*px+x*/px+(x-paintEngine.customBrushWidth/2f),paintEngine.customPatternWidth);
										/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx+yy)*4, paintEngine.patternBrushBytes.Length);
										paintEngine.pixels[pixel] =paintEngine.patternBrushBytes[pixel2];
										paintEngine.pixels[pixel+1] = paintEngine.patternBrushBytes[pixel2+1];
										paintEngine.pixels[pixel+2] = paintEngine.patternBrushBytes[pixel2+2];
										paintEngine.pixels[pixel+3] = paintEngine.patternBrushBytes[pixel2+3];
										break;
									}
								}
							}
						}
						
					} // if alpha>0
					pixel+= 4;
					
				} // for x
				//				pixel = (texWidth*(startY==0?1:startY+y)+startX+1)*4;
				pixel = (paintEngine.texWidth*(startY==0?-1:startY+y)+startX+1)*4;
				//pixel = (texWidth*(startY==0?1:startY+y)+(startX==0?1:startX)+1)*4;
			} // for y
			
		}//end of function : DrawCustomBrush2
		
		
		/// <summary>
		/// Draws the pattern circle.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public static void DrawPatternCircle(int x,int y,byte[] patternSource, int size,AdvancedMobilePaint paintEngine)
		{
			//Debug.Log ("DrawPatternCircle "+ x +" ,"+y);
			
			if (!paintEngine.canDrawOnBlack)
			{
				if (paintEngine.pixels[(paintEngine.texWidth*y+x)*4]==0 && paintEngine.pixels[(paintEngine.texWidth*y+x)*4+1]==0 && paintEngine.pixels[(paintEngine.texWidth*y+x)*4+2]==0 && paintEngine.pixels[(paintEngine.texWidth*y+x)*4+3]!=0) return;
			}
			
			int pixel = 0;
			
			// draw fast circle: 
			int r2 = size * size;//povrsina kruga
			int area = r2 << 2;//sve rgb vrednosti koje cine povrsinu kruga -piksela u krugu
			int rr = size << 1;//precnik kruga
			int tx=0;
			int ty=0;
			float yy=0;
			float xx=0;
			int pixel2=0;
			float lerpVal=1f;
			for (int i = 0; i < area; i++)
			{
				/*int*/ tx = (i % rr) - size;
				/*int*/ ty = (i / rr) - size;
				
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
							lerpVal=patternSource[pixel2+3]/255f*paintEngine.brushAlphaStrength;
							paintEngine.pixels[pixel] = (byte)Mathf.Lerp(paintEngine.pixels[pixel], patternSource[pixel2],lerpVal/*patternSource[pixel2+3]/255f*paintEngine.brushAlphaStrength*/);
							paintEngine.pixels[pixel+1] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+1],patternSource[pixel2+1],lerpVal/*patternSource[pixel2+3]/255f*paintEngine.brushAlphaStrength*/);
							paintEngine.pixels[pixel+2] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+2],patternSource[pixel2+2],lerpVal/*patternSource[pixel2+3]/255f*paintEngine.brushAlphaStrength*/);
							paintEngine.pixels[pixel+3] = (byte)Mathf.Lerp(paintEngine.pixels[pixel+3],patternSource[pixel2+3],lerpVal/*patternSource[pixel2+3]/255f*paintEngine.brushAlphaStrength*/);
						}
						
					}else{ // no additive, just paint my colors
						
						if (!paintEngine.useLockArea || (paintEngine.useLockArea && paintEngine.lockMaskPixels[pixel]==1))
						{
							// TODO: pattern dynamic scalar value?
							
							/*float*/ yy = Mathf.Repeat(y+ty,paintEngine.customPatternWidth);
							/*float*/ xx = Mathf.Repeat(x+tx,paintEngine.customPatternWidth);//Debug.Log ("P"+xx+","+yy);
							/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx+yy)*4, paintEngine.patternBrushBytes.Length);
							paintEngine.pixels[pixel] = patternSource[pixel2];//r
							paintEngine.pixels[pixel+1] = patternSource[pixel2+1];//g
							paintEngine.pixels[pixel+2] = patternSource[pixel2+2];//b
							
							paintEngine.pixels[pixel+3] = patternSource[pixel2+3];//a
							//}
						}
						
					} // if additive
				} // if in circle
			} // for area
			
		} // DrawPatternCircle()
		
		//DRAW PROPER LINE ON TEXTURE
		public static void DrawLineBrush (Vector2 start,Vector2 end, int size, bool isPattern,byte[] patternSource,AdvancedMobilePaint paintEngine)
		{
			//
			int x0=(int)start.x;
			int y0=(int)start.y;
			int x1=(int)end.x;
			int y1=(int)end.y;
			//			int dx= Mathf.Abs(x1-x0);
			//			int dy= Mathf.Abs(y1-y0);
			int dy = (int)(y1-y0);
			int dx = (int)(x1-x0);
			int pixel=0;//<--pixel cordinates on texture
			int y=0;
			int x=0;
			int pixel2=0;//<---pixel coordinate on pattern texture
			//draw "size"  pixels wide vector line between points, uses current pattern bytes if "isPattern" is true
			{
				int stepx, stepy;
				
				if (dy < 0) {dy = -dy; stepy = -1;}
				else {stepy = 1;}
				if (dx < 0) {dx = -dx; stepx = -1;}
				else {stepx = 1;}
				dy <<= 1;
				dx <<= 1;
				
				float fraction = 0;
				
				//
				
				
				//
				
				if (dx > dy) {
					fraction = dy - (dx >> 1);
					while (Mathf.Abs(x0 - x1) > 1) {
						if (fraction >= 0) {
							y0 += stepy;
							fraction -= dx;
						}
						x0 += stepx;
						fraction += dy;
						//tex.SetPixel(x0, y0, paintColor);
						if(x0<paintEngine.texWidth && x0>=0 && y0<paintEngine.texHeight && y0>=0  )
						{
							
							pixel= paintEngine.texWidth*4*y0+x0*4;
							if(!isPattern)
							{
								paintEngine.pixels[pixel]=paintEngine.paintColor.r;
								paintEngine.pixels[pixel+1]=paintEngine.paintColor.g;
								paintEngine.pixels[pixel+2]=paintEngine.paintColor.b;
								paintEngine.pixels[pixel+3]=paintEngine.paintColor.a;
							}
							else
							{
								float yy = Mathf.Repeat(/*py+y*/y0/*+(y0-customBrushHeight/2f)*/,paintEngine.customPatternHeight);
								float xx = Mathf.Repeat(/*px+x*/x0/*+(x0-customBrushWidth/2f)*/,paintEngine.customPatternWidth);
								/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx +yy)*4, /*patternBrushBytes*/patternSource.Length);
								
								paintEngine.pixels[pixel] =/*patternBrushBytes*/patternSource[pixel2];
								paintEngine.pixels[pixel+1] = /*patternBrushBytes*/patternSource[pixel2+1];
								paintEngine.pixels[pixel+2] = /*patternBrushBytes*/patternSource[pixel2+2];
								paintEngine.pixels[pixel+3] = /*patternBrushBytes*/patternSource[pixel2+3];
							}
						}
						for(int i=0;i<size;i++)
						{
							//tex.SetPixel(x0,y0-(size/2)+i,paintColor);
							y=y0-(size/2)+i;
							pixel= paintEngine.texWidth*4*y+x0*4;
							if(x0<paintEngine.texWidth && x0>=0 && y<paintEngine.texHeight && y>=0 )
							{
								//try{
								if(!isPattern)
								{
									paintEngine.pixels[pixel]=paintEngine.paintColor.r;
									paintEngine.pixels[pixel+1]=paintEngine.paintColor.g;
									paintEngine.pixels[pixel+2]=paintEngine.paintColor.b;
									paintEngine.pixels[pixel+3]=paintEngine.paintColor.a;
								}
								else
								{
									float yy = Mathf.Repeat(/*py+y*/y/*+(y0-customBrushHeight/2f)*/,paintEngine.customPatternHeight);
									float xx = Mathf.Repeat(/*px+x*/x0/*+(x0-customBrushWidth/2f)*/,paintEngine.customPatternWidth);
									/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx +yy)*4, /*patternBrushBytes*/patternSource.Length);
									
									paintEngine.pixels[pixel] =/*patternBrushBytes*/patternSource[pixel2];
									paintEngine.pixels[pixel+1] = /*patternBrushBytes*/patternSource[pixel2+1];
									paintEngine.pixels[pixel+2] = /*patternBrushBytes*/patternSource[pixel2+2];
									paintEngine.pixels[pixel+3] = /*patternBrushBytes*/patternSource[pixel2+3];
								}
								//}
								//								catch(Exception e)
								//								{
								//									Debug.Log ("ERROR ON PIXEL " + pixel.ToString() + " WITH ( "+y.ToString()+" , "+x0.ToString()+" )");
								//								}
							}
						}
					}
				}
				else {
					fraction = dx - (dy >> 1);
					while (Mathf.Abs(y0 - y1) > 1) {
						if (fraction >= 0) {
							x0 += stepx;
							fraction -= dy;
						}
						y0 += stepy;
						fraction += dx;
						//tex.SetPixel(x0, y0, paintColor);
						if(x0<paintEngine.texWidth && x0>=0 && y0<paintEngine.texHeight && y0>=0 )
						{
							pixel= paintEngine.texWidth*4*y0+x0*4;
							if(!isPattern)
							{
								paintEngine.pixels[pixel]=paintEngine.paintColor.r;
								paintEngine.pixels[pixel+1]=paintEngine.paintColor.g;
								paintEngine.pixels[pixel+2]=paintEngine.paintColor.b;
								paintEngine.pixels[pixel+3]=paintEngine.paintColor.a;
							}
							else
							{
								float yy = Mathf.Repeat(/*py+y*/y0/*+(y0-customBrushHeight/2f)*/,paintEngine.customPatternHeight);
								float xx = Mathf.Repeat(/*px+x*/x0/*+(x0-customBrushWidth/2f)*/,paintEngine.customPatternWidth);
								/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx +yy)*4, /*patternBrushBytes*/patternSource.Length);
								
								paintEngine.pixels[pixel] =/*patternBrushBytes*/patternSource[pixel2];
								paintEngine.pixels[pixel+1] = /*patternBrushBytes*/patternSource[pixel2+1];
								paintEngine.pixels[pixel+2] = /*patternBrushBytes*/patternSource[pixel2+2];
								paintEngine.pixels[pixel+3] = /*patternBrushBytes*/patternSource[pixel2+3];
							}
						}
						for(int i=0;i<size;i++)
						{
							//tex.SetPixel(x0,y0-(size/2)+i,paintColor);
							x=x0-(size/2)+i;
							pixel= paintEngine.texWidth*4*y0+x*4;
							if(x<paintEngine.texWidth && x>=0 && y0<paintEngine.texHeight && y0>=0)
							{
								if(!isPattern)
								{
									paintEngine.pixels[pixel]=paintEngine.paintColor.r;
									paintEngine.pixels[pixel+1]=paintEngine.paintColor.g;
									paintEngine.pixels[pixel+2]=paintEngine.paintColor.b;
									paintEngine.pixels[pixel+3]=paintEngine.paintColor.a;
								}
								else
								{
									float yy = Mathf.Repeat(/*py+y*/y0/*+(y0-customBrushHeight/2f)*/,paintEngine.customPatternHeight);
									float xx = Mathf.Repeat(/*px+x*/x/*+(x0-customBrushWidth/2f)*/,paintEngine.customPatternWidth);
									/*int*/ pixel2 = (int) Mathf.Repeat( (paintEngine.customPatternWidth*xx +yy)*4, /*patternBrushBytes*/patternSource.Length);
									
									paintEngine.pixels[pixel] =/*patternBrushBytes*/patternSource[pixel2];
									paintEngine.pixels[pixel+1] = /*patternBrushBytes*/patternSource[pixel2+1];
									paintEngine.pixels[pixel+2] = /*patternBrushBytes*/patternSource[pixel2+2];
									paintEngine.pixels[pixel+3] = /*patternBrushBytes*/patternSource[pixel2+3];
								}
							}
						}
					}
				}
				
			}
			//tex.LoadRawTextureData(pixels); 
			//tex.Apply();
		}
	
	}
}