using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2DCompetitionGame
{
    /// <summary>
    /// Vector2のラッパー？クラス
    /// </summary>
	public class Vector
	{
		// Public Fields
		public float X { get; set; }
		public float Y { get; set; }

		// Public Properties
		public static Vector Zero { get { return new Vector(0, 0); } }
		public static Vector One { get { return new Vector(1, 1); } }
		public static Vector UnitX { get { return new Vector(1, 0); } }
		public static Vector UnitY { get { return new Vector(0, 1); } }

		// Constructors
		public Vector()
		{
			this.X = 0;
			this.Y = 0;
		}
		public Vector(float X)
		{
			this.X = X;
			this.Y = X;
		}
		public Vector(float X, float Y)
		{
			this.X = X;
			this.Y = Y;
		}

		#region Public Methods
		public static Vector Add(Vector v1, Vector v2)
		{
			return new Vector(v1.X + v2.X, v1.Y + v2.Y);
		}

		public static float Distance(Vector v1, Vector v2)
		{
			return (float)Math.Sqrt((double)((v1.X - v2.X) * (v1.X - v2.X) + (v1.Y - v2.Y) * (v1.Y - v2.Y)));
		}
		public static void Distance(ref Vector value1, ref Vector value2, out float result)
		{
			result = Vector.Distance(value1, value2);
		}

		public static float Dot(Vector vector1, Vector vector2)
		{
			return vector1.X * vector2.X + vector1.Y * vector2.Y;
		}
		public static void Dot(ref Vector value1, ref Vector value2, out float result)
		{
			result = Vector.Dot(value1, value2);
		}
		public static Vector Divide(Vector value1, float divider)
		{
			return new Vector(value1.X / divider, value1.Y / divider);
		}
		public static Vector Divide(Vector value1, Vector value2)
		{
			return new Vector(value1.X / value2.X, value1.Y / value2.Y);
		}

		public bool Equals(Vector other)
		{
			return X == other.X && Y == other.Y;
		}
		
		public float Length()
		{
			return (float)Math.Sqrt((double)(X * X + Y * Y));
		}
		public float Length(Vector vector)
		{
			return (float)Math.Sqrt((double)(vector.X * vector.X + vector.Y * vector.Y));
		}
		public float LengthSquared()
		{
			return (float)Math.Sqrt((double)Length());
		}

		public static Vector Max(Vector value1, Vector value2)
		{
			return new Vector(value1.X <= value2.X ? value2.X : value1.X, value1.Y <= value2.Y ? value2.Y : value1.Y);
		}
		public static void Max(ref Vector value1, ref Vector value2, out Vector result)
		{
			result = Vector.Max(value1, value2);
		}
		public static Vector Min(Vector value1, Vector value2)
		{
			return new Vector(value1.X <= value2.X ? value1.X : value2.X, value1.Y <= value2.Y ? value1.Y : value2.Y);
		}
		public static void Min(ref Vector value1, ref Vector value2, out Vector result)
		{
			result = Vector.Min(value1, value2);
		}

		public static Vector Multiply(Vector value1, float scaleFactor)
		{
			return new Vector(value1.X * scaleFactor, value1.Y * scaleFactor);
		}
		public static Vector Multiply(Vector value1, Vector value2)
		{
			return new Vector(value1.X * value2.X, value1.Y * value2.Y);
		}
		public static Vector Negate(Vector value)
		{
			return new Vector(-value.X, -value.Y);
		}
		/// <summary>
		/// 現在のベクトルを単位ベクトルに変換する。長さが１単位で元のベクトルと同じ方向を指しているベクトルが作成される。
		/// </summary>
		public void Normalize()
		{
			X /= Length();
			Y /= Length();
		}
		public static Vector Normalize(Vector value)
		{
			return new Vector(value.X / value.Length(), value.Y / value.Length());
		}

		public static Vector Subtract(Vector value1, Vector value2)
		{
			return new Vector(value1.X - value2.X, value1.Y - value1.Y);
		}


		public override string ToString()
		{
			return "{X:" + X.ToString() + " Y:" + Y.ToString() + "}";
		}
		#endregion

		#region Oparator
		public static implicit operator Vector2(Vector vector)
		{
			return new Vector2(vector.X, vector.Y);
		}
		public static implicit operator Vector(Vector2 vector)
		{
			return new Vector(vector.X, vector.Y);
		}

		public static Vector operator -(Vector vector)
		{
			return new Vector(-vector.X, -vector.Y);
		}
		public static Vector operator +(Vector vector1, Vector vector2)
		{
			return new Vector(vector1.X + vector2.X, vector1.Y + vector2.Y);
		}
		public static Vector operator +(Vector vector1, Vector2 vector2)
		{
			return new Vector(vector1.X + vector2.X, vector1.Y + vector2.Y);
		}
		public static Vector operator -(Vector vector1, Vector vector2)
		{
			return new Vector(vector1.X - vector2.X, vector1.Y - vector2.Y);
		}
		public static Vector operator -(Vector vector1, Vector2 vector2)
		{
			return new Vector(vector1.X - vector2.X, vector1.Y - vector2.Y);
		}

		/*public static bool operator >(Vector vector1, Vector vector2)
		{
			if (vector1.X > vector2.X)
		}*/
		#endregion
	}
}
