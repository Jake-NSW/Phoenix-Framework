using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PileTexturer))]
public class PileTexturerEditor : Editor {

	/*const float GAUSSIAN_MULTIPLIER = 1f/273;

	static readonly float[,] GAUSSIAN = {
		{ 1,  4,  7,  4,  1},
		{ 4, 16, 26, 16,  4},
		{ 7, 26, 41, 26,  7},
		{ 4, 16, 26, 16,  4},
		{ 1,  4,  7,  4,  1},
	};

	const float CONVEXITY_MULTIPLIER = 0.00001f;

	static readonly float[,] CONVEXITY = {
		{ -1,  -4,  -7,  -4,  -1},
		{ -4, -16, -26, -16,  -4},
		{ -7, -26,6552, -26,  -7},
		{ -4, -16, -26, -16,  -4},
		{ -1,  -4,  -7,  -4,  -1},
	};*/

	PileTexturer pile;

	void OnEnable() {
		this.pile = (PileTexturer) this.target;
	}

	void OnSceneGUI() {
		Handles.matrix = this.pile.transform.localToWorldMatrix;
		Handles.DrawWireCube(Vector3.zero, Vector3.one);
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		if(GUILayout.Button("Apply Texture!")) {
			ApplyTexture();
		}
	}

	void ApplyTexture() {
		Terrain[] terrains = FindObjectsOfType<Terrain>();

		Bounds thisBounds = new Bounds(this.pile.transform.position, this.pile.transform.localScale);

		foreach(Terrain terrain in terrains) {
			Bounds bounds = terrain.terrainData.bounds;
			bounds.center += terrain.transform.position;

			if(bounds.Intersects(thisBounds)) {
				//ClearTexture(terrain);
				ApplyTextureToTerrain(terrain);
			}
		}
	}

	void ClearTexture(Terrain terrain) {
		TerrainData data = terrain.terrainData;
		float[,,] alphaMap = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);

		for(int x = 0; x < data.alphamapWidth; x++) {
			for(int y = 0; y < data.alphamapHeight; y++) {
				for(int i = 0; i < data.alphamapLayers; i++) {
					alphaMap[x,y,i] = i == 0 ? 1f : 0f;
				}
			}
		}

