//Advanced Mobile Paint - engine i API za iscrtavanje tekstura na mobilnim uredjajima  
//baziran na MobilePaint.cs skripti
//minimalna verija Unity-ja koju podrzava je 4.6.1
//verzija 1.3,mart 2017. 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using AdvancedMobilePaint.Tools;

namespace AdvancedMobilePaint
{
	// list of drawmodes
	public enum DrawMode
	{
		Default,//color vector brush drawing mode
		CustomBrush,//bitmap brush draw mode 
		FloodFill,//flood fill brush draw mode
		Pattern,//pattern vector brush draw mode
		//Stamp// TODO :this draws brush but it does not do Draw line between two brush positions in same gesture
	}
	//brush settings 
	public enum BrushProperties
	{
		Clear,//eraser-reads brush shape and size but sets all pixels transparent 
		Default,//default - reads brush shape and size but sets base color only
		Simple,//simple - sets non transparent brush pixels by copying data from brush
		Pattern//pattern- sets pattern non transparent pixels using brush size and shape
	}
	//vector brush settings
	public enum VectorBrush
	{
		Circle, //default , more modes to be included in future
		Rectangular // rectangular mode  
	}
	/// <summary>
	/// Advanced mobile paint engine.
	/// </summary>
public class AdvancedMobilePaint : MonoBehaviour {
		/// <summary>
		/// Flag koji oznacava da li je interakcija(promena teksture) dozvoljena.
		/// true= moze da boji ,false ne moze
		/// </summary>
		public bool drawEnabled=false;
		/// <summary>
		/// Pokazivac na Undo controller.
		/// </summary>
		public PaintUndoManager undoController;
		/// <summary>
		/// The brush mode.
		///	Setuje mod brush-a.
		/// </summary>
		public BrushProperties brushMode=BrushProperties.Default;
		/// <summary>
		/// The paint layer mask.
		/// </summary>
		public LayerMask paintLayerMask;
		/// <summary>
		/// The create canvas mesh flag.
		/// Setujete ovaj fleg na true ako je ova komponenta u canvasu.
		/// </summary>
		public bool createCanvasMesh=false;
		/// <summary>
		/// The connect brush stokes flag.
		/// </summary>
		public bool connectBrushStokes=true; // if brush moves too fast, then connect them with line. NOTE! Disable this if you are painting to custom mesh
	
		//public bool doInterpolation = false;
		/// <summary>
		/// The color of the paint.
		/// </summary>
		public Color32 paintColor = new Color32(255,0,0,255);
		//public float resolutionScaler = 1.0f; // 1 means screen resolution, 0.5f means half the screen resolution
		public int brushSize = 24; // default brush size
		/// <summary>
		/// The use additive colors flag.
		/// </summary>
		public bool useAdditiveColors = true; // true = alpha adds up slowly, false = 1 click will instantly set alpha to brush or paint color alpha value
		/// <summary>
		/// The brush alpha strength.
		/// </summary>
		public float brushAlphaStrength = 0.5f; // multiplier to soften brush additive alpha, 0.1f is nice & smooth, 1 = faster
		/// <summary>
		/// The draw mode.
		/// </summary>
		public DrawMode drawMode = DrawMode.CustomBrush; //
		/// <summary>
		/// The use lock area.
		/// </summary>
		public bool useLockArea=false; // locking mask: only paint in area of the color that your click first
		/// <summary>
		/// The use mask layer only.
		/// </summary>
		public bool useMaskLayerOnly = false; // if true, only check pixels from mask layer, not from the painted texture
		/// <summary>
		/// The use threshold.
		/// </summary>
		public bool useThreshold = false;
		/// <summary>
		/// The paint threshold.
		/// </summary>
		public byte paintThreshold = 128; // 0 = only exact match, 255 = match anything
		
		//lock maska za iscrtavanje
		[HideInInspector]
		public byte[] lockMaskPixels; // locking mask pixels
		
		/// <summary>
		/// The can draw on black.
		/// </summary>
		public bool canDrawOnBlack=true; // to stop filling on mask black lines, FIXME: not working if its not pure black..
		//ne menjaj! osim ako ne znas sta radis
		
		public string targetTexture = "_MainTex"; // target texture for this material shader (usually _MainTex)
		
		public FilterMode filterMode = FilterMode.Point;
		
		// clear color
		public Color32 clearColor = new Color32(255,255,255,255);
		
		// for using texture on canvas
		/// <summary>
		/// The use mask image.
		/// </summary>
		public bool useMaskImage=false;
		/// <summary>
		/// The mask texture.
		/// </summary>
		public Texture2D maskTex;
		
		// for using custom brushes
		//public bool useCustomBrushes=true;
		/// <summary>
		/// The custom brush texture.
		/// </summary>
		public Texture2D/*[]*/ customBrush;
		/// <summary>
		/// The use custom brush alpha.
		/// </summary>
		public bool useCustomBrushAlpha=true; // true = use alpha from brush, false = use alpha from current paint color
		//public int selectedBrush = 0; // currently selected brush index
		
		//private Color[] customBrushPixels;
		[HideInInspector]
		public byte[] customBrushBytes;
		
		public int customBrushWidth;
		public int customBrushHeight;
		public int customBrushWidthHalf;
		//private int customBrushHeightHalf;
		private int texWidthMinusCustomBrushWidth;
		//private int texHeightMinusCustomBrushHeight;
		
		// PATTERNS
		/// <summary>
		/// The custom pattern texture.
		/// </summary>
		public  Texture2D customPattern;
		[HideInInspector]
		public byte[] patternBrushBytes;
		[HideInInspector]
		public int customPatternWidth;
		[HideInInspector]
		public int customPatternHeight;
		//public int selectedPattern=0;
		// UNDO
		private byte[] undoPixels; // undo buffer
		//private List<byte[]> undoPixels; // undo buffer(s)
		/// <summary>
		/// The undo enabled flag.
		/// </summary>
		public bool undoEnabled = false;
		// undo step used internaly dont change
		UStep   drawUndoStep= null;
		/// <summary>
		/// The draw texture pixels.
		/// </summary>
		[HideInInspector]
		public byte[] pixels; // byte array for texture painting, this is the image that we paint into.
		[HideInInspector]
		public  byte[] maskPixels; // byte array for mask texture
		private byte[] clearPixels; // byte array for clearing texture
		/// <summary>
		/// The drawing texture.
		/// </summary>
		public Texture2D tex; // texture that we paint into (it gets updated from pixels[] array when painted)
		//private Texture2D maskTex; // texture used as a overlay mask
		[HideInInspector]
		public int texWidth;
		[HideInInspector]
		public int texHeight;
		private Touch touch; // touch reference
		private Camera cam; // main camera reference
		private RaycastHit hit;
		private bool wentOutside=false;
		//UNUSED KEPT FOR COMPATIBILITY
		private bool usingClearingImage = false; // did we have initial texture as maintexture, then use it as clear pixels array
		
		private Vector2 pixelUV; // with mouse
		private Vector2 pixelUVOld; // with mouse
		
		private Vector2[] pixelUVs; // mobiles
		private Vector2[] pixelUVOlds; // mobiles
		
		[HideInInspector]
		public bool textureNeedsUpdate = false; // if we have modified texture

		public Texture2D pattenTexture;
		//Sprite imageSprite;
		/// <summary>
		/// The ray source if alternative ray is used.
		/// </summary>
		public Transform raySource;
		/// <summary>
		/// The use alternative ray flag. Set true if you are using alternative ray source.
		/// </summary>
		public bool useAlternativeRay=false;
		/// <summary>
		/// The multitouch enabled flag.
		/// </summary>
		public bool multitouchEnabled=false;
		/// <summary>
		/// The is line paint flag. Set to true if you want tuo use vector line brush
		/// </summary>
		public bool isLinePaint=false;
		/// <summary>
		/// The size of the line edge.
		/// </summary>
		public int lineEdgeSize=0;//0=no edge, >0 = edge

		private Vector2 pixelUV_older;
		private Vector2[] pixelUV_olders;
		/// <summary>
		/// The type of the vector brush.
		/// </summary>
		public VectorBrush vectorBrushType = VectorBrush.Circle;
		/// <summary>
		/// The is pattern line flag. Set to true if you want to pattern core and adge of pattern.
		/// </summary>
		public bool isPatternLine=false;
		/// <summary>
		/// The brush size tmp.
		/// </summary>
		private int brushSizeTmp=24;
		
		/// <summary>
		/// The use smart flood fill flag. Set true to use floodFill brush which automatically detects borders and uses pixels where all chanels are lower than threshold as border pixeles.
		/// </summary>
		public bool useSmartFloodFill=false;
		
		// NAPOMENA: inicijalizacija je prebacena u Start! Korigujte script execution order  
		void Start()
		{
			//InitializeEverything();	
		}
		
