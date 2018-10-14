using Newtonsoft.Json;
using System;

namespace CFLogGet.Tests
{
    public static class TestHelper
    {
        public static void WriteConsole(object obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}