using System;
using AutoMapper;
class Program {
    static void Main() {
        var t = typeof(MapperConfiguration);
        foreach (var c in t.GetConstructors())
            Console.WriteLine(c);
    }
}
