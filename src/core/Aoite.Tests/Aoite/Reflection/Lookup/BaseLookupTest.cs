﻿

using System;
using System.Linq;
using System.Reflection;
using Aoite.ReflectionTest.SampleModel.Animals.Enumerations;

namespace Aoite.ReflectionTest.Lookup
{
    public abstract class BaseLookupTest
    {
        #region Animal Info
        protected static readonly string[] AnimalStaticFieldNames = { "nextId" };
        protected static readonly Type[] AnimalStaticFieldTypes = { typeof(int) };
        protected static readonly string[] AnimalStaticPropertyNames = { "LastID" };
        protected static readonly Type[] AnimalStaticPropertyTypes = { typeof(int) };
        protected static readonly string[] AnimalStaticMethodNames = { "get_LastID" };
        protected static readonly string[] AnimalStaticMemberNames =
            AnimalStaticMethodNames.Concat(AnimalStaticPropertyNames).Concat(AnimalStaticFieldNames).Concat(new[] { ".cctor" }).ToArray();
        protected static readonly MemberTypes[] AnimalStaticMemberTypes =
            Enumerable.Range(0, AnimalStaticFieldNames.Length).Select(i => MemberTypes.Field)
                .Concat(Enumerable.Range(0, AnimalStaticPropertyNames.Length).Select(i => MemberTypes.Property))
                .Concat(Enumerable.Range(0, AnimalStaticMethodNames.Length).Select(i => MemberTypes.Method))
                .Concat(Enumerable.Range(0, 1).Select(i => MemberTypes.Constructor)).ToArray();

        protected static readonly string[] AnimalInstanceFieldNames = { "id", "birthDay", "<ClimateRequirements>k__BackingField", "<MovementCapabilities>k__BackingField" };
        protected static readonly Type[] AnimalInstanceFieldTypes = { typeof(int), typeof(DateTime?), typeof(Climate), typeof(MovementCapabilities) };
        protected static readonly string[] AnimalInstancePropertyNames = { "ID", "BirthDay", "ClimateRequirements", "MovementCapabilities" };
        protected static readonly Type[] AnimalInstancePropertyTypes = AnimalInstanceFieldTypes;
        protected static readonly string[] AnimalInstanceMethodNames = { "get_ID", "get_BirthDay", "set_BirthDay", "get_ClimateRequirements", 
																		 "set_ClimateRequirements", "get_MovementCapabilities", "set_MovementCapabilities" };
        protected static readonly int AnimalConstructorCount = 2;

        protected static readonly string[] AnimalInstanceMemberNames = AnimalInstanceFieldNames
            .Concat(AnimalInstancePropertyNames)
            .Concat(AnimalInstanceMethodNames)
            .Concat(Enumerable.Range(0, AnimalConstructorCount).Select(i => ".ctor")).ToArray();

        protected static readonly MemberTypes[] AnimalInstanceMemberTypes =
            Enumerable.Range(0, AnimalInstanceFieldNames.Length).Select(i => MemberTypes.Field)
            .Concat(Enumerable.Range(0, AnimalInstancePropertyNames.Length).Select(i => MemberTypes.Property))
            .Concat(Enumerable.Range(0, AnimalInstanceMethodNames.Length).Select(i => MemberTypes.Method))
            .Concat(Enumerable.Range(0, AnimalConstructorCount).Select(i => MemberTypes.Constructor)).ToArray();
        #endregion

        #region Mammal Info
        protected static readonly int MammalConstructorCount = 2;
        protected static readonly string[] MammalDeclaredInstanceMemberNames = Enumerable.Range(0, MammalConstructorCount).Select(i => ".ctor").ToArray();
        protected static readonly MemberTypes[] MammalDeclaredInstanceMemberTypes =
            Enumerable.Range(0, MammalDeclaredInstanceMemberNames.Length).Select(i => MemberTypes.Constructor).ToArray();
        #endregion

        #region Lion Info
        protected static readonly string[] LionDeclaredInstanceFieldNames = { "lastMealTime", "<Name>k__BackingField", "<ConstructorInstanceUsed>k__BackingField" };
        protected static readonly Type[] LionDeclaredInstanceFieldTypes = { typeof(DateTime), typeof(string), typeof(int) };
        protected static readonly string[] LionDeclaredInstancePropertyNames = { "Name", "IsHungry", "ConstructorInstanceUsed" };
        protected static readonly Type[] LionDeclaredInstancePropertyTypes = { typeof(string), typeof(bool), typeof(int) };
        protected static readonly string[] LionDeclaredInstanceMethodNames = { "get_Name", "set_Name", "get_IsHungry", 
																		       "get_ConstructorInstanceUsed", "set_ConstructorInstanceUsed" };
        protected static readonly int LionConstructorCount = 4;

