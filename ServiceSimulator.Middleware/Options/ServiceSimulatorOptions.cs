using System;
using System.Collections.Generic;

namespace ServiceSimulator
{
    public class ServiceSimulatorOptions
    {
        public string? RequestStart { get; set; }
        public string? RootDirectory { get; set; }

        private readonly Dictionary<string, OperationInfo> _requestToInfoMap = new Dictionary<string, OperationInfo>(StringComparer.OrdinalIgnoreCase);

        internal void Add(OperationInfo? item)
        {
            if (item is null)
                return;

            if (string.IsNullOrWhiteSpace(item.Request))
                return;

            if (_requestToInfoMap.ContainsKey(item.Request!))
                return;

            _requestToInfoMap.Add(item.Request!, item);
        }

        public OperationInfo GetInfo(string request)
        {
            _requestToInfoMap.TryGetValue(request, out var result);
            return result;
        }
    }
}
