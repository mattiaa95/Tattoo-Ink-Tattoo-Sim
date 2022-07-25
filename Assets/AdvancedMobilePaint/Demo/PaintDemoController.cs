using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AdvancedMobilePaint;
public class PaintDemoController : MonoBehaviour {
	public AdvancedMobilePaint.AdvancedMobilePaint paintEngine;
	
	public Texture2D paintSurface;
	public Texture2D maskTexture;
	public Texture2D brushExample1;
	public Texture2D brushExample2;
	public Texture2D brushExample3;
	public Texture2D patternExample1;
	public Texture2D patternExample2;
	public Texture2D patternExample3;
	
	public Sprite patternSprite;
	public Sprite patternSprite2;
	//public AdvancedMobilePaint.AdvancedMobilePaint paintEngine_Canvas;
	
	bool lockMode=false;
	
	public PaintUndoManager undoManager;
	
	
	public Texture2D unreadableTex2D;
	
	public GameObject rotatedTextureDisplay;
	public float rotationAngle=32.0f;
	// Use this for initialization
	void Start () {
	
	//SetUpQuadPaint();
	patternExample2=/*paintEngine*/PaintUtils.ConvertSpriteToTexture2D(patternSprite2);
	DoReset();
	}
	
//	// Update is called once per frame
//	void Update () {
//	

//	}
	
	public void SetUpQuadPaint()
	{
		paintEngine.SetDrawingTexture(paintSurface);
		paintEngine.useLockArea=false;
		paintEngine.useMaskLayerOnly=false;
		paintEngine.useThreshold=false;
		paintEngine.drawEnabled=true;
	}
	
	public void SetUpBitmapBrushType1()
	{
		paintEngine.drawMode=AdvancedMobilePaint.DrawMode.CustomBrush;
		paintEngine.SetBitmapBrush(brushExample1,AdvancedMobilePaint.BrushProperties.Default,false,true,Color.blue,false,false,null);
		paintEngine.drawEnabled=true;
	}
	
	public void SetUpBitmapBrushType2()
	{
		paintEngine.drawMode=AdvancedMobilePaint.DrawMode.CustomBrush;
		paintEngine.SetBitmapBrush(brushExample1,AdvancedMobilePaint.BrushProperties.Pattern,false,true,Color.blue,false,false,/*paintEngine*/PaintUtils.ConvertSpriteToTexture2D(patternSprite));
		paintEngine.drawEnabled=true;
	}
	
	public void ToogleLockMask()
	{
	lockMode=!lockMode;
	
	if(lockMode)
	{
		paintEngine.SetDrawingMask(maskTexture);
		paintEngine.useLockArea=true;
		paintEngine.useMaskLayerOnly=true;
		paintEngine.useThreshold=true;
	}
	else
	{
			paintEngine.useLockArea=false;
			paintEngine.useMaskLayerOnly=false;
			paintEngine.useThreshold=false;
	}
	
	}
	
//	public void SetUpQuadPaint_Canvas()
//	{
////		paintEngine_Canvas.SetDrawingTexture(paintSurface);
////		paintEngine_Canvas.useLockArea=false;
////		paintEngine_Canvas.useMaskLayerOnly=false;
////		paintEngine_Canvas.useThreshold=false;
////		DoReset();
//	}
	
//	public void SetUpBitmapBrushType1_Canvas()
//	{
////		paintEngine_Canvas.drawMode=AdvancedMobilePaint.DrawMode.CustomBrush;
////		paintEngine_Canvas.SetBitmapBrush(brushExample1,AdvancedMobilePaint.BrushProperties.Default,false,true,Color.blue,false,false,null);
////		paintEngine_Canvas.drawEnabled=true;
//	}
	
//	public void SetUpBitmapBrushType2_Canvas()
//	{
////		paintEngine_Canvas.drawMode=AdvancedMobilePaint.DrawMode.CustomBrush;
////
////		paintEngine_Canvas.SetBitmapBrush(brushExample1,AdvancedMobilePaint.BrushProperties.Pattern,false,true,Color.blue,false,false,patternExample1);
////		paintEngine_Canvas.drawEnabled=true;
//	}
	public void SetUpBitmapBrushType3()
	{
		paintEngine.drawMode=AdvancedMobilePaint.DrawMode.CustomBrush;
		
		paintEngine.SetBitmapBrush(brushExample1,AdvancedMobilePaint.BrushProperties.Pattern,false,true,Color.blue,false,false,patternExample2);
		paintEngine.drawEnabled=true;
	}
//	public void ToogleLockMask()
//	{
//		lockMode=!lockMode;
//		
//		if(lockMode)
//		{
//			paintEngine.SetDrawingMask(maskTexture);
//			paintEngine.useLockArea=true;
//			paintEngine.useMaskLayerOnly=true;
//			paintEngine.useThreshold=true;
//		}
//		else
//		{
//			paintEngine.useLockArea=false;
//			paintEngine.useMaskLayerOnly=false;
//			paintEngine.useThreshold=false;
//		}
//		
//	}
	public void SetUppFlooodFillBrush()
	{
		paintEngine.SetFloodFIllBrush(Color.blue,true);
		paintEngine.useLockArea=false;
		//paintEngine.useMaskLayerOnly=true;
		paintEngine.useThreshold=false;
		paintEngine.drawEnabled=true;
		paintEngine.canDrawOnBlack=false;
	}
	
