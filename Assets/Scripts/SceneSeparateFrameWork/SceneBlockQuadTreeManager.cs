using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBlockQuadTreeManager : MonoBehaviour
{

    private WaitForEndOfFrame m_WaitForFrame;

    /// <summary>
    /// 当前场景资源四叉树
    /// </summary>
    private SceneBlockQuadTree<SceneBlockObject> m_QuadTree;

    /// <summary>
    /// 包围盒
    /// </summary>
    private Bounds m_CheckBounds;

    private float m_RefreshTime;
    private float m_DestroyRefreshTime;

    private const int kMaxCreateObjectCount = 70;
    private Vector3 m_OldRefreshPosition;
    private Vector3 m_OldDestroyRefreshPosition;

    private Queue<SceneBlockObject> m_CreateObjsQueue;

    /// <summary>
    /// 已加载的物体列表
    /// </summary>
    private List<SceneBlockObject> m_LoadedObjectList;

    /// <summary>
    /// 待销毁物体列表
    /// </summary>
    private List<SceneBlockObject> m_PreDestroyObjectList;

    private TriggerHandle<SceneBlockObject> m_TriggerHandle;

    private bool m_IsCreating;

    private bool m_IsInitialized;

    private int m_MaxCreateCount;
    private float m_MaxRefreshTime;
    private float m_MaxDestroyTime;
    private bool m_Asyn;

    public void Init(int maxCreateCount, float maxRefreshTime, float maxDestroyTime, bool asyn, Vector3 center, Vector3 size, Vector3 visibleSize, int quadTreeDepth)
    {
        if (m_IsInitialized)
            return;
        m_QuadTree = new SceneBlockQuadTree<SceneBlockObject>(center, size, quadTreeDepth);
        m_CheckBounds = new Bounds(Vector3.zero, visibleSize);
        m_LoadedObjectList = new List<SceneBlockObject>();
        m_PreDestroyObjectList = new List<SceneBlockObject>();
        m_TriggerHandle = new TriggerHandle<SceneBlockObject>(this.TriggerHandle); 

        m_MaxCreateCount = maxCreateCount;
        m_MaxRefreshTime = maxRefreshTime;
        m_MaxDestroyTime = maxDestroyTime;
        m_Asyn = asyn;

        m_IsInitialized = true;
    }

    public void AddSceneBlockObject(ISceneBlockObject obj)
    {
        if (!m_IsInitialized)
            return;
        if (m_QuadTree == null)
            return;
        if (obj == null)
            return;
        SceneBlockObject sbobj = new SceneBlockObject(obj);
        m_QuadTree.Add(sbobj);
        if (sbobj.Bounds.Intersects(m_CheckBounds))
        {
            DoCreateInternal(sbobj);
        }
    }

    /// <summary>
    /// 刷新坐标
    /// </summary>
    /// <param name="position">坐标</param>
    public void RefreshPosition(Vector3 position)
    {
        if (!m_IsInitialized)
            return;
        if (m_OldRefreshPosition != position)
        {
            m_RefreshTime += Time.deltaTime;
            if (m_RefreshTime > m_MaxRefreshTime)
            {
                m_OldRefreshPosition = position;
                m_RefreshTime = 0;
                m_CheckBounds.center = position;
                m_QuadTree.Trigger(m_CheckBounds, m_TriggerHandle);
                MarkOutofBoundsObjs();
                //m_IsInitLoadComplete = true;
            }
        }
        if (m_OldDestroyRefreshPosition != position)
        {
            if (m_PreDestroyObjectList != null && m_PreDestroyObjectList.Count >= kMaxCreateObjectCount)
            {
                m_DestroyRefreshTime += Time.deltaTime;
                if (m_DestroyRefreshTime > m_MaxDestroyTime)
                {
                    m_OldDestroyRefreshPosition = position;
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
    void TriggerHandle(SceneBlockObject data)
    {
        if (data == null)
            return;
        if (data.Flag == SceneBlockObject.CreateFlag.Old) //如果发生触发的物体已经被创建则标记为不销毁
        {
            data.Flag = SceneBlockObject.CreateFlag.DontDestroy;
        }
        else if (data.Flag == SceneBlockObject.CreateFlag.OutofBounds)
        {
            data.Flag = SceneBlockObject.CreateFlag.DontDestroy;
            if (m_PreDestroyObjectList.Remove(data))
            {
                m_LoadedObjectList.Add(data);
            }
        }
        else if (data.Flag == SceneBlockObject.CreateFlag.None) //如果发生触发的物体未创建则创建该物体并加入已加载的物体列表
        {
            DoCreateInternal(data);
        }
    }

    private void DoCreateInternal(SceneBlockObject data)
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
            if (m_LoadedObjectList[i].Flag == SceneBlockObject.CreateFlag.Old)
            {

                //m_LoadedObjectList[i].Destroy();
                m_LoadedObjectList[i].Flag = SceneBlockObject.CreateFlag.OutofBounds;
                m_PreDestroyObjectList.Add(m_LoadedObjectList[i]);
                //DestroyObject(m_LoadedObjectList[i], kTestAsyn);
                m_LoadedObjectList.RemoveAt(i);
            }
            else
            {
                m_LoadedObjectList[i].Flag = SceneBlockObject.CreateFlag.Old;
                i++;
            }
        }
    }

    void DestroyOutOfBoundsObjs()
    {
        int i = 0;
        while (i < m_PreDestroyObjectList.Count)
        {
            if (m_PreDestroyObjectList.Count <= m_MaxCreateCount)
            {
                return;
            }
            if (m_PreDestroyObjectList[i] == null)
            {
                m_PreDestroyObjectList.RemoveAt(i);
                continue;
            }
            if (m_PreDestroyObjectList[i].Flag == SceneBlockObject.CreateFlag.OutofBounds)
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

    private void CreateObject(SceneBlockObject warper, bool asyn)
    {
        if (warper == null)
            return;
        if (warper.TargetObj == null)
            return;
        if (warper.Flag == SceneBlockObject.CreateFlag.None)
        {
            if (!asyn)
                CreateObjectSync(warper);
            else
                ProcessObjectAsyn(warper, true);
            warper.Flag = SceneBlockObject.CreateFlag.DontDestroy;
        }
    }

    private void DestroyObject(SceneBlockObject warper, bool asyn)
    {
        if (warper == null)
            return;
        if (warper.Flag == SceneBlockObject.CreateFlag.None)
            return;
        if (warper.TargetObj == null)
            return;
        //if (warper.targetObj.TargetGameObject == null)
        //    return;
        if (!asyn)
            DestroyObjectSync(warper);
        else
            ProcessObjectAsyn(warper, false);
        warper.Flag = SceneBlockObject.CreateFlag.None;
    }

    private void CreateObjectSync(SceneBlockObject warper)
    {
        if (warper.ProcessFlag == SceneBlockObject.CreatingProcessFlag.IsPrepareDestroy)
        {
            warper.ProcessFlag = SceneBlockObject.CreatingProcessFlag.None;
            return;
        }
        warper.OnShow(transform);
    }

    private void DestroyObjectSync(SceneBlockObject warper)
    {
        if (warper.ProcessFlag == SceneBlockObject.CreatingProcessFlag.IsPrepareCreate)
        {
            warper.ProcessFlag = SceneBlockObject.CreatingProcessFlag.None;
            return;
        }
        warper.OnHide();
    }

    private void ProcessObjectAsyn(SceneBlockObject warper, bool create)
    {
        if (create)
        {
            if (warper.ProcessFlag == SceneBlockObject.CreatingProcessFlag.IsPrepareDestroy)
            {
                warper.ProcessFlag = SceneBlockObject.CreatingProcessFlag.None;
                return;
            }
            if (warper.ProcessFlag == SceneBlockObject.CreatingProcessFlag.IsPrepareCreate)
                return;
            warper.ProcessFlag = SceneBlockObject.CreatingProcessFlag.IsPrepareCreate;
        }
        else
        {
            if (warper.ProcessFlag == SceneBlockObject.CreatingProcessFlag.IsPrepareCreate)
            {
                warper.ProcessFlag = SceneBlockObject.CreatingProcessFlag.None;
                return;
            }
            if (warper.ProcessFlag == SceneBlockObject.CreatingProcessFlag.IsPrepareDestroy)
                return;
            warper.ProcessFlag = SceneBlockObject.CreatingProcessFlag.IsPrepareDestroy;
        }
        if (m_CreateObjsQueue == null)
            m_CreateObjsQueue = new Queue<SceneBlockObject>();
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
                if (warper.ProcessFlag == SceneBlockObject.CreatingProcessFlag.IsPrepareCreate)
                {
                    warper.ProcessFlag = SceneBlockObject.CreatingProcessFlag.None;
                    if (warper.OnShow(transform))
                    {
                        if (m_WaitForFrame == null)
                            m_WaitForFrame = new WaitForEndOfFrame();
                        yield return m_WaitForFrame;
                    }
                }
                else if (warper.ProcessFlag == SceneBlockObject.CreatingProcessFlag.IsPrepareDestroy)
                {
                    warper.ProcessFlag = SceneBlockObject.CreatingProcessFlag.None;
                    warper.OnHide();
                    if (m_WaitForFrame == null)
                        m_WaitForFrame = new WaitForEndOfFrame();
                    yield return m_WaitForFrame;
                }
            }
        }
        m_IsCreating = false;
    }
}
