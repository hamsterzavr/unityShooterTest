using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Vector3Assistant
    {
        public static float product(Vector3 a,Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }
    }
}
