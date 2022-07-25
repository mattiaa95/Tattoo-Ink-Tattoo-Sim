using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AdvancedMobilePaint.Tools;
namespace AdvancedMobilePaint
{
public class PaintUndoManager : MonoBehaviour {
	
		/// <summary>
		/// Pokazivac na AMP skriptu.
		/// Setuje se automatski ako je ova skripta pridruzena AMP objektu.
		/// </summary>
		public AdvancedMobilePaint paintEngine;
		/// <summary>
		/// The doing work flag.Used internaly.
		/// </summary>
		public bool doingWork=false;
		//stek za undo korake
		Stack<UStep> steps;
		//stek za redo korake
		Stack<UStep> redoSteps;
		/// <summary>
		/// Dubina steka u broju koraka koja moze da se pamti.
		/// </summary>
		public int stackDepth=int.MaxValue;
		/// <summary>
		/// Flag koji oznacava da li je stek popunjen tj da li je moguce dodati undo korake;
		/// </summary>
		public bool stackFull=false;
		
		
		// Use this for initialization		
		void Awake()
		{
			steps = new Stack<UStep> ();
			redoSteps = new Stack<UStep> ();
			if(paintEngine==null)
			{
				if(gameObject.GetComponent<AdvancedMobilePaint>()!=null)
				{
					paintEngine=gameObject.GetComponent<AdvancedMobilePaint>();
					paintEngine.undoEnabled=true;
					paintEngine.undoController=gameObject.GetComponent<PaintUndoManager>();
				}
				else
					Debug.LogError ("AMP: PaintUndoManger Cant find paint engine!");
				
				
			}
			
		}
	
	
	// Use this for initialization (ovde mozete da dodate neku custom inicijalizaciju)
	
//	void Start () {
//	
//	}

