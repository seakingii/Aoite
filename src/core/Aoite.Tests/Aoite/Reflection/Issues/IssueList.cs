﻿using System;
using System.Collections;
using Aoite.Reflection;
using Xunit;

namespace Aoite.ReflectionTest.Issues
{

    public class IssueList
    {
        [Fact()]
        public void Issue1()
        {
            Console.WriteLine("List 1: Add() without Aoite.Reflection");
            var list1 = (ArrayList)typeof(ArrayList).CreateInstance();
            for(int i = 0; i < 10; i++)
            {
                list1.Add(i);
            }

            Console.WriteLine("List 1: Add() by Aoite.Reflection");
            var list2 = typeof(ArrayList).CreateInstance();
            for(int i = 0; i < 10; i++)
            {
                list2.CallMethod("Add", i);
            }
            //var size = (int)list2.GetPropertyValue("Count");
            //for (int i = 0; i < size; i++)
            //{
            //    Assert.Equal(i + 1, list2.GetIndexer(i));
            //}
        }
    }
}
