using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sora.Extension {
    interface IDeepClone<T>
                   where T : class
    {
        ///<summary>
        /// Return a copy of this instance
        ///</summary>
        public T Clone();    
    }
}
