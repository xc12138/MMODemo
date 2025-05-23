﻿#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class UnityMMOGlobalEventsWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(UnityMMO.GlobalEvents);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 8, 0, 0);
			
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SceneChanged", UnityMMO.GlobalEvents.SceneChanged);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MessageShow", UnityMMO.GlobalEvents.MessageShow);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "AlertShow", UnityMMO.GlobalEvents.AlertShow);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "SkillCDChanged", UnityMMO.GlobalEvents.SkillCDChanged);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MainRoleDie", UnityMMO.GlobalEvents.MainRoleDie);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "MainRoleHPChanged", UnityMMO.GlobalEvents.MainRoleHPChanged);
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "ExpChanged", UnityMMO.GlobalEvents.ExpChanged);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "UnityMMO.GlobalEvents does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        
        
        
        
        
		
		
		
		
    }
}