		public void InitializeEverything() 
		{
			// WARNING: fixed maximum amount of touches, is set to 20 here. Not sure if some device supports more?
			pixelUVs = new Vector2[20];
			pixelUVOlds = new Vector2[20];
			pixelUV_olders= new Vector2[20];
			if (createCanvasMesh)
			{
				CreateCanvasQuad();
			}else{ // using existing mesh
				//if (connectBrushStokes) Debug.LogWarning("Custom mesh used, but connectBrushStokes is enabled, it can cause problems on the mesh borders wrapping");				
				if (GetComponent<MeshCollider>()==null) Debug.LogError("MeshCollider is missing, won't be able to raycast to canvas object");
				if (GetComponent<MeshFilter>()==null || GetComponent<MeshFilter>().sharedMesh==null) Debug.LogWarning("Mesh or MeshFilter is missing, won't be able to see the canvas object");
			}
			// create texture
			if (useMaskImage)
			{
				// check if its assigned
				if (maskTex == null)
				{
					Debug.LogWarning("maskImage is not assigned. Setting 'useMaskImage' to false");
					useMaskImage = false;
				}else{
					// Check if we have correct material to use mask image (layer)
					if (GetComponent<Renderer>().material.name.StartsWith("CanvasWithAlpha") || GetComponent<Renderer>().material.name.StartsWith("CanvasDefault"))
					{
						// FIXME: this is bit annoying to compare material names..
						Debug.LogWarning("CanvasWithAlpha and CanvasDefault materials do not support using MaskImage (layer). Disabling 'useMaskImage'");
						Debug.LogWarning("CanvasWithAlpha and CanvasDefault materials do not support using MaskImage (layer). Disabling 'useMaskLayerOnly'");
						useMaskLayerOnly = false;
						
						useMaskImage = false;
						maskTex = null;
					}else{

						texWidth = maskTex.width;
						texHeight = maskTex.height;
						GetComponent<Renderer>().material.SetTexture("_MaskTex", maskTex);
						
					}
				}
				
			}else{	// no mask texture
				// calculate texture size from screen size
				if(tex!=null)
				{
				texWidth=tex.width;
				texHeight=tex.height;
				}
				else
				texWidth = 0;//(int)(Screen.width*resolutionScaler+canvasSizeAdjust.x);
				texHeight =0;// (int)(Screen.height*resolutionScaler+canvasSizeAdjust.y);
			}
			
			// TODO: check if target texture exists
			if (!GetComponent<Renderer>().material.HasProperty(targetTexture)) Debug.LogError("Fatal error: Current shader doesn't have a property: '"+targetTexture+"'");
			
			// we have no texture set for canvas
			if (GetComponent<Renderer>().material.GetTexture(targetTexture)==null)
			{
				// create new texture
				tex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
				GetComponent<Renderer>().material.SetTexture(targetTexture, tex);
				
				// init pixels array
				pixels = new byte[texWidth * texHeight * 4];
				
			}else{ // we have canvas texture, then use that as clearing texture
				
				usingClearingImage = true;
				
				texWidth = GetComponent<Renderer>().material.GetTexture(targetTexture).width;
				texHeight = GetComponent<Renderer>().material.GetTexture(targetTexture).height;
				
				// init pixels array
				pixels = new byte[texWidth * texHeight * 4];

				tex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
				
				// we keep current maintex and read it as "clear pixels array"
				ReadClearingImage();
				
				GetComponent<Renderer>().material.SetTexture(targetTexture, tex);
			}
			ClearImage();
			
			// set texture modes
			tex.filterMode = filterMode;
			tex.wrapMode = TextureWrapMode.Clamp;
			//tex.wrapMode = TextureWrapMode.Repeat;
			
			if (useMaskImage)
			{
				//ReadMaskImage();
				SetDrawingMask(maskTex);
			}
			
			// undo system
			if (undoEnabled)
			{
				undoPixels = new byte[texWidth * texHeight * 4];
				System.Array.Copy(pixels,undoPixels,pixels.Length);
			}
			
			// locking mask enabled
			if (useLockArea)
			{
				lockMaskPixels = new byte[texWidth * texHeight * 4];
			}
			ReadCurrentCustomBrush ();
	
		} // InitializeEverything
	
