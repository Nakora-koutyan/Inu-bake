﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UnityEngine.Rendering
{
    internal ref struct HashFNV1A32
    {
        /// <summary>
        /// FNV prime.
        /// </summary>
        const uint k_Prime = 16777619;

        /// <summary>
        /// FNV offset basis.
        /// </summary>
        const uint k_OffsetBasis = 2166136261;

        uint m_Hash;

        public static HashFNV1A32 Create()
        {
            return new HashFNV1A32 { m_Hash = k_OffsetBasis };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(in int input)
        {
            unchecked
            {
                m_Hash = (m_Hash ^ (uint)input) * k_Prime;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(in uint input)
        {
            unchecked
            {
                m_Hash = (m_Hash ^ input) * k_Prime;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(in bool input)
        {
            m_Hash = (m_Hash ^ (input ? 1u : 0u)) * k_Prime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(in float input)
        {
            unchecked
            {
                m_Hash = (m_Hash ^ (uint)input.GetHashCode()) * k_Prime;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(in double input)
        {
            unchecked
            {
                m_Hash = (m_Hash ^ (uint)input.GetHashCode()) * k_Prime;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(in Vector2 input)
        {
            unchecked
            {
                m_Hash = (m_Hash ^ (uint)input.GetHashCode()) * k_Prime;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(in Vector3 input)
        {
            unchecked
            {
                m_Hash = (m_Hash ^ (uint)input.GetHashCode()) * k_Prime;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(in Vector4 input)
        {
            unchecked
            {
                m_Hash = (m_Hash ^ (uint)input.GetHashCode()) * k_Prime;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append<T>(T input) where T : struct
        {
            unchecked
            {
                m_Hash = (m_Hash ^ (uint)input.GetHashCode()) * k_Prime;
            }
        }

        public int value => (int)m_Hash;

        public override int GetHashCode()
        {
            return value;
        }
    }

    static class DelegateHashCodeUtils
    {
        //Cache to prevent CompilerGeneratedAttribute extraction for known delegate
        static readonly Lazy<Dictionary<int, bool>> s_MethodHashCodeToSkipTargetHashMap = new(() => new Dictionary<int, bool>(64));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFuncHashCode(Delegate del)
        {
            //Get MethodInfo hash code as the main one to be used
            var methodHashCode = RuntimeHelpers.GetHashCode(del.Method);

            //Check if we are dealing with lambda or static delegates and skip target if we are.
            //Static methods have a null Target.
            //Lambdas have a CompilerGeneratedAttribute as they are generated by a compiler.
            //If Lambda have any captured variable Target hashcode will be different each time we re-create lambda.
            if (!s_MethodHashCodeToSkipTargetHashMap.Value.TryGetValue(methodHashCode, out var skipTarget))
            {
                skipTarget = del.Target == null || (
                    del.Method.DeclaringType?.IsNestedPrivate == true &&
                    Attribute.IsDefined(del.Method.DeclaringType, typeof(CompilerGeneratedAttribute), false)
                );

                s_MethodHashCodeToSkipTargetHashMap.Value[methodHashCode] = skipTarget;
            }

            //Combine method info hashcode and target hashcode if needed
            return skipTarget ? methodHashCode : methodHashCode ^ RuntimeHelpers.GetHashCode(del.Target);
        }

        //used for testing
        internal static int GetTotalCacheCount() => s_MethodHashCodeToSkipTargetHashMap.Value.Count;

        internal static void ClearCache() => s_MethodHashCodeToSkipTargetHashMap.Value.Clear();
    }
}