		data.SetAlphamaps(0, 0, alphaMap);
	}

	void ApplyTextureToTerrain(Terrain terrain) {

		Debug.Log("Applying texture to "+terrain.gameObject.name);

		TerrainData data = terrain.terrainData;

		Vector3 normalizedMinPosition = ((this.pile.transform.position-this.pile.transform.localScale/2f) - terrain.transform.position);
		Vector3 normalizedMaxPosition = ((this.pile.transform.position+this.pile.transform.localScale/2f) - terrain.transform.position);
		Vector3 inverseScale = new Vector3(1f/data.size.x, 1f/data.size.y, 1f/data.size.z);

		normalizedMinPosition.Scale(inverseScale);
		normalizedMaxPosition.Scale(inverseScale);

		//Debug.Log("min: "+normalizedMinPosition+", max: "+normalizedMaxPosition);

		normalizedMinPosition = Vector3.Max(Vector3.zero, normalizedMinPosition);
		normalizedMaxPosition = Vector3.Min(Vector3.one, normalizedMaxPosition);

		Vector3 normalizedSize = normalizedMaxPosition-normalizedMinPosition;

		int minX = Mathf.RoundToInt(normalizedMinPosition.x*data.alphamapWidth);
		int minY = Mathf.RoundToInt(normalizedMinPosition.z*data.alphamapHeight);
		int width = Mathf.RoundToInt(normalizedSize.x*data.alphamapWidth);
		int height = Mathf.RoundToInt(normalizedSize.z*data.alphamapHeight);

		int detail_minX = Mathf.RoundToInt(normalizedMinPosition.x*data.detailWidth);
		int detail_minY = Mathf.RoundToInt(normalizedMinPosition.z*data.detailHeight);
		int detail_width = Mathf.RoundToInt(normalizedSize.x*data.detailWidth);
		int detail_height = Mathf.RoundToInt(normalizedSize.z*data.detailHeight);


		float[,,] alphaMap = data.GetAlphamaps(minX, minY, width, height);
		//float[,] sumMap = new float[height,width];

		Vector2 sampleDistance = new Vector2((normalizedSize.x/width), (normalizedSize.z/height));

		/*bool calculateConvexity = false;
		foreach(PileTexturer.Layer layers in this.pile.layers) {
			if(layers.type == PileTexturer.Layer.Type.Concave || layers.type == PileTexturer.Layer.Type.Convex) {
				calculateConvexity = true;
				break;
			}
		}*/

		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {

				float normalizedX = normalizedMinPosition.x + x*sampleDistance.x;
				float normalizedY = normalizedMinPosition.z + y*sampleDistance.y;

				/*float convexity = 0f;
				if(calculateConvexity) {
					convexity = CONVEXITY_MULTIPLIER*Kernel(CONVEXITY, normalizedX, normalizedY, sampleDistance.x, sampleDistance.y, data);
				}*/

				for(int layer = 0; layer < data.alphamapLayers; layer++) {

					float value = alphaMap[y,x,layer];

					if(layer < this.pile.layers.Length) {
						PileTexturer.Layer layerStyle = this.pile.layers[layer];

						switch(layerStyle.type) {
						case PileTexturer.Layer.Type.Set:
							value = layerStyle.outputGain;
							break;
						case PileTexturer.Layer.Type.Slope:
							value = layerStyle.curve.Evaluate(layerStyle.inputGain*data.GetSteepness(normalizedX, normalizedY))*layerStyle.outputGain;
							break;
						case PileTexturer.Layer.Type.Height:
							value = layerStyle.curve.Evaluate(layerStyle.inputGain*data.GetInterpolatedHeight(normalizedX, normalizedY))*layerStyle.outputGain;
							break;
						/*case PileTexturer.Layer.Type.Convex:
							value = layerStyle.curve.Evaluate(layerStyle.inputGain*convexity)*layerStyle.outputGain;
							break;
						case PileTexturer.Layer.Type.Concave:
							value = layerStyle.curve.Evaluate(layerStyle.inputGain*(1f-convexity))*layerStyle.outputGain;
							break;*/
						default:
							// Keep value
							break;
						}

						Random.InitState(layerStyle.noiseSeed);
						float noiseOffsetX = Random.Range(-1000f, 1000f);
						float noiseOffsetY = Random.Range(-1000f, 1000f);

						value *= 1f + layerStyle.outputNoise*2f*(Mathf.PerlinNoise(noiseOffsetX + layerStyle.noiseFrequency*normalizedX, noiseOffsetY + layerStyle.noiseFrequency*normalizedY)-0.5f);
					}

					value = Mathf.Clamp01(value);

					for(int prevLayers = 0; prevLayers < layer; prevLayers++) {
						alphaMap[y,x,prevLayers] = Mathf.Clamp01(alphaMap[y,x,prevLayers]-value);
					}

					alphaMap[y,x,layer] = value;
				}

				float sum = 0f;
				for(int layer = 0; layer < data.alphamapLayers; layer++) {
					sum += alphaMap[y,x,layer];
				}

				sum = Mathf.Max(sum, 0.01f);

				for(int layer = 0; layer < data.alphamapLayers; layer++) {
					alphaMap[y,x,layer] /= sum;
				}


			}
		}

		data.SetAlphamaps(minX, minY, alphaMap);


		int nDetailLayers = data.detailPrototypes.Length;
		for(int detailLayer = 0; detailLayer < nDetailLayers; detailLayer++) {

			PileTexturer.DetailLayer detailLayerStyle = this.pile.detailLayers[detailLayer];

			int[,] detailMap = data.GetDetailLayer(detail_minX, detail_minY, detail_width, detail_height, detailLayer);
			int alphaLayer = detailLayerStyle.layerIndex;

			for(int x = 0; x < detail_width; x++) {
				for(int y = 0; y < detail_height; y++) {

					int xAlpha = Mathf.Clamp((int) (x*(((float)width)/detail_width)), 0, width);
					int yAlpha = Mathf.Clamp((int) (y*(((float)width)/detail_height)), 0, height);

					float alphaValue = alphaMap[xAlpha, yAlpha, alphaLayer];

					detailMap[x,y] = Mathf.RoundToInt(detailLayerStyle.densityCurve.Evaluate(alphaValue)*detailLayerStyle.densityMultiplier);

				}
			}

			data.SetDetailLayer(detail_minX, detail_minY, detailLayer, detailMap);
		}


	}

	float Kernel(float[,] kernel, float centerX, float centerY, float deltaX, float deltaY, TerrainData data) {
		int offsetX = -kernel.GetLength(0)/2;
		int offsetY = -kernel.GetLength(1)/2;

		float sum = 0f;

		for(int x = 0; x < kernel.GetLength(0); x++) {
			for(int y = 0; y < kernel.GetLength(1); y++) {
				float normalizedX = centerX+(x-offsetX)*deltaX;
				float normalizedY = centerY+(y-offsetY)*deltaY;

				sum += data.GetInterpolatedHeight(normalizedX, normalizedY)*kernel[x,y];
			}
		}

		return sum;
	}
}