	/// <summary>
	/// Dodaje nov korak u stek ako stek nije pun.
	/// </summary>
	/// <param name="step">Step.</param>
	public void AddStep( UStep step)
	{

		if (step != null && steps != null) {
			if(steps.Count<stackDepth)
			{
			steps.Push (step);
			#if UNITY_EDITOR
				Debug.Log ("AMP: NEW STEP ADDED!");
			#endif
			}
			else
			{
				//#if UNITY_EDITOR
				stackFull=true;
				Debug.Log ("AMP: StackFull");
				//#endif
				}
		}
		else
		{
			//#if UNITY_EDITOR
				//normalno nikad ne bi trebalo da se izvrsi ova grana
				Debug.Log ("AMP: ERROR IN ADDING NEW STEP!");
			//#endif
		}	
	}
	/// <summary>
	/// Brise sve stekove (sve korake).
	/// </summary>
	public void ClearSteps()
	{
		steps.Clear ();
		redoSteps.Clear ();
		stackFull=false;
	}
	/// <summary>
	/// Reiscrtavanje svih koraka.
	/// Ne ekstenduj ovu funkciju [INTERNAL]; 
	/// </summary>
	void UndoRedrawSteps()
	{
		Stack <UStep> tmpStack= new Stack<UStep>();
		//save previous settings
		UStep settings= new UStep();
		settings.SetStepPropertiesFromEngine(paintEngine);
		//get all steps from stack bottom to top
		while(steps.Count!=0)
		{
			tmpStack.Push(steps.Pop());
		}
		//redraw all the steps in order of drawing
		
		while(tmpStack.Count!=0)
		{
			UStep tmp=tmpStack.Pop();
			//skip possible non-drawing steps
			if(tmp.type>=0|| tmp.type<5)
			{
				Debug.Log ("UNDO STEP EXEC");
				//setup engine in order to read step
				tmp.SetPropertiesFromStep(paintEngine);
				//bitmap based brushes redraw
				if(tmp.type==0)
				{
					switch(tmp.brushMode)
					{
						case BrushProperties.Default:
						Debug.Log ("DEF MODE");
						paintEngine.ReadCurrentCustomBrush();
							for (int i=0; i<tmp.drawCoordinates.Count; i++)
								BitmapBrushesTools.DrawCustomBrush2 ((int)tmp.drawCoordinates [i].x, (int)tmp.drawCoordinates [i].y,paintEngine);
						break;
						case BrushProperties.Simple:
							//FIX UNDO
							paintEngine.ReadCurrentCustomBrush();
							for (int i=0; i<tmp.drawCoordinates.Count; i++)
								BitmapBrushesTools.DrawCustomBrush2 ((int)tmp.drawCoordinates [i].x, (int)tmp.drawCoordinates [i].y,paintEngine);
						break;
						case BrushProperties.Pattern:
							paintEngine.ReadCurrentCustomBrush();
							paintEngine.ReadCurrentCustomPattern(tmp.patternTexture);
							for (int i=0; i<tmp.drawCoordinates.Count; i++)
								BitmapBrushesTools.DrawCustomBrush2 ((int)tmp.drawCoordinates [i].x, (int)tmp.drawCoordinates [i].y,paintEngine);
						break;
						default:
						break;
				
					}
				}else
				if(tmp.type==2)
				{
						for (int i=0; i<tmp.drawCoordinates.Count; i++)
						if(paintEngine.useSmartFloodFill)
						{
							FloodFillTools.FloodFillAutoMaskWithThreshold((int)tmp.drawCoordinates[i].x, (int)tmp.drawCoordinates[i].y,paintEngine);
						}else
						{
						if (tmp.useTreshold)
						{	
							if (tmp.useMaskLayerOnly)
							{
								FloodFillTools.FloodFillMaskOnlyWithThreshold((int)tmp.drawCoordinates[i].x, (int)tmp.drawCoordinates[i].y,paintEngine);
							}else{
								FloodFillTools.FloodFillWithTreshold((int)tmp.drawCoordinates[i].x, (int)tmp.drawCoordinates[i].y,paintEngine);
							}
						}else{
							if (tmp.useMaskLayerOnly)
							{
								FloodFillTools.FloodFillMaskOnly((int)tmp.drawCoordinates[i].x, (int)tmp.drawCoordinates[i].y,paintEngine);
							}else{
								FloodFillTools.FloodFill((int)tmp.drawCoordinates[i].x, (int)tmp.drawCoordinates[i].y,paintEngine);
							}
						}
						}
				}else
				if(tmp.type==1)
					{
						
						
						if(tmp.brushMode==BrushProperties.Default)
						{
							if(tmp.vectorBrushType==VectorBrush.Circle)
							{
								for (int i=0; i<tmp.drawCoordinates.Count; i++)
									VectorBrushesTools.DrawCircle((int)tmp.drawCoordinates[i].x, (int)tmp.drawCoordinates[i].y,paintEngine);
							}else
							{
								for (int i=0; i<tmp.drawCoordinates.Count; i++)
									VectorBrushesTools.DrawRectangle((int)tmp.drawCoordinates[i].x, (int)tmp.drawCoordinates[i].y,paintEngine);
							}
							
						}else
							if(tmp.brushMode==BrushProperties.Pattern)
						{
							paintEngine.ReadCurrentCustomPattern(tmp.patternTexture);
							if(tmp.vectorBrushType==VectorBrush.Circle)
							{
								for (int i=0; i<tmp.drawCoordinates.Count; i++)
									VectorBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[i].x, (int)tmp.drawCoordinates[i].y,paintEngine);
							}else
							{
								for (int i=0; i<tmp.drawCoordinates.Count; i++)
									VectorBrushesTools.DrawPatternRectangle((int)tmp.drawCoordinates[i].x, (int)tmp.drawCoordinates[i].y,paintEngine);
							}
						}
						
						
					}else if(tmp.type==4)
						{
							if(paintEngine.multitouchEnabled)
								DrawMultiTouchLine(tmp);
							else
								DrawSingleTouchLine(tmp);
							
						}	
					//return it to stack
				steps.Push(tmp);
			}
			else
				steps.Push(tmp);
		}
		//load and apply changes
		paintEngine.tex.LoadRawTextureData (paintEngine.pixels);
		paintEngine.tex.Apply ();
		//restore modes
		settings.SetPropertiesFromStep(paintEngine);
	}
		public void DrawMultiTouchLine(UStep tmp)
		{
			
			if(tmp.lineEgdeSize==0)
			{
				Debug.Log("DRAW MULTI TOUCH LINE UNDO MANAGER!");
				if(tmp.isPatternLine)
				{
					for(int j=0;j<tmp.touchCoordinates.Count;j++)
					{
						VectorBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[0]].x,(int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[0]].y,paintEngine);
						for(int i=1;i<tmp.touchCoordinates[j].coordinatesIndex.Count;i++)
						{
							BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i-1]],tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]],paintEngine.brushSize*2,true,paintEngine.patternBrushBytes,paintEngine);
							VectorBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]].x,(int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]].y,paintEngine);
						}
					}
				}
				else
				{
					for(int j=0;j<tmp.touchCoordinates.Count;j++)
					{
						VectorBrushesTools.DrawCircle((int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[0]].x,(int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[0]].y,paintEngine);
						for(int i=1;i<tmp.touchCoordinates[j].coordinatesIndex.Count;i++)
						{
							BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i-1]],tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]],paintEngine.brushSize*2,false,null,paintEngine);
							VectorBrushesTools.DrawCircle((int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]].x,(int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]].y,paintEngine);
						}
					}
				}
			}else
			{
				if(tmp.isPatternLine)
				{
					for(int j=0;j<tmp.touchCoordinates.Count;j++)
					{
						BitmapBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[0]].x,(int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[0]].y,paintEngine.customBrushBytes, paintEngine.brushSize+paintEngine.lineEdgeSize,paintEngine);
						VectorBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[0]].x,(int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[0]].y,paintEngine);
						for(int i=1;i<tmp.touchCoordinates[j].coordinatesIndex.Count;i++)
						{
							BitmapBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]].x,(int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]].y,paintEngine.customBrushBytes, paintEngine.brushSize+paintEngine.lineEdgeSize,paintEngine);
							BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i-1]],tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]],paintEngine.brushSize*2+paintEngine.lineEdgeSize*2,true,paintEngine.customBrushBytes,paintEngine);
						//
							if(i>1)
							{
								BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i-2]],tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i-1]],paintEngine.brushSize*2+paintEngine.lineEdgeSize*2,true,paintEngine.customBrushBytes,paintEngine);
								BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i-2]],tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i-1]],paintEngine.brushSize*2,true,paintEngine.patternBrushBytes,paintEngine);
								VectorBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i-2]].x, (int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i-2]].y,paintEngine);
							}
						}
					}
				}
				else
				{
					for(int j=0;j<tmp.touchCoordinates.Count;j++)
					{
						VectorBrushesTools.DrawCircle((int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[0]].x,(int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[0]].y,paintEngine);
						for(int i=1;i<tmp.touchCoordinates[j].coordinatesIndex.Count;i++)
						{
							BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i-1]],tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]],paintEngine.brushSize*2,false,null,paintEngine);
							VectorBrushesTools.DrawCircle((int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]].x,(int)tmp.drawCoordinates[tmp.touchCoordinates[j].coordinatesIndex[i]].y,paintEngine);
						}
					}
				}
			}
		}
	public void DrawSingleTouchLine(UStep tmp)
	{
			if(tmp.lineEgdeSize==0)
			{
				if(tmp.isPatternLine)
				{
					VectorBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[0].x,(int)tmp.drawCoordinates[0].y,paintEngine);
					for(int i=1;i<tmp.drawCoordinates.Count;i++)
					{
						BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[i-1],tmp.drawCoordinates[i],paintEngine.brushSize*2,true,paintEngine.patternBrushBytes,paintEngine);
						VectorBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[i].x,(int)tmp.drawCoordinates[i].y,paintEngine);
					}
				}
				else
				{
					VectorBrushesTools.DrawCircle((int)tmp.drawCoordinates[0].x,(int)tmp.drawCoordinates[0].y,paintEngine);
					for(int i=1;i<tmp.drawCoordinates.Count;i++)
					{
						BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[i-1],tmp.drawCoordinates[i],paintEngine.brushSize*2,false,null,paintEngine);
						VectorBrushesTools.DrawCircle((int)tmp.drawCoordinates[i].x,(int)tmp.drawCoordinates[i].y,paintEngine);
					}
				}
			}else
			{
				if(tmp.isPatternLine)
				{
					BitmapBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[0].x,(int)tmp.drawCoordinates[0].y,paintEngine.customBrushBytes, paintEngine.brushSize+paintEngine.lineEdgeSize,paintEngine);
					VectorBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[0].x,(int)tmp.drawCoordinates[0].y,paintEngine);
					for(int i=1;i<tmp.drawCoordinates.Count;i++)
					{
						//									paintEngine.DrawLineBrush(tmp.drawCoordinates[i-1],tmp.drawCoordinates[i],paintEngine.brushSize*2,true,paintEngine.patternBrushBytes);
						//									paintEngine.DrawPatternCircle((int)tmp.drawCoordinates[i].x,(int)tmp.drawCoordinates[i].y);
						BitmapBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[i].x,(int)tmp.drawCoordinates[i].y,paintEngine.customBrushBytes, paintEngine.brushSize+paintEngine.lineEdgeSize,paintEngine);
						BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[i-1],tmp.drawCoordinates[i],paintEngine.brushSize*2+paintEngine.lineEdgeSize*2,true,paintEngine.customBrushBytes,paintEngine);
						//
						if(i>1)
						{
							BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[i-2],tmp.drawCoordinates[i-1],paintEngine.brushSize*2+paintEngine.lineEdgeSize*2,true,paintEngine.customBrushBytes,paintEngine);
							BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[i-2],tmp.drawCoordinates[i-1],paintEngine.brushSize*2,true,paintEngine.patternBrushBytes,paintEngine);
							VectorBrushesTools.DrawPatternCircle((int)tmp.drawCoordinates[i-2].x, (int)tmp.drawCoordinates[i-2].y,paintEngine);
						}
					}
				}
				else
				{
					VectorBrushesTools.DrawCircle((int)tmp.drawCoordinates[0].x,(int)tmp.drawCoordinates[0].y,paintEngine);
					for(int i=1;i<tmp.drawCoordinates.Count;i++)
					{
						BitmapBrushesTools.DrawLineBrush(tmp.drawCoordinates[i-1],tmp.drawCoordinates[i],paintEngine.brushSize*2,false,null,paintEngine);
						VectorBrushesTools.DrawCircle((int)tmp.drawCoordinates[i].x,(int)tmp.drawCoordinates[i].y,paintEngine);
					}
				}
			}
	}
		/// <summary>
		/// Undo operacija.
		/// </summary>
		public void UndoLastStep()
		{
			 {
				//Debug.Log ("Undo step" + steps.Count.ToString ());
				if (!doingWork && steps.Count > 0) {
					doingWork = true;
					//pop top of stack and push it to redo stack
					//we dont need it for undo draw
					UStep step = steps.Pop ();
					switch(step.type)
					{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
						//restore initial texture state
						paintEngine.CopyUndoPixels(paintEngine.pixels);
						//redraw all steps from bottom to top
						UndoRedrawSteps();
						doingWork = false;
						break;
					default:
						break;
					}
					//push this step on undo stack
					redoSteps.Push (step);
					
				}
			}
			
		}
		/// <summary>
		/// Redo operacija.
		/// </summary>
		public void RedoLastStep()
		{
			 {
				//Debug.Log ("Redo step" + redoSteps.Count.ToString ());
				if (!doingWork && redoSteps.Count > 0) {
					doingWork = true;
					UStep step = redoSteps.Pop ();
					
					switch(step.type)
					{
					case 0:
					case 1:
					case 2:
					case 3:
					case 4:
						 {//BitmapDraw
						UStep settings= new UStep();
						settings.SetStepPropertiesFromEngine(paintEngine);
						
						
						if(step.type>-1|| step.type<5)
						{
							Debug.Log ("Redo STEP EXEC");
							step.SetPropertiesFromStep(paintEngine);
							if(step.type==0)
							{
								switch(step.brushMode)
								{
								case BrushProperties.Default:
									Debug.Log ("DEF-R MODE");
									paintEngine.ReadCurrentCustomBrush();
									for (int i=0; i<step.drawCoordinates.Count; i++)
										BitmapBrushesTools.DrawCustomBrush2 ((int)step.drawCoordinates [i].x, (int)step.drawCoordinates [i].y,paintEngine);
									break;
								case BrushProperties.Simple:
									//FIX REDO
									paintEngine.ReadCurrentCustomBrush();
									for (int i=0; i<step.drawCoordinates.Count; i++)
										BitmapBrushesTools.DrawCustomBrush2 ((int)step.drawCoordinates [i].x, (int)step.drawCoordinates [i].y,paintEngine);
									break;
								case BrushProperties.Pattern:
									paintEngine.ReadCurrentCustomBrush();
									paintEngine.ReadCurrentCustomPattern(step.patternTexture);
									for (int i=0; i<step.drawCoordinates.Count; i++)
										BitmapBrushesTools.DrawCustomBrush2 ((int)step.drawCoordinates [i].x, (int)step.drawCoordinates [i].y,paintEngine);
									break;
								default:
									break;
									
								}
							}else
								if(step.type==2)
							{
								for (int i=0; i<step.drawCoordinates.Count; i++)
									if(paintEngine.useSmartFloodFill)
								{
									FloodFillTools.FloodFillAutoMaskWithThreshold((int)step.drawCoordinates[i].x, (int)step.drawCoordinates[i].y,paintEngine);
								}else
								{
									if (step.useTreshold)
								{
									if (step.useMaskLayerOnly)
									{
										FloodFillTools.FloodFillMaskOnlyWithThreshold((int)step.drawCoordinates[i].x, (int)step.drawCoordinates[i].y,paintEngine);
									}else{
										FloodFillTools.FloodFillWithTreshold((int)step.drawCoordinates[i].x, (int)step.drawCoordinates[i].y,paintEngine);
									}
								}else{
									if (step.useMaskLayerOnly)
									{
										FloodFillTools.FloodFillMaskOnly((int)step.drawCoordinates[i].x, (int)step.drawCoordinates[i].y,paintEngine);
									}else{
										FloodFillTools.FloodFill((int)step.drawCoordinates[i].x, (int)step.drawCoordinates[i].y,paintEngine);
									}
								}
								}
							}else
							if(step.type==1)
							{
								
								
								if(step.brushMode==BrushProperties.Default)
								{
									if(step.vectorBrushType==VectorBrush.Circle)
									{
										for (int i=0; i<step.drawCoordinates.Count; i++)
											VectorBrushesTools.DrawCircle((int)step.drawCoordinates[i].x, (int)step.drawCoordinates[i].y,paintEngine);
									}
									else
									{
										for (int i=0; i<step.drawCoordinates.Count; i++)
											VectorBrushesTools.DrawRectangle((int)step.drawCoordinates[i].x, (int)step.drawCoordinates[i].y,paintEngine);	
									}
								
								}else
								if(step.brushMode==BrushProperties.Pattern)
								{
									paintEngine.ReadCurrentCustomPattern(step.patternTexture);
									if(step.vectorBrushType==VectorBrush.Circle)
									{
										for (int i=0; i<step.drawCoordinates.Count; i++)
											VectorBrushesTools.DrawPatternCircle((int)step.drawCoordinates[i].x, (int)step.drawCoordinates[i].y,paintEngine);
									}else
									{
										for (int i=0; i<step.drawCoordinates.Count; i++)
											VectorBrushesTools.DrawPatternRectangle((int)step.drawCoordinates[i].x, (int)step.drawCoordinates[i].y,paintEngine);
									}
								}
								
								
							}else if(step.type==4)
							{

								if(paintEngine.multitouchEnabled)
									DrawMultiTouchLine(step);
								else
									DrawSingleTouchLine(step);
							}
							
							paintEngine.tex.LoadRawTextureData (paintEngine.pixels);
							paintEngine.tex.Apply ();
							//return it to stack
							steps.Push(step);
						}
						else
							steps.Push(step);
						
						settings.SetPropertiesFromStep(paintEngine);
						}
						break;
					default:
						break;
					}
					
					
					//steps.Push (step);
					doingWork = false;
				}
				
			}
		}
		

}
/// <summary>
/// UStep klasa za cuvanje podesavanja AMP-a za UNDO i druge potrebe.Interno se koristi.
/// </summary>
public class UStep
{
		public int type;//step type ,if extending this class use negative integers for added custom types
		//0 draw bitmap - bitmap brush
		//1 draw bitmap - vector brush
		//2 draw bitmap - floodfill brush
		//3 ?
		//4 draw line
		public BrushProperties brushMode;
		public DrawMode drawMode;
		public int brushSize;// only for vector brushes
		public Texture2D brushTexture;
		public Texture2D patternTexture;
		public bool useAdditiveColors;
		public bool canDrawOnBlack;
		public Color paintColor;
		public bool useLockArea;
		public bool useCustomBrushAlpha;
		public int vectorBrushIndex;
		public bool isFloodFill;
		public bool connectBrushStrokes;
		//public bool interpolation;
		public bool useMaskLayerOnly;
		public bool useTreshold;
		public bool treshold;
		public bool useMaskImage;
		public float brushAlphaStrength;
		public List<Vector2> drawCoordinates;
		//
		public VectorBrush vectorBrushType;
		public int brushWidth;
		public int brushHeight;
		public bool isLine;
		public int lineEgdeSize;
		public bool isPatternLine;
		public List<TouchCoordinates> touchCoordinates;
		