	public void DoUndo()
	{
		undoManager.UndoLastStep();
	}
	public void DoRedo()
	{
		undoManager.RedoLastStep();
	}
	public void DoReset()
	{
		undoManager.ClearSteps();
		SetUpQuadPaint();
	}
	
	public void SetUpVectorBrushType()
	{
		paintEngine.drawMode=AdvancedMobilePaint.DrawMode.Default;
		
		//paintEngine_Canvas.SetBitmapBrush(brushExample1,AdvancedMobilePaint.BrushProperties.Pattern,false,true,Color.blue,false,false,patternExample2);
		paintEngine.SetVectorBrush(VectorBrush.Rectangular,32,128,Color.red,null,false,false,false,false);
		//paintEngine.brushMode=AdvancedMobilePaint.BrushProperties.Default;
		paintEngine.customBrushHeight=32;
		paintEngine.customBrushWidth=128;
		//paintEngine.brushMode=BrushProperties.Default;
		//paintEngine.drawMode=DrawMode.Default;
		paintEngine.drawEnabled=true;
	}
	public void SetUpVectorBrushType2()
	{
		
		
		//paintEngine_Canvas.SetBitmapBrush(brushExample1,AdvancedMobilePaint.BrushProperties.Pattern,false,true,Color.blue,false,false,patternExample2);
		paintEngine.SetVectorBrush(VectorBrush.Rectangular,32,32,Color.gray,/*paintEngine*/PaintUtils.ConvertSpriteToTexture2D(patternSprite)/*patternExample1*/,false,false,false,false);
		//paintEngine.drawMode=AdvancedMobilePaint.DrawMode.Pattern;
		//paintEngine.brushMode=AdvancedMobilePaint.BrushProperties.Pattern;
		paintEngine.customBrushHeight=128;
		paintEngine.customBrushWidth=32;
		paintEngine.drawEnabled=true;
	}
	public void ToogleMultitouch()
	{
		paintEngine.multitouchEnabled=!paintEngine.multitouchEnabled;
		
		paintEngine.connectBrushStokes=true;
	}
	
	public void DrawLine2()
	{
		//line inside
//		paintEngine.ReadCurrentCustomPattern(paintEngine.ConvertSpriteToTexture2D(patternSprite));
//		//line edge
//		paintEngine.customBrush=paintEngine.ConvertSpriteToTexture2D(patternSprite2);
//		paintEngine.ReadCurrentCustomBrush();
//		paintEngine.brushSize=10;
//		paintEngine.lineEdgeSize=0;//10;
//		paintEngine.paintColor=Color.red;
//		paintEngine.useAdditiveColors=false;
//		paintEngine.isLinePaint=true;
//		paintEngine.isPatternLine=true;
//	
		//
		paintEngine.SetLineBrush(10,0,Color.red,null,null);
	}
	public void DrawLine3()
	{
		//line inside
		//paintEngine.ReadCurrentCustomPattern(paintEngine.ConvertSpriteToTexture2D(patternSprite));
		//line edge
		//paintEngine.customBrush=paintEngine.ConvertSpriteToTexture2D(patternSprite2);
//		paintEngine.ReadCurrentCustomBrush();
//		paintEngine.brushSize=10;
//		paintEngine.lineEdgeSize=0;//10;
//		paintEngine.paintColor=Color.blue;
//		paintEngine.useAdditiveColors=false;
//		paintEngine.isLinePaint=true;
//		paintEngine.isPatternLine=false;
		//paintEngine.DrawLineBrush(new Vector2(11f,256f), new Vector2(500f,/*11f*/256f),8, true);
		//
		paintEngine.SetLineBrush(10,0,Color.red,/*paintEngine*/PaintUtils.ConvertSpriteToTexture2D(patternSprite),null);
	}
	
	public void ReadUnreadableTex()
	{
		paintSurface=PaintUtils.ReadUnreadableTexture(unreadableTex2D);
		paintEngine.SetDrawingTexture(paintSurface);
	}
	
	public void RotateTexture()
	{
		Texture2D rotTex=PaintUtils.RotateTexture(paintSurface,rotationAngle);
		rotatedTextureDisplay.GetComponent<RawImage>().texture=rotTex;
		
	}
}
