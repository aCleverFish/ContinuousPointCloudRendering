using UnityEngine;
using System.Collections;
using System.IO;

public class DrawParticlePoint : MonoBehaviour {

    ParticleSystem particleSystem; 
    int pointCount; 

    ArrayList list = new ArrayList();

    // Use this for initialization
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        list = ReadFile();

        DrawPointCloud(list);
       
    }

    ArrayList ReadFile()
    {
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

        while ((s = r.ReadLine()) != null)
        {
            string[] words = s.Split(","[0]);

            Vector3 xyz = new Vector3(float.Parse(words[0]), -float.Parse(words[1]), float.Parse(words[2])) * 10;
            vecList.Add(xyz);
        }

        return vecList;
    }

    ParticleSystem.Particle[] allParticles;         
    void DrawPointCloud(ArrayList drawList)
    {
        var main = particleSystem.main;
        main.startSpeed = 0.0f; 
        main.startLifetime = 1000.0f;

        var pointCount = drawList.Count;
        allParticles = new ParticleSystem.Particle[pointCount];
        main.maxParticles = pointCount;
        particleSystem.Emit(pointCount);
        particleSystem.GetParticles(allParticles);

        for (int i = 0; i < pointCount; i++)
        {
            allParticles[i].position = (Vector3)drawList[i]; 
            allParticles[i].startColor = Color.yellow; 
            allParticles[i].startSize = 0.02f;                     
        }

        particleSystem.SetParticles(allParticles, pointCount);
    }
}