		/// <summary>
		/// Sets the engine properties from step.
		/// </summary>
		/// <param name="paintEngine">Paint engine.</param>
		public void SetPropertiesFromStep(AdvancedMobilePaint paintEngine )
		{
			paintEngine.brushMode=this.brushMode;
			paintEngine.drawMode=this.drawMode;
			paintEngine.brushSize=this.brushSize;
			paintEngine.customBrush=this.brushTexture;
			paintEngine.pattenTexture=this.patternTexture;
			paintEngine.useAdditiveColors=this.useAdditiveColors;
			paintEngine.canDrawOnBlack=this.canDrawOnBlack;
			paintEngine.paintColor=this.paintColor;
			paintEngine.useLockArea=this.useLockArea;
			paintEngine.useCustomBrushAlpha=this.useCustomBrushAlpha;
			paintEngine.connectBrushStokes=this.connectBrushStrokes;
			//paintEngine.doInterpolation=this.interpolation;
			paintEngine.useMaskLayerOnly=this.useMaskLayerOnly;
			paintEngine.useThreshold=this.useTreshold;
			paintEngine.useMaskImage=this.useMaskImage;
			paintEngine.brushAlphaStrength=this.brushAlphaStrength;
			paintEngine.vectorBrushType=this.vectorBrushType;
			paintEngine.customBrushWidth=this.brushWidth;
			paintEngine.customBrushHeight=this.brushHeight;
			paintEngine.isLinePaint=this.isLine;
			paintEngine.lineEdgeSize=this.lineEgdeSize;
			paintEngine.isPatternLine=this.isPatternLine;
			//
		
		}
		/// <summary>
		/// Sets the step properties from engine.
		/// </summary>
		/// <param name="paintEngine">Paint engine.</param>
		public void SetStepPropertiesFromEngine(AdvancedMobilePaint paintEngine)
		{
			this.brushMode=paintEngine.brushMode;
			this.drawMode=paintEngine.drawMode;
			this.brushSize=paintEngine.brushSize;
			this.brushTexture=paintEngine.customBrush;
			this.patternTexture=paintEngine.pattenTexture;
			this.useAdditiveColors=paintEngine.useAdditiveColors;
			this.canDrawOnBlack=paintEngine.canDrawOnBlack;
			this.paintColor=paintEngine.paintColor;
			this.useLockArea=paintEngine.useLockArea;
			this.useCustomBrushAlpha=paintEngine.useCustomBrushAlpha;
			this.connectBrushStrokes=paintEngine.connectBrushStokes;
			//this.interpolation=paintEngine.doInterpolation;
			this.useMaskLayerOnly=paintEngine.useMaskLayerOnly;
			this.useTreshold=paintEngine.useThreshold;
			this.useMaskImage=paintEngine.useMaskImage;
			this.brushAlphaStrength=paintEngine.brushAlphaStrength;
			this.vectorBrushType=paintEngine.vectorBrushType;
			this.brushHeight=paintEngine.customBrushHeight;
			this.brushWidth=paintEngine.customBrushWidth;
			this.isLine=paintEngine.isLinePaint;
			this.isPatternLine=paintEngine.isPatternLine;
			this.lineEgdeSize=paintEngine.lineEdgeSize;
		}
		
