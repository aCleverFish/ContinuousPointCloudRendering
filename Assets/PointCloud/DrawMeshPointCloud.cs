using UnityEngine;
using System.Collections;
using System.IO;

public class DrawMeshPointCloud : MonoBehaviour
{
    ArrayList list = new ArrayList();

    void Start()
    {
        // 1. 读取数据
        list = ReadFile();

        // 2. 渲染
        CreateMesh();
    }

    ArrayList ReadFile()
    {
        // 提前将点云存成csv文件放在Assert/StreamingAssets文件夹下，文本的每行代表一个点，由点的x，y，z
        //csv文件存储数据，用逗号分隔，比较容易读取处理
        string path = (Application.streamingAssetsPath + "/" + "elephant.csv");
        FileInfo fInfo = new FileInfo(path);

        string s = "";
        StreamReader r;
        ArrayList vecList = new ArrayList();

        if (fInfo.Exists)
        {
            r = new StreamReader(path);
        }
        else
        {
            Debug.Log("文件不存在");
            return null;
        }
        // 点云数据存入队列
        while ((s = r.ReadLine()) != null)
        {
            string[] words = s.Split(","[0]);

            Vector3 xyz = new Vector3(float.Parse(words[0]), -float.Parse(words[1]), float.Parse(words[2])) * 10;
            vecList.Add(xyz);
        }

        return vecList;
    }



    void CreateMesh()
    {
        int num = list.Count;

        GameObject pointObj = new GameObject();
        pointObj.name = "new";
        //处理大象朝向
        pointObj.transform.rotation = Quaternion.Euler(new Vector3(180, -180, 0));
        pointObj.AddComponent<MeshFilter>();
        pointObj.AddComponent<MeshRenderer>();
        Mesh meshNeed = new Mesh();
        Material mat = new Material(Shader.Find("Custom/VertexColor"));
        pointObj.GetComponent<MeshFilter>().mesh = meshNeed;
        pointObj.GetComponent<MeshRenderer>().material = mat;

        Vector3[] points = new Vector3[num];
        Color[] colors = new Color[num];
        int[] indecies = new int[num];
        for (int i = 0; i < num; ++i)
        {
            points[i] = (Vector3)list[i];
            indecies[i] = i;
            colors[i] = Color.white;
        }

        meshNeed.vertices = points;
        meshNeed.colors = colors;
        meshNeed.SetIndices(indecies, MeshTopology.Points, 0);

    }
}