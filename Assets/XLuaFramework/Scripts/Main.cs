using UnityEngine;
using System.Collections;
using XLua;
using UnityMMO;

namespace XLuaFramework {
    // 负责整个游戏流程调度，从启动后热更新，渠道SDK接入，各系统的初始化，直到登录才完成使命

    public class Main : MonoBehaviour {
        public enum State{
            CheckExtractResource, //初次运行游戏时需要解压资源文件
            UpdateResourceFromNet, //热更阶段，从服务器上拿到最新的资源
            InitAssetBundle, //初始化AssetBundle
            StartLogin, //登录流程
            StartGame, // 正式进入场景游戏
            Playing, //完成启动流程，接下来控制器交给玩法逻辑
            None, //无
        }
        public enum SubState{
            Enter,Update
        }
        State cur_state = State.None;
        SubState cur_sub_state = SubState.Enter;
        public bool IsNeedPrintConcole = false;
        
        LoadingView loadingView;

        void Start()
        {
            Debug.Log("----------------------------- 游戏开始 Main() -----------------------------");

            AppConfig.Init();

            GameObject.DontDestroyOnLoad(this.gameObject);

            if (IsNeedPrintConcole)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.Android)
                {
                    this.gameObject.AddComponent<LogHandler>();
                }
            }

            this.gameObject.AddComponent<ThreadManager>();
            this.gameObject.AddComponent<ResourceManager>();
            this.gameObject.AddComponent<NetworkManager>();
            this.gameObject.AddComponent<XLuaManager>();
            UnityMMO.NetMsgDispatcher.GetInstance().Init();
            ResMgr.GetInstance().Init();

            Debug.Log("--------------- LoadingView Begin in main -----------------");
            var loadingViewTrans = GameObject.Find("UICanvas/Top/LoadingView");
            loadingViewTrans.gameObject.SetActive(true);
            loadingView = loadingViewTrans.GetComponent<LoadingView>();
            JumpToState(State.CheckExtractResource);
        }

        void JumpToState(State new_state)
        {
            cur_state = new_state;
            cur_sub_state = SubState.Enter;
            Debug.Log("-----------------new_state : " + new_state.ToString());
        }

        private void Update() {
            if (cur_state == State.Playing)
                return;
            switch(cur_state)
            {
                case State.CheckExtractResource:
                    if (cur_sub_state == SubState.Enter)
                    {
                        cur_sub_state = SubState.Update;
                        loadingView.SetData(0.0f,"首次解压游戏数据(不消化流量)");
                        this.gameObject.AddComponent<AssetsHotFixManager>();
                        AssetsHotFixManager.Instance.CheckExtractResource(delegate(float precent){
                            loadingView.SetData(0.3f*precent,"首次解压游戏数据(不消化流量)");
                        }, delegate(){
                            Debug.Log("Main ++++++++++++++++ 解压游戏数据完成");
                            JumpToState(State.UpdateResourceFromNet);
                        });
                    }
                    break;
                case State.UpdateResourceFromNet:
                    if (cur_sub_state == SubState.Enter)
                    {
                        cur_sub_state = SubState.Update;
                        loadingView.SetData(0.3f,"从服务器下载最新的资源文件...");
                        ConfigGame.GetInstance().Load();
                        AssetsHotFixManager.Instance.UpdateResource(delegate(float percent, string tip)
                        {
                            loadingView.SetData(0.3f+0.5f*percent,tip);
                        },delegate(string result)
                        {
                            if (result == "")
                            {
                                Debug.Log("Main ++++++++++++++++ 下载资源完成");
                            }
                            else
                            {
                                Debug.Log(result);
                            }
                            JumpToState(State.InitAssetBundle);
                        });
                    }
                    break;
                case State.InitAssetBundle:
                    if (cur_sub_state == SubState.Enter)
                    {
                        cur_sub_state = SubState.Update;
                        loadingView.SetData(0.8f,"初始化游戏资源...");
                        ResourceManager.GetInstance().Initialize(AppConfig.AssetDir,delegate() {
                            ResMgr.GetInstance().StartPreLoadRes((bool isOK)=>
                            {
                                Debug.Log("Main ++++++++++++++++ 初始化资源完成");
                                JumpToState(State.StartLogin);
                            });
                        });
                    }
                    break;
                case State.StartLogin:
                    if (cur_sub_state == SubState.Enter)
                    {
                        cur_sub_state = SubState.Update;
                        loadingView.SetData(1,"初始化游戏资源完毕");
                        XLuaManager.Instance.InitLuaEnv();
                        XLuaManager.Instance.StartLogin(delegate(){
                            Debug.Log("Main ++++++++++++++++ XLua 登陆流程 完成");
                            JumpToState(State.StartGame);
                        });
                    }
                    break;
                case State.StartGame:
                    if (cur_sub_state == SubState.Enter)
                    {
                        Debug.Log("Main ++++++++++++++++ XLua 进入场景地图");
                        cur_sub_state = SubState.Update;
                        this.gameObject.AddComponent<UnityMMO.MainWorld>();
                        UnityMMO.MainWorld.Instance.StartGame();
                        JumpToState(State.Playing);
                    }                
                    break;
                case State.Playing:
                    break;
            }
        }
    }

}