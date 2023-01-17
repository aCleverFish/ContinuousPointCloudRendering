using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestDrawParticleSystem : MonoBehaviour
{
    ParticleSystem particleSystem;
    int pointCount;
    PointInfo pointInfo = new PointInfo();

    class PointInfo
    {
        public ArrayList pointPositionList = new ArrayList();
        public ArrayList pointColorList = new ArrayList();
    }

    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        //1.读取数据
        pointInfo = ReadFile();

        //2.粒子显示
        DrawPointCloud(pointInfo);
    }

    PointInfo ReadFile()
    {
        // 提前将点云存成csv文件放在Assert/StreamAssets文件夹下，文本的每行代表一个点，分别为x，y，z
        // csv文件存储数据，用逗号分割，比较容易处理

        string path = (Application.streamingAssetsPath) + "/" + "testA.csv";
        FileInfo fileInfo = new FileInfo(path);

        string s = "";
        StreamReader r;
        PointInfo vertextList = new PointInfo();

        if (fileInfo.Exists)
        {
            r = new StreamReader(path);
        }
        else
        {
            Debug.Log("文件不存在");
            return null;
        }

        while ((s = r.ReadLine()) != null)
        {
            string[] pointInfo = s.Split(" "[0]);
            Vector3 xyz = new Vector3(float.Parse(pointInfo[0]), float.Parse(pointInfo[1]), float.Parse(pointInfo[2]));

            Color32 color = new Color32(byte.Parse(pointInfo[3]), byte.Parse(pointInfo[4]), byte.Parse(pointInfo[5]), 255);

            vertextList.pointPositionList.Add(xyz);
            vertextList.pointColorList.Add(color);
        }

        return vertextList;
    }


    ParticleSystem.Particle[] allParticles;

    void DrawPointCloud(PointInfo drawList)
    {
        var main = particleSystem.main;
        main.startSpeed = 0.0f;
        main.startLifetime = 1000.0f;

        var pointCount = drawList.pointPositionList.Count;
        allParticles = new ParticleSystem.Particle[pointCount];
        main.maxParticles = pointCount;
        particleSystem.Emit(pointCount);
        particleSystem.GetParticles(allParticles);

        //给例子赋值属性
        for (int i = 0; i < pointCount; i++)
        {
            allParticles[i].position = (Vector3)drawList.pointPositionList[i];    // 设置每个点的位置
            allParticles[i].startColor = (Color32)drawList.pointColorList[i];     // 设置每个点的rgba
            allParticles[i].startSize = 0.02f;                  // 设置每个点的大小
        }

        particleSystem.SetParticles(allParticles,pointCount);   //点云载入粒子系统
    }
}