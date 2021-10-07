using UnityEngine;
using System.Collections;

public class MeshData
{
	private Vector3[] vertices;
	int[] triangles;
	Vector2[] uvs;

	Vector3[] borderVertices;
	int[] borderTriangles;

	int triangleIndex;
	int borderTriangleIndex;
	bool useFlatShading;

	public MeshData(int verticesPerLine, float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, bool useFlatShading)
	{
		
		AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
		int borderedSize = heightMap.GetLength(0);
		int meshSize = borderedSize - 2;
		float topLeftX = (meshSize - 1) / -2f;
		float topLeftZ = (meshSize - 1) / 2f;

		this.useFlatShading = useFlatShading;
		vertices = new Vector3[verticesPerLine * verticesPerLine];
		uvs = new Vector2[verticesPerLine * verticesPerLine];
		triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

		borderVertices = new Vector3[verticesPerLine * 4 + 4];
		borderTriangles = new int[24 * verticesPerLine];

		int[,] vertexIndicesMap = new int[borderedSize, borderedSize];
		// Stores the number of vertices that make up the mesh
		int meshVertexIndex = 0;
		// Stores the number of vertices found in the border, negative increment
		int borderVertexIndex = -1;

		// Updates index depending whether the vertex makes up the border or not
		for (int y = 0; y < borderedSize; y++)
		{
			for (int x = 0; x < borderedSize; x++)
			{
				bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

				if (isBorderVertex)
				{
					vertexIndicesMap[x, y] = borderVertexIndex;
					borderVertexIndex--;
				}
				else
				{
					vertexIndicesMap[x, y] = meshVertexIndex;
					meshVertexIndex++;
				}
			}
		}

		for (int y = 0; y < borderedSize; y++)
		{
			for (int x = 0; x < borderedSize; x++)
			{
				int vertexIndex = vertexIndicesMap[x, y];
				Vector2 percent = new Vector2((x - 1) / (float)meshSize, (y - 1) / (float)meshSize);

				// finds vertex position in game space
				float vertexHeight = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;
				Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSize, vertexHeight, topLeftZ - percent.y * meshSize);

				AddVertex(vertexPosition, percent, vertexIndex);

				if (x < borderedSize - 1 && y < borderedSize - 1)
				{
					// creates the 2 triangles based on vertex
					int a = vertexIndicesMap[x, y];
					int b = vertexIndicesMap[x + 1, y];
					int c = vertexIndicesMap[x, y + 1];
					int d = vertexIndicesMap[x + 1, y + 1];
					AddTriangle(a, d, c);
					AddTriangle(d, a, b);

				}
				vertexIndex++;
			}
		}

		ProcessMesh();

	}
	//Constructor chaining
	public MeshData(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, bool useFlatShading)
		: this(heightMap.GetLength(0), heightMap, heightMultiplier, _heightCurve, useFlatShading) { }


	public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
	{
		if(vertexIndex < 0)
		{
			// vertex is found in border and added to border array
			borderVertices[-vertexIndex - 1] = vertexPosition;
		}
		else
		{
			// vertex is found in mesh and added to vertices array
			vertices[vertexIndex] = vertexPosition;
			uvs[vertexIndex] = uv;
		}
	}

	public void AddTriangle(int a, int b, int c)
	{
		// a,b and c represent an index in the vertices array

		if (a < 0 || b < 0 || c < 0)
		{
			borderTriangles[borderTriangleIndex] = a;
			borderTriangles[borderTriangleIndex + 1] = b;
			borderTriangles[borderTriangleIndex + 2] = c;
			borderTriangleIndex += 3;
		}
		else
		{
			triangles[triangleIndex] = a;
			triangles[triangleIndex + 1] = b;
			triangles[triangleIndex + 2] = c;
			triangleIndex += 3;
		}
	}
	Vector3[] CalculateNormals()
	{ 
		// manual normal calculation to account for border
		Vector3[] vertexNormals = new Vector3[vertices.Length];
		int triangleCount = triangles.Length / 3;
		for (int i = 0; i < triangleCount; i++)
		{
			// finds normals for all triangles in mesh
			int normalTriangleIndex = i * 3;
			int vertexIndexA = triangles[normalTriangleIndex];
			int vertexIndexB = triangles[normalTriangleIndex + 1];
			int vertexIndexC = triangles[normalTriangleIndex + 2];

			// each triangle has a normal based on the average normal of its vertices
			// each vertex has a normal based on all of the triangles that it is found in
			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			vertexNormals[vertexIndexA] += triangleNormal;
			vertexNormals[vertexIndexB] += triangleNormal;
			vertexNormals[vertexIndexC] += triangleNormal;
		}

		int borderTriangleCount = borderTriangles.Length / 3;
		for (int i = 0; i < borderTriangleCount; i++)
		{
			//finds normals for all triangles that have a vertex in the border
			int normalTriangleIndex = i * 3;
			int vertexIndexA = borderTriangles[normalTriangleIndex];
			int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
			int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

			// checks which vertices are found in the border, applys normals to all those that aren't
			Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			if(vertexIndexA > 0)
			{
				vertexNormals[vertexIndexA] += triangleNormal;
			}
			if(vertexIndexB > 0)
			{
				vertexNormals[vertexIndexB] += triangleNormal;
			}
			if(vertexIndexC > 0)
			{
				vertexNormals[vertexIndexC] += triangleNormal;
			}
		}

		for (int i = 0; i < vertexNormals.Length; i++)
		{
			// normalises normals as sum may exceed 1
			vertexNormals[i].Normalize();
		}
		return vertexNormals;
	}

	void FlatShading()
	{
		Vector3[] flatShadedVertices = new Vector3[triangles.Length];
		Vector2[] flatShadedUvs = new Vector2[triangles.Length];

		for (int i = 0; i < triangles.Length; i++)
		{
			flatShadedVertices[i] = vertices[triangles[i]];
			flatShadedUvs[i] = uvs[triangles[i]];
			triangles[i] = i;
		}
		vertices = flatShadedVertices;
		uvs = flatShadedUvs;
	}

	public void ProcessMesh()
	{
		// normal calculation depends on lighting method
		if(useFlatShading)
		{
			FlatShading();
		}
		else
		{
			CalculateNormals();
		}
	}

	Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
	{
		// finds triangle normal from the vertices that make it up
		Vector3 vertexA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
		Vector3 vertexB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
		Vector3 vertexC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

		Vector2 sideAB = vertexB - vertexA;
		Vector3 sideAC = vertexC - vertexA;
		// uses cross-product to find perpendicular vector
		return Vector3.Cross(sideAB, sideAC).normalized;
	}

	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		if(useFlatShading)
		{
			mesh.RecalculateNormals();
		}
		else
		{
			mesh.normals = CalculateNormals();
		}
		return mesh;
	}

}

