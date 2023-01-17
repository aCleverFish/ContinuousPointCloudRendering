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

        //1.��ȡ����
        pointInfo = ReadFile();

        //2.������ʾ
        DrawPointCloud(pointInfo);
    }

    PointInfo ReadFile()
    {
        // ��ǰ�����ƴ��csv�ļ�����Assert/StreamAssets�ļ����£��ı���ÿ�д���һ���㣬�ֱ�Ϊx��y��z
        // csv�ļ��洢���ݣ��ö��ŷָ�Ƚ����״���

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
            Debug.Log("�ļ�������");
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

        //�����Ӹ�ֵ����
        for (int i = 0; i < pointCount; i++)
        {
            allParticles[i].position = (Vector3)drawList.pointPositionList[i];    // ����ÿ�����λ��
            allParticles[i].startColor = (Color32)drawList.pointColorList[i];     // ����ÿ�����rgba
            allParticles[i].startSize = 0.02f;                  // ����ÿ����Ĵ�С
        }

        particleSystem.SetParticles(allParticles,pointCount);   //������������ϵͳ
    }
}