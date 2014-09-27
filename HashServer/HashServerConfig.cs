using JA.Clizby;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HashServer
{
    public class HashServerConfig
    {
        [Alias("Directory")]
        public IEnumerable<string> Directories { get; set; }

        [Alias("Root", "Recursive", "Tree")]
        public IEnumerable<string> RecursiveDirectories { get; set; }

        [Alias("Exclude")]
        public IEnumerable<string> Excludes { get; set; }

        public static HashServerConfig Parse(IEnumerable<string> args)
        {
            return new OptionReader<HashServerConfig>(
                    new HashServerConfigMapper(config => config.Directories),
                    new HashServerConfigMapper(config => config.Excludes),
                    new HashServerConfigMapper(config => config.RecursiveDirectories))
                .Parse(args);
        }
    }

    public class HashServerConfigMapper : IMapper<HashServerConfig>
    {
        protected Expression<Func<HashServerConfig, IEnumerable<string>>> PropertyAccessor { get; set; }
        protected IList<string> _values = new List<string>();
        protected PropertyInfo Property { get { return (PropertyInfo)((MemberExpression)PropertyAccessor.Body).Member; } }

        public string Name { get { return Property.Name; } }

        public HashServerConfigMapper(Expression<Func<HashServerConfig, IEnumerable<string>>> propertyAccessor)
        {
            PropertyAccessor = propertyAccessor;
        }

        public void Set(HashServerConfig target, string value)
        {
            if (Property.GetValue(target) == null)
                Property.SetValue(target, _values);

            _values.Add(value);
        }
        
        public bool Validate(HashServerConfig target)
        {
            if (Property.GetValue(target) == null)
                Property.SetValue(target, _values);

            return ((IEnumerable<string>)Property.GetValue(target)).All(Directory.Exists);
        }
    }
}
