Shader "AMP/PaintCullingFront" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_MaskTex ("Mask (RGBA)", 2D) = "white" {}
	
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite On //Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {
		//Blend SrcAlpha OneMinusSrcAlpha
        
         Cull Front
			SetTexture [_MainTex] {
				combine texture * primary
			}
			
//			SetTexture [_MaskTex] 
//			{ 
//				combine texture lerp (texture) previous, previous+texture
//			}
			
		}
	}
}
}