	// Update is called once per frame
		void Update () 
		{
			if (drawEnabled) {

				if(!multitouchEnabled)
				{
					if(isLinePaint)
						MousePaint2();
					else
						MousePaint ();
				}
				else 
					TouchPaint();
				UpdateTexture();	
			}	
		} 
	
	
	//FUNKCIJE
	/// <summary>
	/// Dirktno iscrtavanje na povrsini teksture (van enable paint stanja)
	/// </summary>
	/// <param name="raySource">Ray source.</param>
	public void ImmediateDraw(Transform raySource)
	{
			Ray ray= new Ray(raySource.position,new Vector3(0f,0f,1f));
			if (!Physics.Raycast ( Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)) /* ray*/, out hit, Mathf.Infinity  , paintLayerMask )) {  /*Debug.Log ("WENT OUTSIDE COLLIDER"); */return;}
			if(hit.collider!=gameObject.GetComponent<Collider>()) { Debug.Log("HIT SOME OTHER COLLIDER");return;}
			//pixelUVOld = pixelUV; // take previous value, so can compare them
			Vector2 pixelUV_t = hit.textureCoord;
			pixelUV_t.x *= texWidth;
			pixelUV_t.y *= texHeight;
			// lets paint where we hit
			switch (drawMode)
			{
			case DrawMode.Default: // drawing
				if(brushMode==BrushProperties.Default)
					VectorBrushesTools.DrawCircle((int)pixelUV_t.x, (int)pixelUV_t.y,this);
				else if (brushMode==BrushProperties.Pattern)
					VectorBrushesTools.DrawPatternCircle((int)pixelUV_t.x, (int)pixelUV_t.y,this);
				break;
			case DrawMode.Pattern: // draw with pattern	
				VectorBrushesTools.DrawPatternCircle((int)pixelUV_t.x, (int)pixelUV_t.y,this);
				break;
			case DrawMode.CustomBrush: // custom brush
				BitmapBrushesTools.DrawCustomBrush2((int)pixelUV_t.x, (int)pixelUV_t.y,this);
				break;	
			case DrawMode.FloodFill: // floodfill
			//	if (pixelUVOld == pixelUV) break;
				if(useSmartFloodFill)
				{
					FloodFillTools.FloodFillAutoMaskWithThreshold((int)pixelUV_t.x,(int)pixelUV_t.y,this);
				}
				else
				{
				if (useThreshold)
				{
					if (useMaskLayerOnly)
					{
						FloodFillTools.FloodFillMaskOnlyWithThreshold((int)pixelUV_t.x, (int)pixelUV_t.y,this);
					}else{
						FloodFillTools.FloodFillWithTreshold((int)pixelUV_t.x, (int)pixelUV_t.y,this);
					}
				}else{
					if (useMaskLayerOnly)
					{
						FloodFillTools.FloodFillMaskOnly((int)pixelUV_t.x, (int)pixelUV_t.y,this);
					}else{
						FloodFillTools.FloodFill((int)pixelUV_t.x,(int)pixelUV_t.y,this);
					}
				}
				}
				break;		
			default: // unknown mode
				Debug.LogWarning("AMP: Unknown drawing mode:"+drawMode);
				break;
			}
			textureNeedsUpdate=true;
			UpdateTexture();
			
	}
	
	/// <summary>
	/// Funkcija utvrdjuje da li je raycast sa screenPosition-a(npr Input.mousePosition) pogadja nezamaskirani deo teksture na kojoj se crta.
	/// </summary>
	/// <returns><c>true</c> ako raycast pogadja nezamaskirani deo teksture; otherwise, <c>false</c>.</returns>
	/// <param name="screenPosition">Pocetna pozicija ray-a(npr. Input.mousePosition).</param>
	public bool IsRaycastInsideMask(Vector3 screenPosition)
	{

				//Vector3 newX= Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (!Physics.Raycast (Camera.main.ScreenPointToRay(screenPosition), out hit, Mathf.Infinity, paintLayerMask))
		{
			#if UNITY_EDITOR
			Debug.Log("AMP: RAY IS NOT HITTING THIS MESH COLLIDER AT ALL!");
			#endif
			return false;
		}
		if(hit.collider!=gameObject.GetComponent<Collider>()) 
		{
				#if UNITY_EDITOR
				Debug.Log("AMP: RAY IS NOT HITTING THIS MESH COLLIDER AT ALL!");
				#endif
				return false;
		}
				
		Vector2 pixelUV1 = hit.textureCoord;
		pixelUV1.x *= texWidth;
		pixelUV1.y *= texHeight;
		int startX1=((int)pixelUV1.x);
		int startY1=((int)pixelUV1.y);
		int pixel1 = (texWidth*startY1+startX1)*4;
		if((pixel1<0|| pixel1>=pixels.Length))//NOTE: Primetiti da exception na ovoj liniji koda ukazuje na to da glavna tekstura za crtanje nije generisana ili ucitana, generisite/ucitajte je pre poziva ove funkcije
		{
			#if UNITY_EDITOR
			Debug.Log("AMP: RAY IS OUT OF BOUNDS OF MASK TEXTURE AND OUTSIDE DRAWABLE AREA");
			#endif
			return false;
		}
		else
			if(lockMaskPixels[pixel1]==1) //NOTE: Primetiti da exception na ovoj liniji koda ukazuje na to da lock maska nije generisana, generisite je pre poziva ove funkcije
			{
				#if UNITY_EDITOR
				Debug.Log("AMP: RAY IS IN BOUNDS OF MASK TEXTURE AND INSIDE DRAWABLE AREA ");
				#endif
				return true;
					
			}
			else
			{
					#if UNITY_EDITOR
					Debug.Log("AMP: RAY IS OUT OF BOUNDS OF MASK BUT INSIDE DRAWABLE AREA");
					#endif
					return false;
			}
		return false;
			
	}//END IsRaycastInsideMask


	 
		void MousePaint () 
		{
			if (Input.GetMouseButtonDown(0)&& Input.touchCount<=1)
			{	
				if (useLockArea)
				{
					if (!Physics.Raycast (useAlternativeRay?Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)):Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, paintLayerMask)) return;
					if(hit.collider!=gameObject.GetComponent<Collider>()) return;
 
					Input.multiTouchEnabled=false;
					CreateAreaLockMask((int)(hit.textureCoord.x*texWidth), (int)(hit.textureCoord.y*texHeight));
					//Debug.Log("NAIL new step generated");
					if(undoEnabled)
					{
						drawUndoStep= new UStep();
						switch(drawMode)
						{
						case DrawMode.Default:
							drawUndoStep.type=1;
							break;
						case DrawMode.CustomBrush:
							drawUndoStep.type=0;
							break;
						case DrawMode.FloodFill:
							drawUndoStep.type=2;
							break;
						case DrawMode.Pattern:
							drawUndoStep.type=1;
							//FIX
							//
							break;
						}
						drawUndoStep.SetStepPropertiesFromEngine(this);
						drawUndoStep.drawCoordinates= new List<Vector2>();
					}
					
				}
			}
 
			if (Input.GetMouseButton(0)&& Input.touchCount<=1)
			{
				//Debug.Log ("PAINTING");
				// Only if we hit something, then we continue
				if (!Physics.Raycast (useAlternativeRay?Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)):Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, paintLayerMask)) {wentOutside=true; return;}
				if(hit.collider!=gameObject.GetComponent<Collider>()) {wentOutside=true; return;}
				pixelUVOld = pixelUV; // take previous value, so can compare them
				pixelUV = hit.textureCoord;
				pixelUV.x *= texWidth;
				pixelUV.y *= texHeight;
				//Debug.Log ("UV"+pixelUV);
				if (wentOutside) {pixelUVOld = pixelUV;wentOutside=false;}
				
				// lets paint where we hit
				
				
				switch (drawMode)
				{
				case DrawMode.Default: // drawing
					if(!useAdditiveColors && (pixelUVOld == pixelUV)) break;
					if(brushMode==BrushProperties.Default)
					{
						if(vectorBrushType==VectorBrush.Circle)
							VectorBrushesTools.DrawCircle((int)pixelUV.x, (int)pixelUV.y,this);
						else
							VectorBrushesTools.DrawRectangle ((int)pixelUV.x, (int)pixelUV.y,this);
					}
					else if (brushMode==BrushProperties.Pattern)
						VectorBrushesTools.DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y,this);
					textureNeedsUpdate = true;
					break;
				case DrawMode.Pattern: // draw with pattern	
					if(!useAdditiveColors && (pixelUVOld == pixelUV)) break;
						//DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y);
						{
						if(vectorBrushType==VectorBrush.Circle)
							VectorBrushesTools.DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y,this);
						else
							VectorBrushesTools.DrawPatternRectangle ((int)pixelUV.x, (int)pixelUV.y,this);
						}
					textureNeedsUpdate = true;
					break;
				case DrawMode.CustomBrush: // custom brush
					if(!useAdditiveColors && (pixelUVOld == pixelUV)) break;
					BitmapBrushesTools.DrawCustomBrush2((int)pixelUV.x, (int)pixelUV.y,this);
					textureNeedsUpdate = true;
					break;	
				case DrawMode.FloodFill: // floodfill
					//if (pixelUVOld == pixelUV) break;
					if(useSmartFloodFill)
					{
						FloodFillTools.FloodFillAutoMaskWithThreshold((int)pixelUV.x,(int)pixelUV.y,this);
					}
					else
					{
					if (useThreshold)
					{
						if (useMaskLayerOnly)
						{
							FloodFillTools.FloodFillMaskOnlyWithThreshold((int)pixelUV.x, (int)pixelUV.y,this);
						}else{
							FloodFillTools.FloodFillWithTreshold((int)pixelUV.x, (int)pixelUV.y,this);
						}
					}else{
						if (useMaskLayerOnly)
						{
							FloodFillTools.FloodFillMaskOnly((int)pixelUV.x, (int)pixelUV.y,this);
						}else{
							FloodFillTools.FloodFill((int)pixelUV.x, (int)pixelUV.y,this);
						}
					}
					}
					textureNeedsUpdate = true;
					break;		
				default: // unknown mode
					Debug.LogWarning("AMP: Unknown drawing mode:"+drawMode);
					break;
				}
				if(drawUndoStep==null && undoEnabled)
				{
					drawUndoStep= new UStep();
					switch(drawMode)
					{
					case DrawMode.Default:
						drawUndoStep.type=1;
						break;
					case DrawMode.CustomBrush:
						drawUndoStep.type=0;
						
						break;
					case DrawMode.FloodFill:
						drawUndoStep.type=2;
						break;
					case DrawMode.Pattern:
						drawUndoStep.type=1;
						break;
					}
					drawUndoStep.SetStepPropertiesFromEngine(this);
					drawUndoStep.drawCoordinates= new List<Vector2>();
				}
				if(undoEnabled )
				{
					Vector2 newCoors= new Vector2(pixelUV.x, pixelUV.y);
					drawUndoStep.drawCoordinates.Add (newCoors);
				}
				
			}
			

			if (Input.GetMouseButtonDown(0)&& Input.touchCount<=1)
			{
				// take this position as start position
				if (!Physics.Raycast (useAlternativeRay?Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)):Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, paintLayerMask)) return;
				if(hit.collider!=gameObject.GetComponent<Collider>()) return;
				pixelUVOld = pixelUV;
				
			}



			// check distance from previous drawing point and connect them with DrawLine
			if (connectBrushStokes && Vector2.Distance(pixelUV,pixelUVOld)>brushSize && Input.touchCount<=1)
			{
				switch (drawMode)
				{
				case DrawMode.Default: // drawing
					#if UNITY_EDITOR
					if (vectorBrushType==VectorBrush.Circle)// DrawLineBrush(pixelUVOld,pixelUV,brushSize,false);
						VectorBrushesTools.DrawLine(pixelUVOld, pixelUV,this);
					else //DrawLineBrush(pixelUVOld,pixelUV,brushSize,true);
						VectorBrushesTools.DrawLineWithVectorBrush(pixelUVOld, pixelUV,this);
					#endif
					#if !UNITY_EDITOR 
					if(Input.touchCount==1)
					{
						
						if(Input.GetTouch(0).phase!=TouchPhase.Stationary)
						{
							//DrawLine(pixelUVOld, pixelUV);
							if (vectorBrushType==VectorBrush.Circle)
								VectorBrushesTools.DrawLine(pixelUVOld, pixelUV,this);
							else
								VectorBrushesTools.DrawLineWithVectorBrush(pixelUVOld, pixelUV,this);
						}
					}
					
					#endif
					
					break;
					
				case DrawMode.CustomBrush:
					#if UNITY_EDITOR
					BitmapBrushesTools.DrawLineWithBrush(pixelUVOld, pixelUV,this);
					#endif
					#if !UNITY_EDITOR 
					if(Input.touchCount==1)
					{
						
						if(Input.GetTouch(0).phase!=TouchPhase.Stationary)
							BitmapBrushesTools.DrawLineWithBrush(pixelUVOld, pixelUV,this);
					}
					
					#endif
					break;
					
				case DrawMode.Pattern:
					#if UNITY_EDITOR
					VectorBrushesTools.DrawLineWithPattern(pixelUVOld, pixelUV,this);
					#endif
					#if !UNITY_EDITOR 
					if(Input.touchCount==1)
					{
						
						if(Input.GetTouch(0).phase!=TouchPhase.Stationary)
							VectorBrushesTools.DrawLineWithPattern(pixelUVOld, pixelUV,this);
					}
					
					#endif
					break;
					
				default: // other modes
					break;
				}
				pixelUVOld = pixelUV;
				textureNeedsUpdate = true;
			}
			
			if (Input.GetMouseButtonUp(0)&& Input.touchCount<=1)
			{
				//if (hideUIWhilePainting && !isUIVisible) ShowUI();
				if (drawEnabled && drawUndoStep!=null && undoEnabled) {
					UStep c= new UStep();
					c=drawUndoStep;
					undoController.AddStep (c);
					drawUndoStep=null;
				}
			}
		}
	//
		//used for line brushes due to different drawing logic
		void MousePaint2 () 
		{
			//line draw:
			//MOUSE DOWN : draw circle at start of line
			//MOUSE HOLD : if curent_position>size draw line and circle brush at the end
			//MOUSE UP: draw circle (end of line)
			if (Input.GetMouseButtonDown(0)&& Input.touchCount<=1)
			{	
				//if (useLockArea)
				//{
				if (!Physics.Raycast (useAlternativeRay?Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)):Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, paintLayerMask)) return;
				if(hit.collider!=gameObject.GetComponent<Collider>()) return;
				//Input.multiTouchEnabled=false;
				//CreateAreaLockMask((int)(hit.textureCoord.x*texWidth), (int)(hit.textureCoord.y*texHeight));
				//
				if(hit.collider!=gameObject.GetComponent<Collider>()) {wentOutside=true; return;}
				//
				pixelUV_older= pixelUV;//pixelUVOld;
				//
				pixelUVOld = pixelUV; // take previous value, so can compare them
				pixelUV = hit.textureCoord;
				pixelUV.x *= texWidth;
				pixelUV.y *= texHeight;
				//Debug.Log ("UV"+pixelUV);
				if (wentOutside) {pixelUVOld = pixelUV;pixelUV_older= pixelUV;wentOutside=false;}
				//
				brushSize= brushSize+lineEdgeSize;
				if(undoEnabled)
				{
					drawUndoStep= new UStep();
					drawUndoStep.type=4;
					drawUndoStep.SetStepPropertiesFromEngine(this);
					drawUndoStep.drawCoordinates= new List<Vector2>();
				}
				//draw start of line
				if(!wentOutside)
				{
					
					Debug.Log ("LINE ON MOUSE DOWN");
					if(lineEdgeSize>0)
					{
						if(isPatternLine)
							BitmapBrushesTools.DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y,customBrushBytes, brushSize+lineEdgeSize,this);
						else
							VectorBrushesTools.DrawCircle((int)pixelUV.x, (int)pixelUV.y,this);
					}
					if(isPatternLine)
						VectorBrushesTools.DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y,this);
					else
						VectorBrushesTools.DrawCircle((int)pixelUV.x, (int)pixelUV.y,this);
						
					textureNeedsUpdate=true;
					pixelUV_older= pixelUV;
					pixelUVOld=pixelUV;
					
				}
				
			}
			else
				if (Input.GetMouseButton(0)&& Input.touchCount<=1 && !Input.GetMouseButtonDown(0))
			{
				//Debug.Log ("PAINTING");
				// Only if we hit something, then we continue
				if (!Physics.Raycast (useAlternativeRay?Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)):Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, paintLayerMask)) {wentOutside=true; return;}
				if(hit.collider!=gameObject.GetComponent<Collider>()) {wentOutside=true; return;}
				//pixelUV_older=pixelUVOld;
				//pixelUVOld = pixelUV; // take previous value, so can compare them
				//if(pixelUV.x==pixelUVOld.x && pixelUV.y==pixelUVOld.y) return;
				pixelUV = hit.textureCoord;
				pixelUV.x *= texWidth;
				pixelUV.y *= texHeight;
				//Debug.Log ("UV"+pixelUV);
				if (wentOutside) {pixelUVOld = pixelUV;pixelUV_older=pixelUV;wentOutside=false;}
				if (/*connectBrushStokes && */Input.GetMouseButton(0) && /*Mathf.Abs*/(Vector2.Distance(pixelUV,pixelUVOld))>=brushSize && Input.touchCount<=1 )
				{
					
					Debug.Log ("LINE ON MOUSE HOLD");
					if(lineEdgeSize>0)
					{
						
						if(isPatternLine)
						{
						
							BitmapBrushesTools.DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y,customBrushBytes, brushSize+lineEdgeSize,this);
							BitmapBrushesTools.DrawLineBrush(pixelUVOld,pixelUV,brushSize*2+lineEdgeSize*2,true,customBrushBytes,this);
						//					// ovde je problem
							BitmapBrushesTools.DrawLineBrush(pixelUV_older,pixelUVOld,brushSize*2+lineEdgeSize*2,true,customBrushBytes,this);
							BitmapBrushesTools.DrawLineBrush(pixelUV_older,pixelUVOld,brushSize*2,true,patternBrushBytes,this);
							VectorBrushesTools.DrawPatternCircle((int)pixelUV_older.x, (int)pixelUV_older.y,this);
						}else
						{
							VectorBrushesTools.DrawCircle((int)pixelUV.x, (int)pixelUV.y,this);
							BitmapBrushesTools.DrawLineBrush(pixelUVOld,pixelUV,brushSize*2+lineEdgeSize*2,false,customBrushBytes,this);
							//					// 
							BitmapBrushesTools.DrawLineBrush(pixelUV_older,pixelUVOld,brushSize*2+lineEdgeSize*2,false,customBrushBytes,this);
							BitmapBrushesTools.DrawLineBrush(pixelUV_older,pixelUVOld,brushSize*2,false,patternBrushBytes,this);
							VectorBrushesTools.DrawCircle((int)pixelUV_older.x, (int)pixelUV_older.y,this);
						}
						//					//
						//					
					}
					if(isPatternLine)
					{
						BitmapBrushesTools.DrawLineBrush(pixelUVOld,pixelUV,brushSize*2,true,patternBrushBytes,this);
						VectorBrushesTools.DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y,this);
					}else
					{
						BitmapBrushesTools.DrawLineBrush(pixelUVOld,pixelUV,brushSize*2,false,patternBrushBytes,this);
						VectorBrushesTools.DrawCircle((int)pixelUV.x, (int)pixelUV.y,this);
					}
					//
					pixelUV_older=pixelUVOld;
					pixelUVOld = pixelUV;
					textureNeedsUpdate = true;
					if(drawUndoStep==null && undoEnabled)
					{
						drawUndoStep= new UStep();
						drawUndoStep.type=4;
						drawUndoStep.SetStepPropertiesFromEngine(this);
						drawUndoStep.drawCoordinates= new List<Vector2>();
					}
					if(undoEnabled )
					{
						Vector2 newCoors= new Vector2(pixelUV.x, pixelUV.y);
						drawUndoStep.drawCoordinates.Add (newCoors);
					}
					
				}
			}else
				
				if (Input.GetMouseButtonUp(0)&& Input.touchCount<=1)
			{
				
				if(drawEnabled)
				{
					
					if (!Physics.Raycast (useAlternativeRay?Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(raySource.position)):Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, paintLayerMask)) {wentOutside=true; return;}
					if(hit.collider!=gameObject.GetComponent<Collider>()) {wentOutside=true; return;}
					pixelUV_older=pixelUV;//pixelUVOld;
					pixelUVOld = pixelUV; // take previous value, so can compare them
					pixelUV = hit.textureCoord;
					pixelUV.x *= texWidth;
					pixelUV.y *= texHeight;
					//Debug.Log ("UV"+pixelUV);
					if (wentOutside) {pixelUVOld = pixelUV;pixelUV_older=pixelUV;wentOutside=false;}
					else
					{
						Debug.Log ("LINE ON MOUSE UP");
						if(lineEdgeSize>0)
						{
							if(isPatternLine)
							{
								BitmapBrushesTools.DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y,customBrushBytes, brushSize+lineEdgeSize,this);
								BitmapBrushesTools.DrawLineBrush(pixelUV_older,pixelUVOld,brushSize*2,true,patternBrushBytes,this);
								VectorBrushesTools.DrawPatternCircle((int)pixelUV_older.x, (int)pixelUV_older.y,this);
							}else
							{
								VectorBrushesTools.DrawCircle((int)pixelUV.x, (int)pixelUV.y,this);
								BitmapBrushesTools.DrawLineBrush(pixelUV_older,pixelUVOld,brushSize*2,false,patternBrushBytes,this);
								VectorBrushesTools.DrawPatternCircle((int)pixelUV_older.x, (int)pixelUV_older.y,this);
							}
						}
						if(isPatternLine)
						{
							VectorBrushesTools.DrawPatternCircle((int)pixelUV.x, (int)pixelUV.y,this);
						}else
							VectorBrushesTools.DrawCircle((int)pixelUV.x, (int)pixelUV.y,this);
						textureNeedsUpdate=true;
						
						if(undoEnabled )
						{
							Vector2 newCoors= new Vector2(pixelUV.x, pixelUV.y);
							drawUndoStep.drawCoordinates.Add (newCoors);
							
							if (drawEnabled && drawUndoStep!=null ) {
								UStep c= new UStep();
								c=drawUndoStep;
								undoController.AddStep (c);
								drawUndoStep=null;
							}
						}
					}
					brushSize-=lineEdgeSize;
				}
			}
		}
	//
	
		void TouchPaint()
		{
			int i = 0;
			
			while (i < Input.touchCount) 
			{
				touch = Input.GetTouch(i);
				i++;
			}
			
			i=0;
			// loop until all touches are processed
			while (i < Input.touchCount) 
			{
				
				touch = Input.GetTouch(i);
				if (touch.phase == TouchPhase.Began) 
				{

						if (!Physics.Raycast (Camera.main.ScreenPointToRay(touch.position), out hit, Mathf.Infinity, paintLayerMask)) {wentOutside=true; return;}
						else if(hit.collider!=gameObject.GetComponent<Collider>()){{wentOutside=true; return;}}

						
						pixelUVs[touch.fingerId] = hit.textureCoord;
						pixelUVs[touch.fingerId].x *= texWidth;
						pixelUVs[touch.fingerId].y *= texHeight;
						pixelUVOlds [touch.fingerId] = pixelUVs [touch.fingerId];
						pixelUV_olders[touch.fingerId]= pixelUVs[touch.fingerId];//pixelUVOld;
						if (wentOutside) {pixelUVOlds [touch.fingerId] = pixelUVs [touch.fingerId];pixelUV_olders[touch.fingerId]= pixelUVs[touch.fingerId];;wentOutside=false;}
					if (useLockArea)
					{
						CreateAreaLockMask((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
					}
					if(undoEnabled && drawUndoStep==null )
					{
						drawUndoStep= new UStep();
						switch(drawMode)
						{
						case DrawMode.Default:
							drawUndoStep.type=1;
							break;
						case DrawMode.CustomBrush:
							drawUndoStep.type=0;
							break;
						case DrawMode.FloodFill:
							drawUndoStep.type=2;
							break;
						case DrawMode.Pattern:
							drawUndoStep.type=1;
							//FIX
							//
							break;
						}
						if(isLinePaint){ drawUndoStep.type=4;brushSizeTmp=brushSize;brushSize= brushSize+lineEdgeSize;}
						drawUndoStep.SetStepPropertiesFromEngine(this);
						drawUndoStep.drawCoordinates= new List<Vector2>();
						if(isLinePaint)
							drawUndoStep.touchCoordinates= new List<TouchCoordinates>();
					}
						//drawUndoStep.					}
				}
				// check state
				if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began) 
				{
					
					// do raycast on touch position
					if (Physics.Raycast (Camera.main.ScreenPointToRay (touch.position), out hit, Mathf.Infinity, paintLayerMask)) 
					if(hit.collider==gameObject.GetComponent<Collider>())	
					{
						// take previous value, so can compare them
						pixelUVOlds [touch.fingerId] = pixelUVs [touch.fingerId];
						// get hit texture coordinate
						pixelUVs [touch.fingerId] = hit.textureCoord;
						pixelUVs [touch.fingerId].x *= texWidth;
						pixelUVs [touch.fingerId].y *= texHeight;

						if(isLinePaint)
						{
							if(lineEdgeSize>0)
							{
								
								if(isPatternLine)
								{
									
									BitmapBrushesTools.DrawPatternCircle((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,customBrushBytes, brushSize+lineEdgeSize,this);
									BitmapBrushesTools.DrawLineBrush(pixelUVOlds[touch.fingerId],pixelUVs[touch.fingerId],brushSize*2+lineEdgeSize*2,true,customBrushBytes,this);
									//					// 
									BitmapBrushesTools.DrawLineBrush(pixelUV_olders[touch.fingerId],pixelUVOlds[touch.fingerId],brushSize*2+lineEdgeSize*2,true,customBrushBytes,this);
									BitmapBrushesTools.DrawLineBrush(pixelUV_olders[touch.fingerId],pixelUVOlds[touch.fingerId],brushSize*2,true,patternBrushBytes,this);
									VectorBrushesTools.DrawPatternCircle((int)pixelUV_olders[touch.fingerId].x, (int)pixelUV_olders[touch.fingerId].y,this);
								}else
								{
									VectorBrushesTools.DrawCircle((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
									BitmapBrushesTools.DrawLineBrush(pixelUVOlds[touch.fingerId],pixelUVs[touch.fingerId],brushSize*2+lineEdgeSize*2,false,customBrushBytes,this);
									//					// 
									BitmapBrushesTools.DrawLineBrush(pixelUV_olders[touch.fingerId],pixelUVOlds[touch.fingerId],brushSize*2+lineEdgeSize*2,false,customBrushBytes,this);
									BitmapBrushesTools.DrawLineBrush(pixelUV_olders[touch.fingerId],pixelUVOlds[touch.fingerId],brushSize*2,false,patternBrushBytes,this);
									VectorBrushesTools.DrawCircle((int)pixelUV_olders[touch.fingerId].x, (int)pixelUV_olders[touch.fingerId].y,this);
								}			
							}
							if(isPatternLine)
							{
								BitmapBrushesTools.DrawLineBrush(pixelUVOlds[touch.fingerId],pixelUVs[touch.fingerId],brushSize*2,true,patternBrushBytes,this);
								VectorBrushesTools.DrawPatternCircle((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							}else
							{
								
								BitmapBrushesTools.DrawLineBrush(pixelUVOlds[touch.fingerId],pixelUVs[touch.fingerId],brushSize*2,false,patternBrushBytes,this);
								VectorBrushesTools.DrawCircle((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							}
							textureNeedsUpdate = true;
						
						}else
						switch (drawMode) 
						{
						case DrawMode.Default:
							//DrawCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							if(brushMode==BrushProperties.Default)
							{
								//DrawCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
								if(vectorBrushType==VectorBrush.Circle)
									VectorBrushesTools.DrawCircle((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
								else
									VectorBrushesTools.DrawRectangle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							}
							else if (brushMode==BrushProperties.Pattern)
								VectorBrushesTools.DrawPatternCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							textureNeedsUpdate = true;
							break;
							
						case DrawMode.CustomBrush:
							BitmapBrushesTools.DrawCustomBrush2 ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							textureNeedsUpdate = true;
							break;
							
						case DrawMode.Pattern:
							VectorBrushesTools.DrawPatternCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							textureNeedsUpdate = true;
							break;
							
						case DrawMode.FloodFill:
							if(useSmartFloodFill)
							{
								FloodFillTools.FloodFillAutoMaskWithThreshold((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							}
							else
							{
							if (useThreshold)
							{
								if (useMaskLayerOnly)
								{
									FloodFillTools.FloodFillMaskOnlyWithThreshold((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
								}else{
									FloodFillTools.FloodFillWithTreshold((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
								}
							}else{
								if (useMaskLayerOnly)
								{
									FloodFillTools.FloodFillMaskOnly((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
								}else{
									
									FloodFillTools.FloodFill((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
								}
							}
							}
							textureNeedsUpdate = true;
							break;	
						default:
							// unknown mode
							break;
						}
						//UNDO STEP CREATION AND ADD
						if(drawUndoStep==null && undoEnabled)
						{
							drawUndoStep= new UStep();
							switch(drawMode)
							{
							case DrawMode.Default:
								drawUndoStep.type=1;
								break;
							case DrawMode.CustomBrush:
								drawUndoStep.type=0;
								
								break;
							case DrawMode.FloodFill:
								drawUndoStep.type=2;
								break;
							case DrawMode.Pattern:
								drawUndoStep.type=1;
								break;
							}
							drawUndoStep.SetStepPropertiesFromEngine(this);
							drawUndoStep.drawCoordinates= new List<Vector2>();
								if(isLinePaint){
									drawUndoStep.touchCoordinates= new List<TouchCoordinates>();
							}		
						}
						if(undoEnabled )
						{
							Vector2 newCoors= new Vector2(pixelUVs[touch.fingerId].x,pixelUVs[touch.fingerId].y);
							drawUndoStep.drawCoordinates.Add (newCoors);
							if(isLinePaint) drawUndoStep.AddTouchCoordinate(touch.fingerId);
						}
						pixelUV_olders[touch.fingerId]= pixelUVOlds[touch.fingerId];//pixelUVOld;
					
				}
				}
				// if we just touched screen, set this finger id texture paint start position to that place
				if (touch.phase == TouchPhase.Began) 
				{
					pixelUVOlds [touch.fingerId] = pixelUVs [touch.fingerId];
					pixelUV_olders[touch.fingerId]= pixelUVOlds[touch.fingerId];//pixelUVOld;
				}
				// check distance from previous drawing point
				if (/*connectBrushStokes && */Vector2.Distance (pixelUVs[touch.fingerId], pixelUVOlds[touch.fingerId]) > brushSize) //TEST
				{
					//if(pd.pointerEnter!=gameObject)
					if(isLinePaint)
					{
						if(lineEdgeSize>0)
						{
							
							if(isPatternLine)
							{
								
								BitmapBrushesTools.DrawPatternCircle((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,customBrushBytes, brushSize+lineEdgeSize,this);
								BitmapBrushesTools.DrawLineBrush(pixelUVOlds[touch.fingerId],pixelUVs[touch.fingerId],brushSize*2+lineEdgeSize*2,true,customBrushBytes,this);
								//					// 
								BitmapBrushesTools.DrawLineBrush(pixelUV_olders[touch.fingerId],pixelUVOlds[touch.fingerId],brushSize*2+lineEdgeSize*2,true,customBrushBytes,this);
								BitmapBrushesTools.DrawLineBrush(pixelUV_olders[touch.fingerId],pixelUVOlds[touch.fingerId],brushSize*2,true,patternBrushBytes,this);
								VectorBrushesTools.DrawPatternCircle((int)pixelUV_olders[touch.fingerId].x, (int)pixelUV_olders[touch.fingerId].y,this);
							}else
							{
								VectorBrushesTools.DrawCircle((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
								BitmapBrushesTools.DrawLineBrush(pixelUVOlds[touch.fingerId],pixelUVs[touch.fingerId],brushSize*2+lineEdgeSize*2,false,customBrushBytes,this);
								//					// 
								BitmapBrushesTools.DrawLineBrush(pixelUV_olders[touch.fingerId],pixelUVOlds[touch.fingerId],brushSize*2+lineEdgeSize*2,false,customBrushBytes,this);
								BitmapBrushesTools.DrawLineBrush(pixelUV_olders[touch.fingerId],pixelUVOlds[touch.fingerId],brushSize*2,false,patternBrushBytes,this);
								VectorBrushesTools.DrawCircle((int)pixelUV_olders[touch.fingerId].x, (int)pixelUV_olders[touch.fingerId].y,this);
							}
							//					//
							//					
						}
						if(isPatternLine)
						{
							BitmapBrushesTools.DrawLineBrush(pixelUVOlds[touch.fingerId],pixelUVs[touch.fingerId],brushSize*2,true,patternBrushBytes,this);
							VectorBrushesTools.DrawPatternCircle((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
						}else
						{
							BitmapBrushesTools.DrawLineBrush(pixelUVOlds[touch.fingerId],pixelUVs[touch.fingerId],brushSize*2,false,patternBrushBytes,this);
							VectorBrushesTools.DrawCircle((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
						}
						//
						pixelUV_olders[touch.fingerId]=pixelUVOlds[touch.fingerId];
						pixelUVOlds[touch.fingerId] = pixelUVs[touch.fingerId];
						textureNeedsUpdate = true;
					}
					else
					switch (drawMode ) 
					{
					case DrawMode.Default:
						//DrawCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
						if(brushMode==BrushProperties.Default)
						{
//							DrawCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y);
							if(vectorBrushType==VectorBrush.Circle)
								VectorBrushesTools.DrawCircle((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							else
								VectorBrushesTools.DrawRectangle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							
						}
						else if (brushMode==BrushProperties.Pattern)
							VectorBrushesTools.DrawPatternCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
						textureNeedsUpdate = true;
						break;
						
					case DrawMode.CustomBrush:
						BitmapBrushesTools.DrawCustomBrush2 ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
						textureNeedsUpdate = true;
						break;
						
					case DrawMode.Pattern:
						VectorBrushesTools.DrawPatternCircle ((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
						textureNeedsUpdate = true;
						break;
						
					case DrawMode.FloodFill:
						if (useThreshold)
						{
							if (useMaskLayerOnly)
							{
								FloodFillTools.FloodFillMaskOnlyWithThreshold((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							}else{
								FloodFillTools.FloodFillWithTreshold((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							}
						}else{
							if (useMaskLayerOnly)
							{
								FloodFillTools.FloodFillMaskOnly((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							}else{
								
								FloodFillTools.FloodFill((int)pixelUVs[touch.fingerId].x, (int)pixelUVs[touch.fingerId].y,this);
							}
						}
						textureNeedsUpdate = true;
						break;	
					default:
						// unknown mode
						break;
					}
					//textureNeedsUpdate = true;
					
					pixelUVOlds [touch.fingerId] = pixelUVs [touch.fingerId];
					
				}
				//
				if(touch.phase==TouchPhase.Ended)
				{
					if(undoEnabled)
					{
						if(drawUndoStep!=null)
						{
							UStep c= new UStep();
							c=drawUndoStep;
							undoController.AddStep (c);
							drawUndoStep=null;
						}
						
					}
				}
				
				// loop all touches
				i++;
			}
			
//			#if ENABLE_4_6_FEATURES
			if (Input.touchCount==0)
			{
				if(undoEnabled)
				{
					if(drawUndoStep!=null)
					{
						UStep c= new UStep();
						c=drawUndoStep;
						undoController.AddStep (c);
						drawUndoStep=null;
					}
						
				}
				if(isLinePaint){ brushSize= brushSizeTmp;}
			}
//			#endif
			
		}

		
	/// <summary>
	///Updejt teksture.
	///U ovoj funkciji mozete dodati kod koji se odnosi na sve teksture koje se istovremeno boje.
	///Po defaultu se samo obradjuje glavna textura. 
	/// </summary>
	public void UpdateTexture ()
	{
		if (textureNeedsUpdate) 
			{
				textureNeedsUpdate = false;
				tex.LoadRawTextureData (pixels);
				tex.Apply (false);
				//SVE OSTALE TESTURE MOGU DA SE UPDEJTUJU OVDE
			}
	}
	//koristi se interno za UNDO/REDO potrebe,nema potrebe da koristite ovu funkciju
	public void CopyUndoPixels(byte[] destiniation)
	{
			System.Array.Copy(undoPixels,destiniation,undoPixels.Length);
	}
	/// <summary>
	/// Creates the area lock mask.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void CreateAreaLockMask(int x, int y)
	{
		if (useThreshold) 
		{
			if (useMaskLayerOnly)
			{
					FloodFillTools.LockAreaFillWithThresholdMaskOnly(x,y,this);
			}else{
					FloodFillTools.LockMaskFillWithThreshold(x,y,this);
			}
		}else{ // no threshold
			if (useMaskLayerOnly)
			{
					FloodFillTools.LockAreaFillMaskOnly(x,y,this);
			}else{
					FloodFillTools.LockAreaFill(x,y,this);
			}
		}
			//lockMaskCreated = true; // not used yet
	}
	
		/// <summary>
		/// Reads the current custom brush.This needs to be called if custom brush is changed.
		/// </summary>
		public void ReadCurrentCustomBrush()
		{
			if(customBrush==null) return ;
			
			customBrushWidth=customBrush/*[selectedBrush]*/.width;
			customBrushHeight=customBrush/*[selectedBrush]*/.height;
			customBrushBytes = new byte[customBrushWidth * customBrushHeight * 4];
			Color[] tmp =customBrush.GetPixels();
			int pixel = 0;
			for (int y = 0; y < customBrushHeight; y++) 
			{
				for (int x = 0; x < customBrushWidth; x++) 
				{
					// TODO: take colors from GetPixels
					Color brushPixel = tmp[y*customBrushHeight+x];//customBrush/*[selectedBrush]*/.GetPixel(x,y);//original
					customBrushBytes[pixel] = (byte)(brushPixel.r*255);
					customBrushBytes[pixel+1] = (byte)(brushPixel.g*255);
					customBrushBytes[pixel+2] = (byte)(brushPixel.b*255);
					customBrushBytes[pixel+3] = (byte)(brushPixel.a*255);
					//}
					pixel += 4;
				}
			}
			//precalculate values
			customBrushWidthHalf = (int)(customBrushWidth/2);
			//texWidthMinusCustomBrushWidth = texWidth-customBrushWidth;
			//texHeightMinusCustomBrushHeight = texHeight-customBrushHeight;
		}
		
		
		
		
		/// <summary>
		/// Reads the current custom pattern.reads current texture pattern into pixel array.
		/// </summary>
		/// <param name="patternTexture">Pattern texture.</param>
		 public void ReadCurrentCustomPattern(Texture2D patternTexture)
		{
			if (patternTexture==null) {/*Debug.LogError("Problem: No custom patterns assigned on "+gameObject.name);*/ return;}
			this.pattenTexture=patternTexture;
			customPatternWidth=patternTexture.width;
			customPatternHeight=patternTexture.height;
			patternBrushBytes = new byte[customPatternWidth * customPatternHeight * 4];
			Color[] tmp =patternTexture.GetPixels();
			int pixel = 0;
			for (int x = 0; x < customPatternWidth; x++)
			{
				for (int y = 0; y < customPatternHeight; y++)
				{
					Color brushPixel = tmp[y*customPatternHeight+x];//patternTexture.GetPixel(x,y);
					
					patternBrushBytes[pixel] = (byte)(brushPixel.r*255);
					patternBrushBytes[pixel+1] = (byte)(brushPixel.g*255);
					patternBrushBytes[pixel+2] = (byte)(brushPixel.b*255);
					patternBrushBytes[pixel+3] = (byte)(brushPixel.a*255);
					
					pixel += 4;
				}
			}
		}
		
		// draws single point to this pixel coordinate, with current paint color
//		public void DrawPoint(int x,int y)
//		{
//			int pixel = (texWidth*y+x)*4;
//			if(brushMode!=BrushProperties.Pattern)
//			{
//			pixels[pixel] = paintColor.r;
//			pixels[pixel+1] = paintColor.g;
//			pixels[pixel+2] = paintColor.b;
//			pixels[pixel+3] = paintColor.a;
//			}
//			//FIXME!
//			
//			else
//			{
//				float yy = Mathf.Repeat(y,customPatternWidth);
//				float xx = Mathf.Repeat(x,customPatternWidth);
//				int pixel2 = (int) Mathf.Repeat( (customPatternWidth*xx+yy)*4, patternBrushBytes.Length);
//				pixels[pixel] =patternBrushBytes[pixel2];
//				pixels[pixel+1] = patternBrushBytes[pixel2+1];
//				pixels[pixel+2] = patternBrushBytes[pixel2+2];
//				pixels[pixel+3] = patternBrushBytes[pixel2+3];
//			}
//		}
		
		
		// draws single point to this pixel array index, with current paint color
//		public void DrawPoint(int pixel)
//		{
//			pixels[pixel] = paintColor.r;
//			pixels[pixel+1] = paintColor.g;
//			pixels[pixel+2] = paintColor.b;
//			pixels[pixel+3] = paintColor.a;
//		}
		
		
		// init/clear image, this can be called outside this script also
		public void ClearImage()
		{
			if (usingClearingImage)
			{
				ClearImageWithImage();
			}
			
			
			else{
				
				int pixel = 0;
				for (int y = 0; y < texHeight; y++) 
				{
					for (int x = 0; x < texWidth; x++) 
					{
						pixels[pixel] = clearColor.r;
						pixels[pixel+1] = clearColor.g;
						pixels[pixel+2] = clearColor.b;
						pixels[pixel+3] = clearColor.a;
						pixel += 4;
						
					}
				}
				if(tex !=null && pixels.Length >0)
				{
					tex.LoadRawTextureData(pixels);
					tex.Apply(true);
				}
				
			}
		} // clear image
		
		
		public void ClearImageWithImage()
		{
			// fill pixels array with clearpixels array
			System.Array.Copy(clearPixels,0,pixels,0,clearPixels.Length);
			
			
			// just assign our clear image array into tex
			tex.LoadRawTextureData(clearPixels);
			tex.Apply(false);
		} // clear image
		
		
		public void ReadMaskImage_old()
		{
			maskPixels = new byte[texWidth * texHeight * 4];
			Color [] c= maskTex.GetPixels(0);
			int pixel = 0;
			for (int y = 0; y < texHeight; y++) 
			{
				for (int x = 0; x < texWidth; x++) 
				{
					//Color c = maskTex.GetPixel(x,y);	
					maskPixels[pixel] = (byte)(c[pixel].r*255);
					maskPixels[pixel+1] = (byte)(c[pixel].g*255);
					maskPixels[pixel+2] = (byte)(c[pixel].b*255);
					maskPixels[pixel+3] = (byte)(c[pixel].a*255);
					pixel += 4;
				}
			}
			
		}

		//izmena---------------------
		public void ReadMaskImage()
		{
			maskPixels = new byte[texWidth * texHeight * 4];
			Color [] c= maskTex.GetPixels(0);
			int pixel = 0;
			int _pixel = 0;
			for (int y = 0; y < texHeight; y++) 
			{
				for (int x = 0; x < texWidth; x++) 
				{
					//Color c = maskTex.GetPixel(x,y);	
					maskPixels[pixel] = (byte)(c[_pixel].r*255);
					maskPixels[pixel+1] = (byte)(c[_pixel].g*255);
					maskPixels[pixel+2] = (byte)(c[_pixel].b*255);
					maskPixels[pixel+3] = (byte)(c[_pixel].a*255);
					pixel += 4;
					_pixel++;
				}
			}
		}

		
		public void ReadClearingImage()
		{
			clearPixels = new byte[texWidth * texHeight * 4];
			
			// get our current texture into tex
			tex.SetPixels32(((Texture2D)GetComponent<Renderer>().material.GetTexture(targetTexture)).GetPixels32());
			tex.Apply(false);
			
			int pixel = 0;
			for (int y = 0; y < texHeight; y++) 
			{
				for (int x = 0; x < texWidth; x++) 
				{
					// TODO: use readpixels32
					Color c = tex.GetPixel(x,y);
					
					clearPixels[pixel] = (byte)(c.r*255);
					clearPixels[pixel+1] = (byte)(c.g*255);
					clearPixels[pixel+2] = (byte)(c.b*255);
					clearPixels[pixel+3] = (byte)(c.a*255);
					pixel += 4;
				}
			}
		}
		/// <summary>
		/// Creates the canvas based quad, and mesh collider, scales mesh to fit actial rect transform size of this object.
		/// </summary>  
		void CreateCanvasQuad()
		{
			// create mesh plane
			Mesh go_Mesh = GetComponent<MeshFilter>().mesh;
			//clear mesh data
			go_Mesh.Clear();
			//come arrays we will use
			Vector3 [] corners= new Vector3[4];
			Vector3 [] corners1= new Vector3[4];
			//get actual object scale based on based hiearchy up to root node
			Vector3 canvasScale=transform.localScale;
			Transform up=transform.parent;
			do{
				canvasScale.x*=up.localScale.x;
				canvasScale.y*=up.localScale.y;
				canvasScale.z*=up.localScale.z;
				up=up.parent;
			}while(up!=null);
			//tace current position in world space
			Vector3 canvasPosition=transform.position;
			//get 
			//Rect r= gameObject.GetComponent<RectTransform>().rect;
			//translate object to origin
			transform.position=Vector3.zero;
			//calculate scale factor
			canvasScale.x=1f/canvasScale.x; 
			canvasScale.y=1f/canvasScale.y;
			canvasScale.z=1f/canvasScale.z;
			//get actual world corners of RectTransform 
			gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
//			Debug.Log (corners[0]);
//			Debug.Log (corners[1]);
//			Debug.Log (corners[2]);
//			Debug.Log (corners[3]);
			//up/down scale each world corner 

/*izmena--------------			
  	 		for(int i=0;i<4;i++)
			{
			Vector3 newC=corners[i];
			newC.x*=(canvasScale.x);
			newC.y*=(canvasScale.y);
			newC.z*=(canvasScale.z);
			corners1[i]=newC;
			}
--------------*/
			//return object from origin to actual position
			transform.position=canvasPosition;

			//izmena-------------------
			Quaternion rotationQM = Quaternion.Euler(0, 0,  -transform.localRotation.eulerAngles.z);
			Vector3 scaleQM = canvasScale; 
			Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rotationQM , Vector3.one);
			//Matrix4x4 m2 = Matrix4x4.TRS(Vector3.zero, Quaternion.identity , scaleQM);
			for (int i = 0; i < 4; i++) 
			{
				corners1[i] = m.MultiplyPoint3x4(corners[i]);
				//corners1[i] = m2.MultiplyPoint3x4(corners1[i]);	 

				Vector3 newC=corners1[i];
				newC.x*=(canvasScale.x);
				newC.y*=(canvasScale.y);
				newC.z*=(canvasScale.z);
				corners1[i]=newC;
			}
			//---------------------

			//assign mesh vertices
			go_Mesh.vertices = new [] {
			 // bottom left
				corners1[0],
			 // top left
				corners1[1],
			 // top right
				corners1[2],
			 // bottom right
				corners1[3]
			};

			 


			//generate quad UV's
			go_Mesh.uv = new [] {new Vector2(0, 0), new Vector2(0, 1),new Vector2(1, 1), new Vector2(1, 0)};
			//quad gets made by using two triangles ( look some OpenGL basics for this)
			go_Mesh.triangles = new  [] {0, 1, 2, 0, 2, 3};
			//recalcualte mesh normals
			// TODO: add option for this
			go_Mesh.RecalculateNormals();
			//calculate mesh tangents
			// TODO: add option to calculate tangents
			go_Mesh.tangents = new [] {new Vector4(1.0f, 0.0f, 0.0f, -1.0f),new Vector4(1.0f, 0.0f, 0.0f, -1.0f),new Vector4(1.0f, 0.0f, 0.0f, -1.0f),new Vector4(1.0f, 0.0f, 0.0f, -1.0f)};
			
			
			// add mesh collider
			gameObject.AddComponent<MeshCollider>();
		}
		
		
		
		
		// compares if two values are below threshold
		public bool CompareThreshold(byte a, byte b)
		{
			//return Mathf.Abs(a-b)<=threshold;
			if (a<b) {a ^= b; b ^= a; a ^= b;} // http://lab.polygonal.de/?p=81
			return (a-b)<=paintThreshold;
		}

		/// <summary>
		/// Sets the mask texture mode drawing mode.
		/// </summary>
		public void SetMaskTextureMode()
		{
		
			useMaskImage = true;
			useLockArea = true;
			ReadMaskImage ();
			gameObject.GetComponent<Renderer>().material.SetTexture("_MaskTex", maskTex);
		}
		
		/// <summary>
		/// Setovanje the bitmap brusha.
		/// </summary>
		/// <returns><c>true</c>, if bitmap brush was set, <c>false</c> otherwise.</returns>
		/// <param name="brushTexture">Brush texture.</param>
		/// <param name="brushType">Brush type.</param>
		/// <param name="isAditiveBrush">If set to <c>true</c> is aditive brush.</param>
		/// <param name="brushCanDrawOnBlack">If set to <c>true</c> brush can draw on black.</param>
		/// <param name="brushColor">Brush color.</param>
		/// <param name="usesLockMasks">If set to <c>true</c> uses lock masks.</param>
		/// <param name="useBrushAlpha">If set to <c>true</c> use brush alpha.</param>
		/// <param name="brushPattern">Brush pattern.</param>
		public bool SetBitmapBrush(Texture2D brushTexture,BrushProperties brushType, bool isAditiveBrush,bool brushCanDrawOnBlack,Color brushColor, bool usesLockMasks,bool useBrushAlpha,Texture2D brushPattern )
		{
			customBrush=brushTexture;
			ReadCurrentCustomBrush();
			brushMode=brushType;
			useAdditiveColors=isAditiveBrush;
			canDrawOnBlack=brushCanDrawOnBlack;
			paintColor=brushColor;
			useLockArea=usesLockMasks;
			useMaskLayerOnly=usesLockMasks;
			useThreshold=usesLockMasks;
			useCustomBrushAlpha=useBrushAlpha;
			if(brushPattern!=null)
			{
			ReadCurrentCustomPattern(brushPattern);
			}
			isLinePaint=false;
			return true;
		}
		/// <summary>
		/// Setovanje vector brusha.
		/// </summary>
		/// <returns><c>true</c>, if vector brush was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type.</param>
		/// <param name="sizeX">Radijus kruga ili sirina pravougaonika.</param>
		/// <param name="sizeY">Visina pravougaonika</param>
		/// <param name="brushColor">Brush color.</param>
		/// <param name="pattern">Pattern.</param>
		/// <param name="isAditiveBrush">If set to <c>true</c> is aditive brush.</param>
		/// <param name="brushCanDrawOnBlack">If set to <c>true</c> brush can draw on black.</param>
		/// <param name="usesLockMasks">If set to <c>true</c> uses lock masks.</param>
		/// <param name="useBrushAlpha">If set to <c>true</c> use brush alpha.</param>
		public bool SetVectorBrush(VectorBrush type, int sizeX,int sizeY, Color brushColor, Texture2D pattern,bool isAditiveBrush,bool brushCanDrawOnBlack, bool usesLockMasks,bool useBrushAlpha)
		{
			vectorBrushType=type;
			if(pattern!=null)
			{
			//Debug.Log ("CIRCLE");
				drawMode=DrawMode.Default;
				ReadCurrentCustomPattern(pattern);
				brushMode=BrushProperties.Pattern;
			}
			else
			{
				drawMode=DrawMode.Default;
				paintColor=brushColor;
				brushMode=BrushProperties.Default;
			}
			useAdditiveColors=isAditiveBrush;
			canDrawOnBlack=brushCanDrawOnBlack;
			useCustomBrushAlpha=useBrushAlpha;
			useLockArea=usesLockMasks;
			brushSize=sizeX;
			
			customBrushHeight=sizeX;
			customBrushWidth=sizeY;
			isLinePaint=false;
			return true;
		}
		/// <summary>
		/// Setuje  Flood Fill brush (bucket).
		/// </summary>
		/// <returns><c>true</c>, if flood F ill brush was set, <c>false</c> otherwise.</returns>
		/// <param name="floodColor">Flood color.</param>
		/// <param name="usesLockMasks">If set to <c>true</c> uses lock masks.</param>
		public bool SetFloodFIllBrush(Color floodColor,bool usesLockMasks)
		{
			isLinePaint=false;
		drawMode=DrawMode.FloodFill;
		paintColor=floodColor;
		useLockArea=usesLockMasks;
		return true;
		}
		/// <summary>
		/// Setuje teksturu po kojoj se crta.
		/// </summary>
		/// <returns><c>true</c>, if drawing texture was set, <c>false</c> otherwise.</returns>
		/// <param name="texture">Texture.</param>
		public bool SetDrawingTexture (Texture2D texture)
		{
		texWidth=texture.width;
		texHeight=texture.height;
		//support for NPOT
		tex= new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);
		pixels= new byte[texture.width*texture.height*4];
		
		//
		Color [] texturePixels=texture.GetPixels();
		pixels= new byte[texture.width*texture.height*4];
		int pix=0;
			for(int i=0;i<texture.height;i++)
				for(int j=0;j<texture.width;j++)
			{
					pixels[pix]=(byte)(texturePixels[i*texture.width+j].r*255);//R
					pixels[pix+1]=(byte)(texturePixels[i*texture.width+j].g*255);//G
					pixels[pix+2]=(byte)(texturePixels[i*texture.width+j].b*255);//B
					pixels[pix+3]=(byte)(texturePixels[i*texture.width+j].a*255);//A

				//----novo
					if(pixels[pix+3]>0) 
					{
						pixels[pix+3]=(byte)255;
					}
					else
					{
						pixels[pix]=(byte)0;
						pixels[pix+1]=(byte)0;
						pixels[pix+2]=(byte)0;
						pixels[pix+3]=(byte)0;
					}
				//----novo
				pix+=4;

			}
		tex.LoadRawTextureData(pixels);
		tex.Apply(false);
		GetComponent<Renderer>().material.SetTexture(targetTexture, tex);
	
		if (createCanvasMesh)
			{
				gameObject.GetComponent<MeshRenderer>().enabled=false;;
				gameObject.GetComponent<RawImage>().texture=tex;
				gameObject.GetComponent<RawImage>().enabled=true;
				
			}
		
		if(undoEnabled)
		{
			undoPixels= new byte[pixels.Length];
			System.Array.Copy(pixels,undoPixels,pixels.Length);
		}
		return true;
		}
		/// <summary>
		/// Setuje teksturu maske.
		/// </summary>
		/// <returns><c>true</c>, if drawing mask was set, <c>false</c> otherwise.</returns>
		/// <param name="texture">Texture.</param>
		public bool SetDrawingMask(Texture2D texture)		
		{
			Color [] texturePixels=texture.GetPixels();
			maskPixels= new byte[texture.width*texture.height*4];
			int pix=0;
			for(int i=0;i<texture.height;i++)
				for(int j=0;j<texture.width;j++)
			{
				//if(texturePixels[i*texture.width+j].r>0)
				
				maskPixels[pix]=(byte)(texturePixels[i*texture.width+j].r*255);//R
				maskPixels[pix+1]=(byte)(texturePixels[i*texture.width+j].g*255);//G
				maskPixels[pix+2]=(byte)(texturePixels[i*texture.width+j].b*255);//B
				maskPixels[pix+3]=(byte)(texturePixels[i*texture.width+j].a*255);//A
				//----novo
				if(maskPixels[pix+3]>0) 
				{
					maskPixels[pix+3]=(byte)255;
				}
				else
				{
					maskPixels[pix]=(byte)0;
					maskPixels[pix+1]=(byte)0;
					maskPixels[pix+2]=(byte)0;
					maskPixels[pix+3]=(byte)0;
				}
				//----novo
				pix+=4;
			}
		return true;
		}
	
	
	/// <summary>
	/// Sets the line brush.
	/// </summary>
	/// <param name="brushSize">Brush size.</param>
	/// <param name="lineEdgeSize">Line edge size.</param>
	/// <param name="color">Color.</param>
	/// <param name="lineCorePattern">Line core pattern.</param>
	/// <param name="lineEdgePattern">Line edge pattern.</param> 
	//FIXME dodaj da ostali brushevi deselektuje linePaint
		public void SetLineBrush(int sizeOfBrush,int edgeSize,Color color,Texture2D lineCorePattern, Texture2D lineEdgePattern)
		{
			isLinePaint=true;
			brushSize=sizeOfBrush;
			paintColor=color;
			lineEdgeSize=edgeSize;
			useAdditiveColors=false;
			useLockArea=false;
			useMaskImage=false;
			
			if (lineCorePattern!=null)
			{
				isPatternLine=true;
				ReadCurrentCustomPattern(lineCorePattern);
				//ISPA
			}
			else isPatternLine=false;
			if(lineEdgeSize >0 && lineEdgePattern==null)
			{
				Debug.LogError("Line edge set, but no pattern for line edge assigned!");
			}
			else if (lineEdgePattern!=null)
			{
				customBrush=lineEdgePattern;
				ReadCurrentCustomBrush();
			}
			customBrushWidth=sizeOfBrush;
			customBrushHeight=sizeOfBrush;
		}
	

	}

	
}