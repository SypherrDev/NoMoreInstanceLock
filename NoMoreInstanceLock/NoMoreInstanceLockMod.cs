using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;

[assembly: MelonInfo(typeof(NoMoreInstanceLockMod), "No More Instance Lock", "1.0.0", "Majora, Requi and Ben")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.Green)]

namespace NoMoreInstanceLock
{
    public class NoMoreInstanceLockMod : MelonMod
    {
        private static PropertyInfo _settleStartTime;

        private static MelonLogger.Instance _logger;
        public override void OnApplicationStart()
        {
            _logger = LoggerInstance;

            foreach (var nestedType in typeof(VRCFlowManagerVRC).GetNestedTypes())
            {
                foreach (var methodInfo in nestedType.GetMethods())
                {
                    if (methodInfo.Name != "MoveNext") continue;

                    if (XrefScanner.XrefScan(methodInfo)
                        .Any(z => z.Type == XrefType.Global && z.ReadAsObject() != null && z.ReadAsObject().ToString() == "Executing Buffered Events"))
                    {
                        _settleStartTime = nestedType.GetProperty("field_Private_Single_0");

                        HarmonyInstance.Patch(methodInfo, typeof(NoMoreInstanceLockMod).GetMethod(nameof(MoveNextPatch), BindingFlags.Static | BindingFlags.NonPublic).ToNewHarmonyMethod());
                    }
                }
            }
        }

        private static void MoveNextPatch(object __instance)
        {
            if (__instance == null) return;
            var eventReplicator = VRC_EventLog.field_Internal_Static_VRC_EventLog_0?.field_Internal_EventReplicator_0;

            if (eventReplicator != null && !eventReplicator.field_Private_Boolean_0 && Time.realtimeSinceStartup - (float)_settleStartTime.GetValue(__instance) >= 10.0)
            {
                eventReplicator.field_Private_Boolean_0 = true;
                _logger.Msg(ConsoleColor.Yellow, "Network didn't settle, ignoring and joining anyways... (World may be broken!)");
            }
        }
    }
}
