using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Common.Primitives
{
    public abstract class SmartEnum<T> where T : SmartEnum<T>
    {
        public int Value { get; }
        public string Name { get; }

        protected SmartEnum(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() => Name;

        public static IReadOnlyCollection<T> List =>
            typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                     .Select(f => (T)f.GetValue(null)!)
                     .ToList()
                     .AsReadOnly();

        public static T FromValue(int value) =>
            List.First(x => x.Value == value);
    }

}
