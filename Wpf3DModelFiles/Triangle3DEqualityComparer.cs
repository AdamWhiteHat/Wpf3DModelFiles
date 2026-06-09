using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Wpf3DModelFiles
{
    public class Triangle3DEqualityComparer : IEqualityComparer<Triangle3D>
    {
        public bool Equals(Triangle3D? left, Triangle3D? right)

        {
            if (!Point3D.Equals(left.A.Location, right.A.Location))
            {
                return false;
            }
            if (!Point3D.Equals(left.B.Location, right.B.Location))
            {
                return false;
            }
            if (!Point3D.Equals(left.C.Location, right.C.Location))
            {
                return false;
            }
            return true;
        }

        public int GetHashCode([DisallowNull] Triangle3D obj)
        {
            return HashCode.Combine(obj.A.Location.GetHashCode(), obj.B.Location.GetHashCode(), obj.C.Location.GetHashCode());
        }
    }
}
