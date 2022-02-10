 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunk {

    public chunkCoord coord;

    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;


    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    public byte[,,] voxelMap = new byte[voxel.ChunkWidth, voxel.ChunkHeight, voxel.ChunkWidth];
    World world;

    private bool _isActive;

    public bool isVoxelMapPopulated = false;


    //asd

    public chunk (chunkCoord _coord, World _world, bool generateOnLoad) {
        
        coord = _coord;
        world = _world;
       isActive = true;
        if(generateOnLoad)
            Init();

    }

    public void Init() {


        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();



        meshRenderer.material = world.material;

        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * voxel.ChunkWidth, 0f, coord.z * voxel.ChunkWidth);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;


        PopulateVoxelMap();
        Chunky();
        



    }




    void Chunky() {
        clearMeshData();

        for (int y = 0; y < voxel.ChunkHeight; y++)
        {
            for (int x = 0; x < voxel.ChunkWidth; x++)
            {
                for (int z = 0; z < voxel.ChunkWidth; z++)
                {
                    // dataToVoxelChunk(transform.position + transform.up * y + transform.right * x + transform.forward * z);

                    if(world.blocktypes[voxelMap[x,y,z]].isSolid)

                    dataToVoxelChunk(new Vector3(x, y, z));
                }
            }
        }
        createMesh();
    }


    public bool isActive {
        get { return _isActive; }
        set
        {

            _isActive = value;
            if (chunkObject != null)
                chunkObject.SetActive(value);



        }
    }

    public Vector3 position {
        get { return chunkObject.transform.position;  }
    }

    bool isVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > voxel.ChunkWidth - 1 || y < 0 || y > voxel.ChunkHeight - 1 || z < 0 || z > voxel.ChunkWidth - 1)
            return false;
        else
            return true;
    }

    public void editVoxel(Vector3 pos, byte newID) {

        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        voxelMap[xCheck, yCheck, zCheck] = newID;

        updateSurrounding(xCheck, yCheck, zCheck);

        Chunky();

    }

    void updateSurrounding(int x, int y, int z) {
        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int p = 0; p < 6; p++) {

            Vector3 currenrVoxel = thisVoxel + voxel.faceChecks[p];

            if (!isVoxelInChunk((int)currenrVoxel.x, (int)currenrVoxel.y, (int)currenrVoxel.z)) {

                world.getChunkFromVector3(currenrVoxel + position).Chunky();
            
            }
        
        }

    }



    bool CheckVoxel(Vector3 pos) {

        int x = Mathf.FloorToInt(pos.x);

        int y = Mathf.FloorToInt(pos.y);

        int z = Mathf.FloorToInt(pos.z);

        if (!isVoxelInChunk(x, y, z))
            return world.CheckForVoxel(pos + position);

        return world.blocktypes[voxelMap[x, y, z]].isSolid;
    }

    public byte GetVoxelFromGlobalVector3(Vector3 pos) {

        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);
    
        return voxelMap[xCheck, yCheck, zCheck];
    
    }

    void PopulateVoxelMap() {

        for (int y = 0; y < voxel.ChunkHeight; y++)
        {
            for (int x = 0; x < voxel.ChunkWidth; x++)
            {
                for (int z = 0; z < voxel.ChunkWidth; z++)
                {
                    

                    voxelMap[x,y,z] = world.GetVoxel(new Vector3(x, y, z) + position);


                }
            }
        }
        isVoxelMapPopulated = true;

    }

    void clearMeshData() {

        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();    
    
    }

    void dataToVoxelChunk(Vector3 pos) {
        //info voxel 
        for (int p = 0; p < 6; p++)
        {
            if (!CheckVoxel(pos+voxel.faceChecks[p])) {
                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
                vertices.Add(pos + voxel.voxelVerts[voxel.voxelTris[p, 0]]);
                vertices.Add(pos + voxel.voxelVerts[voxel.voxelTris[p, 1]]);
                vertices.Add(pos + voxel.voxelVerts[voxel.voxelTris[p, 2]]);
                vertices.Add(pos + voxel.voxelVerts[voxel.voxelTris[p, 3]]);
                
                AddTexture(world.blocktypes[blockID].GetTextureID(p));
                
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);

                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);



                vertexIndex += 4;
            }
        }
    }
    void createMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void AddTexture(int textureID) {

        float y = textureID / voxel.TextureAtlasSizeInBlock;
        float x = textureID - (y * voxel.TextureAtlasSizeInBlock);

        x *= voxel.NormalizedTextureSize;
        y *= voxel.NormalizedTextureSize;

        y = 1f - y - voxel.NormalizedTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + voxel.NormalizedTextureSize));
        uvs.Add(new Vector2(x + voxel.NormalizedTextureSize, y));
        uvs.Add(new Vector2(x + voxel.NormalizedTextureSize, y + voxel.NormalizedTextureSize));
    }
}

public class chunkCoord {
    public int x;
    public int z;

    public chunkCoord() { 
    
        x = 0;
        z = 0;

    }

    public chunkCoord(int _x, int _z) { 
    x = _x;
    z = _z;
    }

    public chunkCoord(Vector3 pos) {
        int xCheck = Mathf.FloorToInt(pos.x);

        int zCheck = Mathf.FloorToInt(pos.z);

        x = xCheck / voxel.ChunkWidth;

        z = zCheck / voxel.ChunkWidth;
    }


    public bool Equals(chunkCoord other) {

        if (other == null) return false;
        else if (other.x == x && other.z == z) return true;
        else return false;

    }

}
