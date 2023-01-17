using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ContinueDrawPointCloud : MonoBehaviour
{
    ParticleSystem particleSystem;
    PointInfo pointInfo = new PointInfo();
    ParticleSystem.Particle[] allParticles;
    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

    private IEnumerator coroutine;

    class PointInfo
    {
        public ArrayList pointPositionList = new ArrayList();
        public ArrayList pointColorList = new ArrayList();
    }

    // Start is called before the first frame update
    void Start()
    {
        /** 
         *  ①建立连接 ②接收数据并存储
         *  目前采取将所有数据暂时存储本地的实现方式,循环读取全部文件
         *  读取完一个文件渲染完成以后再读取下一个文件
         **/
        particleSystem = GetComponent<ParticleSystem>();

        print("Starting " + Time.time);

        coroutine = ShowPointCloud(particleSystem, 0.5f);
        StartCoroutine(coroutine);

        print("Before WaitAndPrint Finishes " + Time.time);

    }
    /// <summary>
    /// 读取一个文件文件
    /// </summary>
    /// <param name="path">读取文件的路径</param>
    /// <returns></returns>
    PointInfo ReadFile(string path)
    {
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

        //先读取.ply文件的前10行，把不属于数据的文件消耗掉
        for (int i = 0; i < 33; i++)
        {
            r.ReadLine();
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
    
    /// <summary>
    /// 画出一个文件中的点云
    /// </summary>
    /// <param name="drawList">需要画的点云对象</param>
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
            allParticles[i].startSize = 0.02f;                                    // 设置每个点的大小
        }

        particleSystem.SetParticles(allParticles, pointCount);                    // 点云载入粒子系统
    }

    /// <summary>
    /// 获取某文件夹下文件数量
    /// </summary>
    /// <param name="path">需要获取文件数量的文件夹路径</param>
    /// <returns></returns>
    public int GetFileNums(string path)
    {
        if(Directory.Exists(path))
        {
            //获取目录信息
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            //获取文件信息
            FileInfo[] files = directoryInfo.GetFiles("*.ply", SearchOption.AllDirectories);
            //产生的临时文件个数
            int tempNum = 0;
            //Debug.Log(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                //过滤掉临时文件
                if (files[i].Name.EndsWith(".meta"))
                {
                    tempNum++;
                    continue;
                }
                //Debug.Log(files[i].Extension);//打印一下扩展名
            }
            return files.Length - tempNum;
        }
        else
        {
            Debug.LogError($"文件目录{path}不存在");
            return 0;
        }
    }

    /// <summary>
    /// 画出一个文件中的点云(先读后画)
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="pointInfo">当前文件的点云信息</param>
    void DrawOneFile(string filePath, PointInfo pointInfo)
    {
        // 1.读取数据
        // 提前将点云存成csv文件放在Assert/StreamAssets文件夹下，文本的每行代表一个点，分别为x，y，z
        // csv文件存储数据，用逗号分割，比较容易处理
        pointInfo = ReadFile(filePath);
        // 2.读取完成后，进行渲染，不开线程保证渲染完成后才读取下一个文件
        DrawPointCloud(pointInfo);
    }

    /// <summary>
    /// 开始进行所有点云绘制
    /// </summary>
    /// <param name="particleSystem"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private IEnumerator ShowPointCloud(ParticleSystem particleSystem, float waitTime)
    {
        //获取存放文件的文件夹下有多少个文件(排除临时文件)
        int fileNum = GetFileNums(Application.streamingAssetsPath);
        Debug.Log(fileNum);

        for (int i = 1; i <= fileNum; i++)
        {
            timer.Start();
            //在进行文件命名时，需要统一命名格式，目前使用point + i 来命名
            DrawOneFile((Application.streamingAssetsPath) + "/" + $"{i-1}.ply", pointInfo);
            print($"{i-1}.ply");
            timer.Stop();
            Debug.Log($"第{i}幅点云图渲染完成，耗时{timer.ElapsedMilliseconds}ms");
            timer.Reset();
            yield return new WaitForSeconds(waitTime);
            print("WaitAndPrint " + Time.time);

        }
    }
}