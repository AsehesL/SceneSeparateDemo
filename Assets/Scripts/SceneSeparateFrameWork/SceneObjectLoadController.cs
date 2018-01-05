using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景物件加载控制器
/// </summary>
public class SceneObjectLoadController : MonoBehaviour
{

    private WaitForEndOfFrame m_WaitForFrame;

    /// <summary>
    /// 当前场景资源四叉树
    /// </summary>
    private QuadTree<SceneObject> m_QuadTree;

    /// <summary>
    /// 刷新时间
    /// </summary>
    private float m_RefreshTime;
    /// <summary>
    /// 销毁时间
    /// </summary>
    private float m_DestroyRefreshTime;
    
    private Vector3 m_OldRefreshPosition;
    private Vector3 m_OldDestroyRefreshPosition;

    /// <summary>
    /// 待加载队列
    /// </summary>
    private Queue<SceneObject> m_CreateObjsQueue;

    /// <summary>
    /// 已加载的物体列表
    /// </summary>
    private List<SceneObject> m_LoadedObjectList;

    /// <summary>
    /// 待销毁物体列表
    /// </summary>
    private List<SceneObject> m_PreDestroyObjectList;

    private TriggerHandle<SceneObject> m_TriggerHandle;

    private bool m_IsCreating;

    private bool m_IsInitialized;

    private int m_MaxCreateCount;
    private int m_MinCreateCount;
    private float m_MaxRefreshTime;
    private float m_MaxDestroyTime;
    private bool m_Asyn;

