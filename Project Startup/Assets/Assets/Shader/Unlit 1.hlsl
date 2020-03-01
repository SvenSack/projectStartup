void LWRPLightingFunction_float (float3 WorldPos, out float3 Direction, out float ShadowAttenuation, out float3 Color)
{
   #ifdef LIGHTWEIGHT_LIGHTING_INCLUDED
   
      //Actual light data from the pipeline
      Light light = GetMainLight(GetShadowCoord(GetVertexPositionInputs(WorldPos)));
      Direction = light.direction;
	  ShadowAttenuation = light.shadowAttenuation;
      Color = light.color;
      
      
   #else
   
      //Hardcoded data, used for the preview shader inside the graph
      //where light functions are not available
      Direction = float3(-0.5, 0.5, -0.5);
	  ShadowAttenuation = 0.4;
      Color = float3(1, 1, 1);
      
      
   #endif
}