		public void AddTouchCoordinate(int fId)
		{
			Debug.Log ("ADD TOUCH COORDINATE");
			if(touchCoordinates==null)
			{
				touchCoordinates= new List<TouchCoordinates>();
			}
			if(touchCoordinates.Count<1)
			{
				Debug.Log ("ADD TOUCH COORDINATE FIRST TIME");
				TouchCoordinates tc= new TouchCoordinates();
				tc.coordinatesIndex= new List<int>();
				tc.fingerId=fId;
				tc.coordinatesIndex.Add(drawCoordinates.Count-1);
				touchCoordinates.Add(tc);
				
			}else
			{
			//
				bool yes=false;
				for(int i=0;i<touchCoordinates.Count;i++)
				{
					if(touchCoordinates[i].fingerId==fId)
					{
						Debug.Log ("ADD TOUCH COORDINATE EXISTING");
						touchCoordinates[i].coordinatesIndex.Add(drawCoordinates.Count-1);
						
						yes=true;break;
					}
				}
				if(!yes)
				{
					Debug.Log ("ADD TOUCH NEW");
					TouchCoordinates tc= new TouchCoordinates();
					tc.coordinatesIndex= new List<int>();
					tc.fingerId=fId;
					tc.coordinatesIndex.Add(drawCoordinates.Count-1);
					touchCoordinates.Add(tc);
				}
			}
			
			
		}
}

	public class TouchCoordinates
	{
		public int fingerId;
		public List<int> coordinatesIndex;
	}

}
