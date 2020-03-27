using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityModel
{
    public class ContractResolver<T> : DefaultContractResolver
    {
        protected Dictionary<string, string> PropertyMappings { get; set; }

        protected void AddMap<U>(Expression<Func<T, U>> expression)
        {
            if (this.PropertyMappings == null)
            {
                this.PropertyMappings = new Dictionary<string, string>();
            }

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                return;
            }

            this.PropertyMappings.Add(memberExpression.Member.Name, memberExpression.Member.Name);
        }
        protected void AddMap<U>(Expression<Func<T, U>> expression, string jsonPropertyName)
        {
            if (this.PropertyMappings == null)
            {
                this.PropertyMappings = new Dictionary<string, string>();
            }

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                return;
            }

            this.PropertyMappings.Add(memberExpression.Member.Name, jsonPropertyName);
        }
        protected override string ResolvePropertyName(string propertyName)
        {
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out string resolvedName);
            return resolved ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }
}