    private IDetector m_CurrentDetector;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="center">场景区域中心</param>
    /// <param name="size">场景区域大小</param>
    /// <param name="asyn">是否异步</param>
    /// <param name="maxCreateCount">最大创建数量</param>
    /// <param name="minCreateCount">最小创建数量</param>
    /// <param name="maxRefreshTime">更新区域时间间隔</param>
    /// <param name="maxDestroyTime">检查销毁时间间隔</param>
    /// <param name="quadTreeDepth">四叉树深度</param>
    public void Init(Vector3 center, Vector3 size, bool asyn, int maxCreateCount, int minCreateCount, float maxRefreshTime, float maxDestroyTime, int quadTreeDepth = 5)
    {
        if (m_IsInitialized)
            return;
        m_QuadTree = new QuadTree<SceneObject>(center, size, quadTreeDepth);
        m_LoadedObjectList = new List<SceneObject>();
        m_PreDestroyObjectList = new List<SceneObject>();
        m_TriggerHandle = new TriggerHandle<SceneObject>(this.TriggerHandle); 

        m_MaxCreateCount = maxCreateCount;
        m_MinCreateCount = minCreateCount;
        m_MaxRefreshTime = maxRefreshTime;
        m_MaxDestroyTime = maxDestroyTime;
        m_Asyn = asyn;

        m_IsInitialized = true;

        m_RefreshTime = maxRefreshTime;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="center">场景区域中心</param>
    /// <param name="size">场景区域大小</param>
    /// <param name="asyn">是否异步</param>
    public void Init(Vector3 center, Vector3 size, bool asyn)
    {
        Init(center, size, asyn, 25, 15, 1, 5);
    }

    public void AddSceneBlockObject(ISceneObject obj)
    {
        if (!m_IsInitialized)
            return;
        if (m_QuadTree == null)
            return;
        if (obj == null)
            return;
        SceneObject sbobj = new SceneObject(obj);
        m_QuadTree.Add(sbobj);
        if (m_CurrentDetector != null && m_CurrentDetector.IsTrigger(sbobj.Bounds))
        {
            DoCreateInternal(sbobj);
        }
    }

    /// <summary>
    /// 刷新坐标
    /// </summary>
    /// <param name="position">坐标</param>
    public void RefreshPosition(IDetector detector)
    {
        if (!m_IsInitialized)
            return;
        if (m_OldRefreshPosition != detector.Position)
        {
            m_RefreshTime += Time.deltaTime;
            if (m_RefreshTime > m_MaxRefreshTime)
            {
                m_OldRefreshPosition = detector.Position;
                m_RefreshTime = 0;
                m_CurrentDetector = detector;
                m_QuadTree.Trigger(detector, m_TriggerHandle);
                MarkOutofBoundsObjs();
                //m_IsInitLoadComplete = true;
            }
        }
        if (m_OldDestroyRefreshPosition != detector.Position)
        {
            if (m_PreDestroyObjectList != null && m_PreDestroyObjectList.Count >= m_MaxCreateCount)
            {
                m_DestroyRefreshTime += Time.deltaTime;
                if (m_DestroyRefreshTime > m_MaxDestroyTime)
                {
                    m_OldDestroyRefreshPosition = detector.Position;
                    m_DestroyRefreshTime = 0;
                    DestroyOutOfBoundsObjs();
                }
            }
        }
    }

    /// <summary>
    /// 四叉树触发处理函数
    /// </summary>
    /// <param name="data">与当前包围盒发生触发的场景物体</param>
    void TriggerHandle(SceneObject data)
    {
        if (data == null)
            return;
        if (data.Flag == SceneObject.CreateFlag.Old) //如果发生触发的物体已经被创建则标记为不销毁
        {
            data.Flag = SceneObject.CreateFlag.DontDestroy;
        }
        else if (data.Flag == SceneObject.CreateFlag.OutofBounds)
        {
            data.Flag = SceneObject.CreateFlag.DontDestroy;
            if (m_PreDestroyObjectList.Remove(data))
            {
                m_LoadedObjectList.Add(data);
            }
        }
        else if (data.Flag == SceneObject.CreateFlag.None) //如果发生触发的物体未创建则创建该物体并加入已加载的物体列表
        {
            DoCreateInternal(data);
        }
    }

    private void DoCreateInternal(SceneObject data)
    {
        m_LoadedObjectList.Add(data);

        CreateObject(data, m_Asyn);
        //data.Create(transform);
        //if (OnSceneBlockObjectCreate != null)
        //    OnSceneBlockObjectCreate.Invoke(data.targetObj);
    }

    /// <summary>
    /// 标记离开视野的物体
    /// </summary>
    void MarkOutofBoundsObjs()
    {
        if (m_LoadedObjectList == null)
            return;
        int i = 0;
        while (i < m_LoadedObjectList.Count)
        {
            if (m_LoadedObjectList[i].Flag == SceneObject.CreateFlag.Old)
            {

                //m_LoadedObjectList[i].Destroy();
                m_LoadedObjectList[i].Flag = SceneObject.CreateFlag.OutofBounds;
                m_PreDestroyObjectList.Add(m_LoadedObjectList[i]);
                //DestroyObject(m_LoadedObjectList[i], kTestAsyn);
                m_LoadedObjectList.RemoveAt(i);
            }
            else
            {
                m_LoadedObjectList[i].Flag = SceneObject.CreateFlag.Old;
                i++;
            }
        }
    }

    void DestroyOutOfBoundsObjs()
    {
        int i = 0;
        while (i < m_PreDestroyObjectList.Count)
        {
            if (m_PreDestroyObjectList.Count <= m_MinCreateCount)
            {
                return;
            }
            if (m_PreDestroyObjectList[i] == null)
            {
                m_PreDestroyObjectList.RemoveAt(i);
                continue;
            }
            if (m_PreDestroyObjectList[i].Flag == SceneObject.CreateFlag.OutofBounds)
            {
                //m_PreDestroyObjectList[i].Flag = SceneBlockObjectWarper.CreateFlag.None;
                //if (OnSceneBlockObjectDestroy != null)
                //    OnSceneBlockObjectDestroy.Invoke(m_PreDestroyObjectList[i].targetObj);
                DestroyObject(m_PreDestroyObjectList[i], m_Asyn);
                m_PreDestroyObjectList.RemoveAt(i);
                continue;
            }
            i++;
        }
    }

    private void CreateObject(SceneObject warper, bool asyn)
    {
        if (warper == null)
            return;
        if (warper.TargetObj == null)
            return;
        if (warper.Flag == SceneObject.CreateFlag.None)
        {
            if (!asyn)
                CreateObjectSync(warper);
            else
                ProcessObjectAsyn(warper, true);
            warper.Flag = SceneObject.CreateFlag.DontDestroy;
        }
    }

    private void DestroyObject(SceneObject warper, bool asyn)
    {
        if (warper == null)
            return;
        if (warper.Flag == SceneObject.CreateFlag.None)
            return;
        if (warper.TargetObj == null)
            return;
        //if (warper.targetObj.TargetGameObject == null)
        //    return;
        if (!asyn)
            DestroyObjectSync(warper);
        else
            ProcessObjectAsyn(warper, false);
        warper.Flag = SceneObject.CreateFlag.None;
    }

    private void CreateObjectSync(SceneObject warper)
    {
        if (warper.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareDestroy)
        {
            warper.ProcessFlag = SceneObject.CreatingProcessFlag.None;
            return;
        }
        warper.OnShow(transform);
    }

    private void DestroyObjectSync(SceneObject warper)
    {
        if (warper.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareCreate)
        {
            warper.ProcessFlag = SceneObject.CreatingProcessFlag.None;
            return;
        }
        warper.OnHide();
    }

    private void ProcessObjectAsyn(SceneObject warper, bool create)
    {
        if (create)
        {
            if (warper.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareDestroy)
            {
                warper.ProcessFlag = SceneObject.CreatingProcessFlag.None;
                return;
            }
            if (warper.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareCreate)
                return;
            warper.ProcessFlag = SceneObject.CreatingProcessFlag.IsPrepareCreate;
        }
        else
        {
            if (warper.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareCreate)
            {
                warper.ProcessFlag = SceneObject.CreatingProcessFlag.None;
                return;
            }
            if (warper.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareDestroy)
                return;
            warper.ProcessFlag = SceneObject.CreatingProcessFlag.IsPrepareDestroy;
        }
        if (m_CreateObjsQueue == null)
            m_CreateObjsQueue = new Queue<SceneObject>();
        m_CreateObjsQueue.Enqueue(warper);
        if (!m_IsCreating)
        {
            StartCoroutine(AsynCreateProcess());
        }
    }

    private IEnumerator AsynCreateProcess()
    {
        if (m_CreateObjsQueue == null)
            yield return 0;
        m_IsCreating = true;
        while (m_CreateObjsQueue.Count > 0)
        {
            var warper = m_CreateObjsQueue.Dequeue();
            if (warper != null)
            {
                if (warper.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareCreate)
                {
                    warper.ProcessFlag = SceneObject.CreatingProcessFlag.None;
                    if (warper.OnShow(transform))
                    {
                        if (m_WaitForFrame == null)
                            m_WaitForFrame = new WaitForEndOfFrame();
                        yield return m_WaitForFrame;
                    }
                }
                else if (warper.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareDestroy)
                {
                    warper.ProcessFlag = SceneObject.CreatingProcessFlag.None;
                    warper.OnHide();
                    if (m_WaitForFrame == null)
                        m_WaitForFrame = new WaitForEndOfFrame();
                    yield return m_WaitForFrame;
                }
            }
        }
        m_IsCreating = false;
    }

#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        if (m_QuadTree != null)
            m_QuadTree.DrawTree(0, 0.1f);
    }
#endif
}
