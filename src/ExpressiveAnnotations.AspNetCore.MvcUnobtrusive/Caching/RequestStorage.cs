using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace ExpressiveAnnotations.AspNetCore.MvcUnobtrusive.Caching
{
    internal class RequestStorage
    {
        private readonly HttpContext _httpContext;

        internal RequestStorage(IHttpContextAccessor httpContextAccessor) => _httpContext = httpContextAccessor.HttpContext;

        private IDictionary<object, object> Items
        {
            get
            {
                if (_httpContext == null)
                {
                    throw new ApplicationException("HttpContext not available.");
                }

                return _httpContext.Items;
            }
        }

        internal T Get<T>(string key) => Items[key] == null ? default(T) : (T)Items[key];

        internal void Set<T>(string key, T value) => Items[key] = value;

        internal void Remove(string key) => Items.Remove(key);
    }
}
