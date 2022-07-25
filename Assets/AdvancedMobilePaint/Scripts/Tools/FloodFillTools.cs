using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace AdvancedMobilePaint.Tools
{
	public static class FloodFillTools  {
		
		#region FLOODFILL
		/// <summary>
		/// Floods the fill mask only.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public static void FloodFillMaskOnly(int x,int y,AdvancedMobilePaint paintEngine)
		{
			Debug.Log ("FloodFillMaskOnly");
			// get canvas hit color
			byte hitColorR = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +0 ];
			byte hitColorG = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +1 ];
			byte hitColorB = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +2 ];
			byte hitColorA = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +3 ];
			
			// early exit if its same color already
			//if (paintColor.r == hitColorR && paintColor.g == hitColorG && paintColor.b == hitColorB && paintColor.a == hitColorA) return;
			
			if (!paintEngine.canDrawOnBlack)
			{
				if (hitColorA==0) return;
			}
			
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			paintEngine.lockMaskPixels = new byte[paintEngine.texWidth * paintEngine.texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (paintEngine.texWidth*(ptsy-1)+ptsx)*4; // down
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.maskPixels[pixel+0]==hitColorR 
					    && paintEngine.maskPixels[pixel+1]==hitColorG 
					    && paintEngine.maskPixels[pixel+2]==hitColorB 
					    && paintEngine.maskPixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel]=1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsx+1<paintEngine.texWidth)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx+1)*4; // right
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.maskPixels[pixel+0]==hitColorR 
					    && paintEngine.maskPixels[pixel+1]==hitColorG 
					    && paintEngine.maskPixels[pixel+2]==hitColorB 
					    && paintEngine.maskPixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel]=1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx-1)*4; // left
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.maskPixels[pixel+0]==hitColorR 
					    && paintEngine.maskPixels[pixel+1]==hitColorG 
					    && paintEngine.maskPixels[pixel+2]==hitColorB 
					    && paintEngine.maskPixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel]=1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsy+1<paintEngine.texHeight)
				{
					pixel = (paintEngine.texWidth*(ptsy+1)+ptsx)*4; // up
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.maskPixels[pixel+0]==hitColorR 
					    && paintEngine.maskPixels[pixel+1]==hitColorG 
					    && paintEngine.maskPixels[pixel+2]==hitColorB 
					    && paintEngine.maskPixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel]=1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
			}
		} // floodfill
		
		
