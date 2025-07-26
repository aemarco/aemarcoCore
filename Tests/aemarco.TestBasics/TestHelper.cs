using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace aemarco.TestBasics;

public static class TestHelper
{

    public static void NothingExpected(object? obj)
    {
        if (obj is null)
            return;

        Assert.Warn($"""
                     Expected Nothing but found: <{GetTypeName(obj)}>:
                     {JsonConvert.SerializeObject(obj, Formatting.Indented)}


                     """);
    }

    public static void PrintPassed(object? obj)
    {
        if (obj is null)
        {
            TestContext.Out.WriteLine("""
                                      Passed with:
                                      null
                                      
                                      
                                      """);
            return;
        }
        TestContext.Out.WriteLine($"""
                                   Passed with: <{GetTypeName(obj)}>:
                                    {JsonConvert.SerializeObject(obj, Formatting.Indented)}
                                    
                                    
                                   """);
    }



    private static string GetTypeName(object obj)
    {
        var type = obj.GetType();
        if (obj is not IEnumerable or string)
            return type.Name;

        var elementType = GetCollectionElementType(type);
        return $"{elementType?.Name ?? "UnknownType"}[]";
    }
    private static Type? GetCollectionElementType(Type collectionType)
    {
        if (collectionType.IsArray)
            return collectionType.GetElementType();


        // Handle collections implementing IEnumerable<T>
        var genericArguments = collectionType.GetGenericArguments();
        if (genericArguments.Length > 0)
            return genericArguments[0];


        // Handle non-generic collections implementing IEnumerable
        if (!typeof(IEnumerable).IsAssignableFrom(collectionType))
            return null;

        foreach (var interfaceType in collectionType.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return interfaceType.GetGenericArguments()[0];
            }
        }
        return null;
    }

}
