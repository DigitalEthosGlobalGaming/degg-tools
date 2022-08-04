﻿using Sandbox;
using Tools;

namespace DeggTools
{
	public class ModelTemplate : IBaseTemplate
	{
		public string GetContent()
		{

			return @"
// THIS FILE IS AUTO-GENERATED

Layer0
{
	shader ""complex.vfx""

	//---- Translucent ----
	F_TRANSLUCENT 1

	//---- Ambient Occlusion ----
	g_flAmbientOcclusionDirectDiffuse ""0.000""
	g_flAmbientOcclusionDirectSpecular ""0.000""
	TextureAmbientOcclusion ""materials/default/default_ao.tga""

	//---- Color ----
	g_flModelTintAmount ""1.000""
	g_vColorTint ""[1.000000 1.000000 1.000000 0.000000]""
	TextureColor ""__texture_color__""

	//---- Fade ----
	g_flFadeExponent ""1.000""

	//---- Fog ----
	g_bFogEnabled ""1""

	//---- Lighting ----
	g_flDirectionalLightmapMinZ ""0.050""
	g_flDirectionalLightmapStrength ""1.000""

	//---- Metalness ----
	g_flMetalness ""0.000""

	//---- Normal ----
	TextureNormal ""[0.501961 0.501961 1.000000 0.000000]""

	//---- Roughness ----
	TextureRoughness ""materials/default/default_rough.tga""

	//---- Texture Coordinates ----
	g_nScaleTexCoordUByModelScaleAxis ""0""
	g_nScaleTexCoordVByModelScaleAxis ""0""
	g_vTexCoordOffset ""[0.000 0.000]""
	g_vTexCoordScale ""[1.000 1.000]""
	g_vTexCoordScrollSpeed ""[0.000 0.000]""

	//---- Translucent ----
	g_flOpacityScale ""1.000""
	TextureTranslucency ""__texture_translucency__""
}
			";
		}
	}

}