//		 basic floodfill
//		/ <summary>
//		/ FloodFill draw.
//		/ </summary>
//		/ <param name="x">The x coordinate.</param>
//		/ <param name="y">The y coordinate.</param>
		public static void FloodFill(int x,int y, AdvancedMobilePaint paintEngine)
		{
			
			
			Debug.Log ("FloodFill");
			// get canvas hit color
			byte hitColorR = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +0 ];
			byte hitColorG = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +1 ];
			byte hitColorB = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +2 ];
			byte hitColorA = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +3 ];
			
			if (!paintEngine.canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			// early exit if its same color already
			if(paintEngine.brushMode!=BrushProperties.Pattern)
				if (paintEngine.paintColor.r == hitColorR && paintEngine.paintColor.g == hitColorG && paintEngine.paintColor.b == hitColorB && paintEngine.paintColor.a == hitColorA) return;
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (paintEngine.texWidth*(ptsy-1)+ptsx)*4; // down
					if (paintEngine.pixels[pixel+0]==hitColorR 
					    && paintEngine.pixels[pixel+1]==hitColorG 
					    && paintEngine.pixels[pixel+2]==hitColorB 
					    && paintEngine.pixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
//						paintEngine.DrawPoint(pixel);
						//DrawPoint(ptsx,ptsy-1);
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
					
					
				}
				
				if (ptsx+1<paintEngine.texWidth)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx+1)*4; // right
					if (paintEngine.pixels[pixel+0]==hitColorR 
					    && paintEngine.pixels[pixel+1]==hitColorG 
					    && paintEngine.pixels[pixel+2]==hitColorB 
					    && paintEngine.pixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
//						paintEngine.DrawPoint(pixel);
						//DrawPoint(ptsx,ptsy);
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx-1)*4; // left
					if (paintEngine.pixels[pixel+0]==hitColorR 
					    && paintEngine.pixels[pixel+1]==hitColorG 
					    && paintEngine.pixels[pixel+2]==hitColorB 
					    && paintEngine.pixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
//						paintEngine.DrawPoint(pixel);
						//DrawPoint(ptsx-1,ptsy);
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsy+1<paintEngine.texHeight)
				{
					pixel = (paintEngine.texWidth*(ptsy+1)+ptsx)*4; // up
					if (paintEngine.pixels[pixel+0]==hitColorR 
					    && paintEngine.pixels[pixel+1]==hitColorG 
					    && paintEngine.pixels[pixel+2]==hitColorB 
					    && paintEngine.pixels[pixel+3]==hitColorA)
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
//						paintEngine.DrawPoint(pixel);
						//DrawPoint(ptsx,ptsy+1);
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
			}
		} // floodfill
		

		/// <summary>
		/// Floodfill by using mask with threshold.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public static void FloodFillMaskOnlyWithThreshold(int x,int y, AdvancedMobilePaint paintEngine)
		{
			Debug.Log ("FloodFillMaskOnlyWithThreshold");
			//Debug.Log("hits");
			// get canvas hit color
			byte hitColorR = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +0 ];
			byte hitColorG = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +1 ];
			byte hitColorB = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +2 ];
			byte hitColorA = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +3 ];
			
			if (!paintEngine.canDrawOnBlack)
			{
				if (hitColorA!=0) return;
			}
			
			// early exit if outside threshold?
			//if (CompareThreshold(paintColor.r,hitColorR) && CompareThreshold(paintColor.g,hitColorG) && CompareThreshold(paintColor.b,hitColorB) && CompareThreshold(paintColor.a,hitColorA)) return;
			if (paintEngine.paintColor.r == hitColorR && paintEngine.paintColor.g == hitColorG && paintEngine.paintColor.b == hitColorB && paintEngine.paintColor.a == hitColorA) return;
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			paintEngine.lockMaskPixels = new byte[paintEngine.texWidth * paintEngine.texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (paintEngine.texWidth*(ptsy-1)+ptsx)*4; // down
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+0],hitColorR) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+1],hitColorG) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+2],hitColorB) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel] = 1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsx+1<paintEngine.texWidth)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx+1)*4; // right
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+0],hitColorR) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+1],hitColorG) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+2],hitColorB) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel] = 1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx-1)*4; // left
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+0],hitColorR) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+1],hitColorG) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+2],hitColorB) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel] = 1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsy+1<paintEngine.texHeight)
				{
					pixel = (paintEngine.texWidth*(ptsy+1)+ptsx)*4; // up
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+0],hitColorR) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+1],hitColorG) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+2],hitColorB) 
					    && paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel] = 1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
			}
		} // floodfillWithTreshold
		
		/// <summary>
		/// Floodfill with treshold.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public static void FloodFillWithTreshold(int x,int y, AdvancedMobilePaint paintEngine)
		{
			Debug.Log ("FloodFillWithThreshold");
			// get canvas hit color
			byte hitColorR = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +0 ];
			byte hitColorG = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +1 ];
			byte hitColorB = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +2 ];
			byte hitColorA = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +3 ];
			
			if (!paintEngine.canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			// early exit if outside threshold
			//if (CompareThreshold(paintColor.r,hitColorR) && CompareThreshold(paintColor.g,hitColorG) && CompareThreshold(paintColor.b,hitColorB) && CompareThreshold(paintColor.a,hitColorA)) return;
			if (paintEngine.paintColor.r == hitColorR && paintEngine.paintColor.g == hitColorG && paintEngine.paintColor.b == hitColorB && paintEngine.paintColor.a == hitColorA) return;
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			paintEngine.lockMaskPixels = new byte[paintEngine.texWidth * paintEngine.texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (paintEngine.texWidth*(ptsy-1)+ptsx)*4; // down
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],hitColorR) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],hitColorG) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],hitColorB) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel] = 1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsx+1<paintEngine.texWidth)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx+1)*4; // right
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],hitColorR) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],hitColorG) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],hitColorB) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel] = 1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx-1)*4; // left
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],hitColorR) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],hitColorG) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],hitColorB) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						//paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel] = 1;
												paintEngine.pixels[pixel] = paintEngine.paintColor.r;
												paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
												paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
												paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsy+1<paintEngine.texHeight)
				{
					pixel = (paintEngine.texWidth*(ptsy+1)+ptsx)*4; // up
					if (paintEngine.lockMaskPixels[pixel]==0
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],hitColorR) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],hitColorG) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],hitColorB) 
					    && paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],hitColorA))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
