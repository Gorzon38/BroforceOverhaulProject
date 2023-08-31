using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerroristC4Programs.Components
{
    public class MookExtended : MonoBehaviour
    {
        public Mook mook = null;
        public GibHolder decapitationGib = null;
        public bool isUnarmed = false;

        protected virtual void Awake()
        {
            decapitationGib = Map.Instance.activeTheme.mook.As<MookTrooper>().decapitationGib;
        }
    }
}
