using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheGeneralsTraining.Components
{
    public class ExtendedBroComponent<T> : MonoBehaviour where T : BroBase
    {
        public T bro;

        protected virtual void Awake()
        {
            bro = GetComponent<T>();
            if (bro == null)
                Destroy(this);
        }
    }
}
