using ExitGames.Client.Photon;
using HarmonyLib;
using MelonLoader;
using NoMoreInstanceLock;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: MelonInfo(typeof(NoMoreInstanceLockMod), "No More Instance Lock", "1.0.0", "Majora")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.Green)]

namespace NoMoreInstanceLock 
{
    public class NoMoreInstanceLockMod : MelonMod
    {
        public override void OnApplicationStart()
        {
            HarmonyInstance.Patch(typeof(LoadBalancingClient).GetMethod(nameof(LoadBalancingClient.Method_Public_Virtual_New_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0)), prefix: new HarmonyMethod(typeof(NoMoreInstanceLockMod).GetMethod("OpRaiseEvent", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
        }

        public static void OpRaiseEvent(byte __0, Il2CppSystem.Object __1, RaiseEventOptions __2, SendOptions __3) 
        {
            if (__0 == 3)
            {
                VRC_EventLog.field_Internal_Static_VRC_EventLog_0.field_Internal_EventReplicator_0.field_Private_Boolean_0 = true;
            }
        }
    }
}

