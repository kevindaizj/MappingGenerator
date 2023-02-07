using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsBlazor.Services
{
    public static class MappingService
    {
        public static string MapToInstace(string assemblyPath, string? fromTypeName, string toTypeName, string? fromVarName = null)
        {
            var isFromNull = string.IsNullOrWhiteSpace(fromTypeName);
            var (fromProps, toProps) = GetFromToProperties(assemblyPath, fromTypeName, toTypeName);

            var sb = new StringBuilder();
            var nl = System.Environment.NewLine;
            sb.Append($"new {toTypeName.Split(".").Last()} {nl}");
            sb.Append("{");
            sb.Append(nl);

            foreach (var tp in toProps)
            {
                var name = tp.Name;
                var fp = fromProps.FirstOrDefault(p => p.Name.ToUpper() == name.ToUpper());
                var fpName = fp?.Name ?? (isFromNull ? name : string.Empty);

                var comment = fp == null ? (isFromNull ? "" : "//") : "";

                sb.Append($"    {comment}{name} = {fromVarName ?? "o"}.{fpName}, {nl}");
            }

            sb.Append("}");

            return sb.ToString();
        }

        public static string MapToProperties(string assemblyPath, string? fromTypeName, string toTypeName, string? fromVarName = null, string? toVarName = null)
        {
            var isFromNull = string.IsNullOrWhiteSpace(fromTypeName);
            var (fromProps, toProps) = GetFromToProperties(assemblyPath, fromTypeName, toTypeName);

            var sb = new StringBuilder();
            var nl = System.Environment.NewLine;

            foreach (var tp in toProps)
            {
                var name = tp.Name;
                var fp = fromProps.FirstOrDefault(p => p.Name.ToUpper() == name.ToUpper());
                var fpName = fp?.Name ?? (isFromNull ? name : string.Empty);

                var comment = fp == null ? (isFromNull ? "" : "//") : "";

                sb.Append($"{comment}{toVarName ?? "to"}.{name} = {fromVarName ?? "from"}.{fpName}; {nl}");
            }

            return sb.ToString();
        }

        private static (List<PropertyInfo> from, List<PropertyInfo> to) GetFromToProperties(string assemblyPath, string? fromTypeName, string toTypeName)
        {
            var asm = Assembly.LoadFrom(assemblyPath);
            var types = asm.GetTypes().Where(t => t.IsClass).ToList();

            var fromType = fromTypeName != null ? types.SingleOrDefault(t => t.FullName == fromTypeName) : null;
            var toType = types.SingleOrDefault(t => t.FullName == toTypeName);

            var fromProps = fromType?.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList() ?? new List<PropertyInfo>();
            var toProps = toType?.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList() ?? new List<PropertyInfo>();

            return (fromProps, toProps);
        }
    }
}