        protected static readonly string[] LionDeclaredInstanceMemberNames = LionDeclaredInstanceFieldNames
            .Concat(LionDeclaredInstancePropertyNames)
            .Concat(LionDeclaredInstanceMethodNames)
            .Concat(Enumerable.Range(0, LionConstructorCount).Select(i => ".ctor")).ToArray();

        protected static readonly MemberTypes[] LionDeclaredInstanceMemberTypes =
            Enumerable.Range(0, LionDeclaredInstanceFieldNames.Length).Select(i => MemberTypes.Field)
            .Concat(Enumerable.Range(0, LionDeclaredInstancePropertyNames.Length).Select(i => MemberTypes.Property))
            .Concat(Enumerable.Range(0, LionDeclaredInstanceMethodNames.Length).Select(i => MemberTypes.Method))
            .Concat(Enumerable.Range(0, LionConstructorCount).Select(i => MemberTypes.Constructor)).ToArray();

        protected static readonly string[] LionInstanceFieldNames = LionDeclaredInstanceFieldNames.Concat(AnimalInstanceFieldNames).ToArray();
        protected static readonly Type[] LionInstanceFieldTypes = LionDeclaredInstanceFieldTypes.Concat(AnimalInstanceFieldTypes).ToArray();
        protected static readonly string[] LionInstancePropertyNames = LionDeclaredInstancePropertyNames.Concat(AnimalInstancePropertyNames).ToArray();
        protected static readonly Type[] LionInstancePropertyTypes = LionDeclaredInstancePropertyTypes.Concat(AnimalInstancePropertyTypes).ToArray();

        protected static readonly string[] LionInstanceMethodNames = LionDeclaredInstanceMethodNames.Concat(AnimalInstanceMethodNames).ToArray();
        protected static readonly string[] LionInstanceMemberNames = LionDeclaredInstanceMemberNames.Concat(AnimalInstanceMemberNames).Concat(MammalDeclaredInstanceMemberNames).ToArray();
        protected static readonly MemberTypes[] LionInstanceMemberTypes = LionDeclaredInstanceMemberTypes.Concat(AnimalInstanceMemberTypes).Concat(MammalDeclaredInstanceMemberTypes).ToArray();
        #endregion

        #region Reptile Info
        protected static readonly string[] ReptileDeclaredInstancePropertyNames = { "SlideDistance" };
        protected static readonly Type[] ReptileDeclaredInstancePropertyTypes = { typeof(double) };
        protected static readonly string[] ReptileDeclaredInstanceMethodNames = { "get_SlideDistance", "set_SlideDistance", "Move" };
        protected static readonly Type[][] ReptileDeclaredInstanceMethodParameterTypes = { new[] { typeof(double) } };

        protected static readonly string[] ReptileInstancePropertyNames =
            AnimalInstancePropertyNames.Concat(ReptileDeclaredInstancePropertyNames).ToArray();
        protected static readonly Type[] ReptileInstancePropertyTypes =
            AnimalInstancePropertyTypes.Concat(ReptileDeclaredInstancePropertyTypes).ToArray();
        protected static readonly string[] ReptileInstanceMethodNames =
            AnimalInstanceMethodNames.Concat(ReptileDeclaredInstanceMethodNames).ToArray();
        #endregion

        #region Snake Info
        protected static readonly string[] SnakeDeclaredInstancePropertyNames = { "HasDeadlyBite", "SwimDistance", "SlideDistance" };
        protected static readonly Type[] SnakeDeclaredInstancePropertyTypes = { typeof(bool), typeof(double), typeof(double) };
        protected static readonly string[] SnakeDeclaredInstanceMethodNames = { "get_HasDeadlyBite", "set_HasDeadlyBite", "get_SwimDistance", "set_SwimDistance", 
																		        "get_SlideDistance", "set_SlideDistance", "Move", "Move", "Bite" };
        protected static readonly int SnakeConstructorCount = 2;

        protected static readonly string[] SnakeInstancePropertyNames =
            ReptileInstancePropertyNames.Concat(SnakeDeclaredInstancePropertyNames).ToArray();
        protected static readonly Type[] SnakeInstancePropertyTypes =
            ReptileInstancePropertyTypes.Concat(SnakeDeclaredInstancePropertyTypes).ToArray();
        protected static readonly string[] SnakeInstanceMethodNames =
            ReptileInstanceMethodNames.Concat(SnakeDeclaredInstanceMethodNames).ToArray();
        #endregion
    }
}