//						paintEngine.DrawPoint(pixel);
						paintEngine.lockMaskPixels[pixel] = 1;
						paintEngine.pixels[pixel] = paintEngine.paintColor.r;
						paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
						paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
						paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;

					}
				}
			}
		} // floodfillWithTreshold
		
		/// <summary>
		/// Locks area Floodfill.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public static void LockAreaFill(int x,int y, AdvancedMobilePaint paintEngine)
		{
			Debug.Log ("LockAreaFill");
			byte hitColorR = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +0 ];
			byte hitColorG = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +1 ];
			byte hitColorB = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +2 ];
			byte hitColorA = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +3 ];
			
			if (!paintEngine.canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			paintEngine.lockMaskPixels = new byte[paintEngine.texWidth * paintEngine.texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (paintEngine.texWidth*(ptsy-1)+ptsx)*4; // down
					
					if (paintEngine.lockMaskPixels[pixel]==0
					    && (paintEngine.pixels[pixel+0]==hitColorR || paintEngine.pixels[pixel+0]==paintEngine.paintColor.r) 
					    && (paintEngine.pixels[pixel+1]==hitColorG || paintEngine.pixels[pixel+1]==paintEngine.paintColor.g) 
					    && (paintEngine.pixels[pixel+2]==hitColorB || paintEngine.pixels[pixel+2]==paintEngine.paintColor.b) 
					    && (paintEngine.pixels[pixel+3]==hitColorA || paintEngine.pixels[pixel+3]==paintEngine.paintColor.a))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx+1<paintEngine.texWidth)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx+1)*4; // right
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.pixels[pixel+0]==hitColorR || paintEngine.pixels[pixel+0]==paintEngine.paintColor.r) 
					    && (paintEngine.pixels[pixel+1]==hitColorG || paintEngine.pixels[pixel+1]==paintEngine.paintColor.g) 
					    && (paintEngine.pixels[pixel+2]==hitColorB || paintEngine.pixels[pixel+2]==paintEngine.paintColor.b) 
					    && (paintEngine.pixels[pixel+3]==hitColorA || paintEngine.pixels[pixel+3]==paintEngine.paintColor.a))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx-1)*4; // left
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.pixels[pixel+0]==hitColorR || paintEngine.pixels[pixel+0]==paintEngine.paintColor.r) 
					    && (paintEngine.pixels[pixel+1]==hitColorG || paintEngine.pixels[pixel+1]==paintEngine.paintColor.g) 
					    && (paintEngine.pixels[pixel+2]==hitColorB || paintEngine.pixels[pixel+2]==paintEngine.paintColor.b) 
					    && (paintEngine.pixels[pixel+3]==hitColorA || paintEngine.pixels[pixel+3]==paintEngine.paintColor.a))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsy+1<paintEngine.texHeight)
				{
					pixel = (paintEngine.texWidth*(ptsy+1)+ptsx)*4; // up
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.pixels[pixel+0]==hitColorR || paintEngine.pixels[pixel+0]==paintEngine.paintColor.r) 
					    && (paintEngine.pixels[pixel+1]==hitColorG || paintEngine.pixels[pixel+1]==paintEngine.paintColor.g) 
					    && (paintEngine.pixels[pixel+2]==hitColorB || paintEngine.pixels[pixel+2]==paintEngine.paintColor.b) 
					    && (paintEngine.pixels[pixel+3]==hitColorA || paintEngine.pixels[pixel+3]==paintEngine.paintColor.a))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
			}
		} // LockAreaFill
		
		/// <summary>
		/// Floodfill by using mask lock area.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public  static void LockAreaFillMaskOnly(int x,int y, AdvancedMobilePaint paintEngine)
		{
			Debug.Log ("LockAreaFillMaskOnly");
			byte hitColorR = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +0 ];
			byte hitColorG = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +1 ];
			byte hitColorB = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +2 ];
			byte hitColorA = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +3 ];
			
			if (!paintEngine.canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			paintEngine.lockMaskPixels = new byte[paintEngine.texWidth * paintEngine.texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (paintEngine.texWidth*(ptsy-1)+ptsx)*4; // down
					
					if (paintEngine.lockMaskPixels[pixel]==0
					    && (paintEngine.maskPixels[pixel+0]==hitColorR || paintEngine.maskPixels[pixel+0]==paintEngine.paintColor.r) 
					    && (paintEngine.maskPixels[pixel+1]==hitColorG || paintEngine.maskPixels[pixel+1]==paintEngine.paintColor.g) 
					    && (paintEngine.maskPixels[pixel+2]==hitColorB || paintEngine.maskPixels[pixel+2]==paintEngine.paintColor.b) 
					    && (paintEngine.maskPixels[pixel+3]==hitColorA || paintEngine.maskPixels[pixel+3]==paintEngine.paintColor.a))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx+1<paintEngine.texWidth)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx+1)*4; // right
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.maskPixels[pixel+0]==hitColorR || paintEngine.maskPixels[pixel+0]==paintEngine.paintColor.r) 
					    && (paintEngine.maskPixels[pixel+1]==hitColorG || paintEngine.maskPixels[pixel+1]==paintEngine.paintColor.g) 
					    && (paintEngine.maskPixels[pixel+2]==hitColorB || paintEngine.maskPixels[pixel+2]==paintEngine.paintColor.b) 
					    && (paintEngine.maskPixels[pixel+3]==hitColorA || paintEngine.maskPixels[pixel+3]==paintEngine.paintColor.a))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx-1)*4; // left
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.maskPixels[pixel+0]==hitColorR || paintEngine.maskPixels[pixel+0]==paintEngine.paintColor.r) 
					    && (paintEngine.maskPixels[pixel+1]==hitColorG || paintEngine.maskPixels[pixel+1]==paintEngine.paintColor.g) 
					    && (paintEngine.maskPixels[pixel+2]==hitColorB || paintEngine.maskPixels[pixel+2]==paintEngine.paintColor.b) 
					    && (paintEngine.maskPixels[pixel+3]==hitColorA || paintEngine.maskPixels[pixel+3]==paintEngine.paintColor.a))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsy+1<paintEngine.texHeight)
				{
					pixel = (paintEngine.texWidth*(ptsy+1)+ptsx)*4; // up
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.maskPixels[pixel+0]==hitColorR || paintEngine.maskPixels[pixel+0]==paintEngine.paintColor.r) 
					    && (paintEngine.maskPixels[pixel+1]==hitColorG || paintEngine.maskPixels[pixel+1]==paintEngine.paintColor.g) 
					    && (paintEngine.maskPixels[pixel+2]==hitColorB || paintEngine.maskPixels[pixel+2]==paintEngine.paintColor.b) 
					    && (paintEngine.maskPixels[pixel+3]==hitColorA || paintEngine.maskPixels[pixel+3]==paintEngine.paintColor.a))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
			}
		} // LockAreaFillMaskOnly
		
		

		// create locking mask floodfill, using threshold, checking pixels from mask only
		public static void LockAreaFillWithThresholdMaskOnly(int x,int y, AdvancedMobilePaint paintEngine)
		{
			Debug.Log("LockAreaFillWithThresholdMaskOnly");
			
			// get canvas color from this point
			byte hitColorR = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +0 ];
			byte hitColorG = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +1 ];
			byte hitColorB = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +2 ];
			byte hitColorA = paintEngine.maskPixels[ ((paintEngine.texWidth*(y)+x)*4) +3 ];
			
			if (!paintEngine.canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) {/*Debug.Log ("CANT DRAW ON BLACK");*/return;}
			}
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			paintEngine.lockMaskPixels = new byte[paintEngine.texWidth * paintEngine.texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (paintEngine.texWidth*(ptsy-1)+ptsx)*4; // down
					
					if (paintEngine.lockMaskPixels[pixel]==0 // this pixel is not used yet
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+0],hitColorR)) // if pixel is same as hit color OR same as paint color
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+1],hitColorG)) 
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+2],hitColorB)) 
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+3],hitColorA)))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx+1<paintEngine.texWidth)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx+1)*4; // right
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+0],hitColorR)) // if pixel is same as hit color OR same as paint color
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+1],hitColorG)) 
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+2],hitColorB)) 
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+3],hitColorA)))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx-1)*4; // left
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+0],hitColorR)) // if pixel is same as hit color OR same as paint color
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+1],hitColorG)) 
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+2],hitColorB)) 
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+3],hitColorA)))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						paintEngine.lockMaskPixels[pixel] = 1; 
					}
				}
				
				if (ptsy+1<paintEngine.texHeight)
				{
					pixel = (paintEngine.texWidth*(ptsy+1)+ptsx)*4; // up
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+0],hitColorR)) // if pixel is same as hit color OR same as paint color
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+1],hitColorG)) 
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+2],hitColorB))
					    && (paintEngine.CompareThreshold(paintEngine.maskPixels[pixel+3],hitColorA)))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						paintEngine.lockMaskPixels[pixel] = 1; 
					}
				}
			}
		} // LockMaskFillWithTreshold
		
		
		
		// create locking mask floodfill, using threshold
		public static void LockMaskFillWithThreshold(int x,int y,AdvancedMobilePaint paintEngine)
		{
			Debug.Log("LockMaskFillWithTreshold");
			// get canvas color from this point
			byte hitColorR = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +0 ];
			byte hitColorG = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +1 ];
			byte hitColorB = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +2 ];
			byte hitColorA = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +3 ];
			
			if (!paintEngine.canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			paintEngine.lockMaskPixels = new byte[paintEngine.texWidth * paintEngine.texHeight * 4];
			
			while (fillPointX.Count > 0)
			{
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (paintEngine.texWidth*(ptsy-1)+ptsx)*4; // down
					
					if (paintEngine.lockMaskPixels[pixel]==0 // this pixel is not used yet
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],hitColorR) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],paintEngine.paintColor.r)) // if pixel is same as hit color OR same as paint color
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],hitColorG) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],paintEngine.paintColor.g)) 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],hitColorB) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],paintEngine.paintColor.b)) 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],hitColorA) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],paintEngine.paintColor.a)))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx+1<paintEngine.texWidth)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx+1)*4; // right
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],hitColorR) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],paintEngine.paintColor.r)) // if pixel is same as hit color OR same as paint color
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],hitColorG) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],paintEngine.paintColor.g)) 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],hitColorB) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],paintEngine.paintColor.b)) 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],hitColorA) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],paintEngine.paintColor.a)))
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						paintEngine.lockMaskPixels[pixel] = 1;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx-1)*4; // left
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],hitColorR) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],paintEngine.paintColor.r)) // if pixel is same as hit color OR same as paint color
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],hitColorG) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],paintEngine.paintColor.g)) 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],hitColorB) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],paintEngine.paintColor.b)) 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],hitColorA) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],paintEngine.paintColor.a)))
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						paintEngine.lockMaskPixels[pixel] = 1; 
					}
				}
				
				if (ptsy+1<paintEngine.texHeight)
				{
					pixel = (paintEngine.texWidth*(ptsy+1)+ptsx)*4; // up
					if (paintEngine.lockMaskPixels[pixel]==0 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],hitColorR) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+0],paintEngine.paintColor.r)) // if pixel is same as hit color OR same as paint color
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],hitColorG) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+1],paintEngine.paintColor.g)) 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],hitColorB) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+2],paintEngine.paintColor.b)) 
					    && (paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],hitColorA) || paintEngine.CompareThreshold(paintEngine.pixels[pixel+3],paintEngine.paintColor.a)))
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						paintEngine.lockMaskPixels[pixel] = 1; 
					}
				}
			}
		} // LockMaskFillWithTreshold
		
		
		/// <summary>
		/// Flood fill with automask based on threshold.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="paintEngine">Paint engine.</param>
		public static void FloodFillAutoMaskWithThreshold(int x,int y, AdvancedMobilePaint paintEngine)
		{
			
			
			Debug.Log ("FloodFill");
			// get canvas hit color
			byte hitColorR = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +0 ];
			byte hitColorG = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +1 ];
			byte hitColorB = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +2 ];
			byte hitColorA = paintEngine.pixels[ ((paintEngine.texWidth*(y)+x)*4) +3 ];
			int lowestVal=paintEngine.paintThreshold;//70;
			if (!paintEngine.canDrawOnBlack)
			{
				if (hitColorR==0 && hitColorG==0 && hitColorB==0 && hitColorA!=0) return;
			}
			
			// early exit if its same color already
			if(paintEngine.brushMode!=BrushProperties.Pattern)
				if (paintEngine.paintColor.r == hitColorR && paintEngine.paintColor.g == hitColorG && paintEngine.paintColor.b == hitColorB && paintEngine.paintColor.a == hitColorA) return;
			
			
			
			Queue<int> fillPointX = new Queue<int>();
			Queue<int> fillPointY = new Queue<int>();
			fillPointX.Enqueue(x);
			fillPointY.Enqueue(y);
			
			int ptsx,ptsy;
			int pixel = 0;
			
			while (fillPointX.Count > 0)
			{
				//
				
				ptsx = fillPointX.Dequeue();
				ptsy = fillPointY.Dequeue();
				
				if (ptsy-1>-1)
				{
					pixel = (paintEngine.texWidth*(ptsy-1)+ptsx)*4; // down
					if (
						!( paintEngine.pixels[pixel+0]<lowestVal  && paintEngine.pixels[pixel+1]<lowestVal && paintEngine.pixels[pixel+2]<lowestVal  )
						//&& paintEngine.pixels[pixel+3]>0
						&& !( paintEngine.pixels[pixel+0] ==paintEngine.paintColor.r    
					     && paintEngine.pixels[pixel+1]==paintEngine.paintColor.g
					     && paintEngine.pixels[pixel+2]==paintEngine.paintColor.b)
						//&& paintEngine.pixels[pixel+3]!=paintEngine.paintColor.a
						)
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy-1);
						//paintEngine.DrawPoint(pixel);
						//DrawPoint(ptsx,ptsy-1);
						paintEngine.pixels[pixel] = paintEngine.paintColor.r;
						paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
						paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
						paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
					
					
				}
				
				if (ptsx+1<paintEngine.texWidth)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx+1)*4; // right
					if (
						!( paintEngine.pixels[pixel+0]<lowestVal  && paintEngine.pixels[pixel+1]<lowestVal && paintEngine.pixels[pixel+2]<lowestVal  )
						//&& paintEngine.pixels[pixel+3]>0
						&& !( paintEngine.pixels[pixel+0] ==paintEngine.paintColor.r    
					     && paintEngine.pixels[pixel+1]==paintEngine.paintColor.g
					     && paintEngine.pixels[pixel+2]==paintEngine.paintColor.b)
						//&& paintEngine.pixels[pixel+3]!=paintEngine.paintColor.a
						)
					{
						fillPointX.Enqueue(ptsx+1);
						fillPointY.Enqueue(ptsy);
						//paintEngine.DrawPoint(pixel);
						//DrawPoint(ptsx,ptsy);
						paintEngine.pixels[pixel] = paintEngine.paintColor.r;
						paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
						paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
						paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsx-1>-1)
				{
					pixel = (paintEngine.texWidth*ptsy+ptsx-1)*4; // left
					if (
						!( paintEngine.pixels[pixel+0]<lowestVal  && paintEngine.pixels[pixel+1]<lowestVal && paintEngine.pixels[pixel+2]<lowestVal  )
						//&& paintEngine.pixels[pixel+3]>0
						&& !( paintEngine.pixels[pixel+0] ==paintEngine.paintColor.r    
					     && paintEngine.pixels[pixel+1]==paintEngine.paintColor.g
					     && paintEngine.pixels[pixel+2]==paintEngine.paintColor.b)
						//&& paintEngine.pixels[pixel+3]!=paintEngine.paintColor.a
						)
					{
						fillPointX.Enqueue(ptsx-1);
						fillPointY.Enqueue(ptsy);
						//paintEngine.DrawPoint(pixel);
						//DrawPoint(ptsx-1,ptsy);
						paintEngine.pixels[pixel] = paintEngine.paintColor.r;
						paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
						paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
						paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
				
				if (ptsy+1<paintEngine.texHeight)
				{
					pixel = (paintEngine.texWidth*(ptsy+1)+ptsx)*4; // up
					if (
						!( paintEngine.pixels[pixel+0]<lowestVal  && paintEngine.pixels[pixel+1]<lowestVal && paintEngine.pixels[pixel+2]<lowestVal  )
						//&& paintEngine.pixels[pixel+3]>0
						&& !( paintEngine.pixels[pixel+0] ==paintEngine.paintColor.r    
					     && paintEngine.pixels[pixel+1]==paintEngine.paintColor.g
					     && paintEngine.pixels[pixel+2]==paintEngine.paintColor.b)
						//&& paintEngine.pixels[pixel+3]!=paintEngine.paintColor.a
						)
					{
						fillPointX.Enqueue(ptsx);
						fillPointY.Enqueue(ptsy+1);
						//paintEngine.DrawPoint(pixel);
						//DrawPoint(ptsx,ptsy+1);
						paintEngine.pixels[pixel] = paintEngine.paintColor.r;
						paintEngine.pixels[pixel+1] = paintEngine.paintColor.g;
						paintEngine.pixels[pixel+2] = paintEngine.paintColor.b;
						paintEngine.pixels[pixel+3] = paintEngine.paintColor.a;
					}
				}
			}
		} // floodfill
		
		#endregion FLOODFILL
		
	}
}
