using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Sinyi.Caching.Memory.Services
{
    [Authorize]
    [MDP.AspNetCore.Module("Sinyi-MemoryCache")]
    public partial class MemoryCacheController : Controller
    {
        // Fields
        private readonly MemoryCacheClient _memoryCache = null;


        // Constructors
        public MemoryCacheController(MemoryCacheClient memoryCache)
        {
            #region Contracts

            if (memoryCache == null) throw new ArgumentException(nameof(memoryCache));

            #endregion

            // Default
            _memoryCache = memoryCache;
        }
    }

    public partial class MemoryCacheController : Controller
    {
        // Methods
        public ActionResult<RemoveResultModel> Remove([FromBody] RemoveActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Remove
            var result = _memoryCache.Remove(actionModel.Key);

            // Return
            return new RemoveResultModel()
            {
                Result = result
            };
        }


        // Class
        public class RemoveActionModel
        {
            // Properties
            public string Key { get; set; }
        }

        public class RemoveResultModel
        {
            // Properties
            public bool Result { get; set; }
        }
    }
}
