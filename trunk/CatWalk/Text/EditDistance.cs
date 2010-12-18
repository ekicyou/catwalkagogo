/*
	$Id$
*/

using System;
using System.Collections.Generic;

namespace CatWalk.Text{
	public static class EditDistance{
		public static int GetDistance(string s, string t){
			return UnsafeVectorLevenshtein(s, t);
		}
		
		public static int GetDistanceTo(this string s, string t){
			return UnsafeVectorLevenshtein(s, t);
		}
		
		/// <summary>
		/// Compute Levenshtein distance
		/// GNU C version ported to C#
		/// Memory efficient version
		/// Cetin Sert, PHP Dev Team, Free Software Foundation
		/// http://www.koders.com/c/fid95F7F5D4792831FB74EB61BCD353ECD6DC38A794.aspx
		/// </summary>
		/// <returns>
		/// Levenshtein edit distance if that is not greater than 65535; otherwise -1
		/// </returns>
		internal static unsafe int GNULevenshtein(string s, string t){
			fixed (char* fps = s)
			fixed (char* fpt = t)
			{
				char* ps = fps;
				char* pt = fpt;
				int i, j, n;
				int l1, l2;
				char* p1, p2, tmp;

				/* skip equal start sequence, if any */
				if (s.Length >= t.Length)
				{
					while (*ps == *pt)
					{
						/* if we already used up one string,
						 * then the result is the length of the other */
						if (*ps == '\0') break;
						ps++; pt++;
					}
				}
				else // sl < tl
				{
					while (*ps == *pt)
					{
						/* if we already used up one string,
						 * then the result is the length of the other */
						if (*pt == '\0') break;
						ps++; pt++;
					}
				}

				/* length count #1*/
				l1 = s.Length - (int)(ps - fps);
				l2 = t.Length - (int)(pt - fpt);

				/* if we already used up one string, then
				 the result is the length of the other */
				if (*ps == '\0') return l2;
				if (*pt == '\0') return l1;

				/* length count #2*/
				ps += l1;
				pt += l2;

				/* cut of equal tail sequence, if any */
				while (*--ps == *--pt)
				{
					l1--; l2--;
				}

				/* reset pointers, adjust length */
				ps -= l1++;
				pt -= l2++;

				/* possible dist to great? */
				//if ((l1 - l2 >= 0 ? l1 - l2 : -(l1 - l2)) >= char.MaxValue) return -1;
				if (Math.Abs(l1 - l2) >= char.MaxValue) return -1;

				/* swap if l2 longer than l1 */
				if (l1 < l2)
				{
					tmp = ps; ps = pt; pt = tmp;
					l1 ^= l2; l2 ^= l1; l1 ^= l2;
				}

				/* fill initial row */
				n = (*ps != *pt) ? 1 : 0;
				char* r = stackalloc char[l1 * 2];
				for (i = 0, p1 = r; i < l1; i++, *p1++ = (char)n++, p1++) { /*empty*/}

				/* calc. rowwise */
				for (j = 1; j < l2; j++)
				{
					/* init pointers and col#0 */
					p1 = r + ((j & 1) == 0 ? 1 : 0);
					p2 = r + (j & 1);
					n = *p1 + 1;
					*p2++ = (char)n; p2++;
					pt++;

					/* foreach column */
					for (i = 1; i < l1; i++)
					{
						if (*p1 < n) n = *p1 + (*(ps + i) != *pt ? 1 : 0); /* replace cheaper than delete? */
						p1++;
						if (*++p1 < n) n = *p1 + 1; /* insert cheaper then insert ? */
						*p2++ = (char)n++; /* update field and cost for next col's delete */
						p2++;
					}
				}

				/* return result */
				return n - 1;
			}
		}


		/// <summary>
		/// Compute Levenshtein distance
		/// Single Dimensional array vector unsafe version
		/// Memory efficient version
		/// Cetin Sert, Sten Hjelmqvist
		/// http://www.codeproject.com/cs/algorithms/Levenshtein.asp
		/// </summary>
		/// <returns>Levenshtein edit distance</returns>
		internal static unsafe int UnsafeVectorLevenshtein(string s, string t){
			fixed (char* ps = s)
			fixed (char* pt = t)
			{
				int n = s.Length;	   // length of s
				int m = t.Length;	   // length of t
				int cost;			   // cost

				// Step 1
				if (n == 0) return m;
				if (m == 0) return n;

				/// Create the two vectors
				int* v0 = stackalloc int[n + 1];
				int* v1 = stackalloc int[n + 1];
				int* vTmp;


				// Step 2
				// Initialize the first vector
				for (int i = 1; i <= n; i++) v0[i] = i;


				// Step 3
				// For each column - unsafe
				for (int j = 1; j <= m; j++)
				{
					v1[0] = j;

					// Step 4
					for (int i = 1; i <= n; i++)
					{

						// Step 5
						cost = (ps[i - 1] == pt[j - 1]) ? 0 : 1;

						// Step 6
						int m_min = v0[i] + 1;
						int b = v1[i - 1] + 1;
						int c = v0[i - 1] + cost;

						if (b < m_min) m_min = b;
						if (c < m_min) m_min = c;

						v1[i] = m_min;
					}

					/// Swap the vectors
					vTmp = v0;
					v0 = v1;
					v1 = vTmp;
				}

				// Step 7
				return v0[n];
			}
		}


		/// <summary>
		/// Compute Levenshtein distance
		/// Single Dimensional array vector version
		/// Memory efficient version
		/// Sten Hjelmqvist
		/// http://www.codeproject.com/cs/algorithms/Levenshtein.asp
		/// </summary>
		/// <returns>Levenshtein edit distance</returns>
		internal static int VectorLevenshtein(string s, string t){
			int n = s.Length;	   // length of s
			int m = t.Length;	   // length of t
			int cost;			   // cost

			// Step 1
			if (n == 0) return m;
			if (m == 0) return n;

			/// Create the two vectors
			int[] v0 = new int[n + 1];
			int[] v1 = new int[n + 1];
			int[] vTmp;


			/// Step 2
			/// Initialize the first vector
			for (int i = 1; i <= n; i++) v0[i] = i;


			// Step 3
			// Fore each column
			for (int j = 1; j <= m; j++)
			{
				/// Set the 0'th element to the column number
				v1[0] = j;

				// Step 4
				/// Fore each row
				for (int i = 1; i <= n; i++)
				{

					// Step 5
					cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

					// Step 6
					/// Find minimum
					int m_min = v0[i] + 1;
					int b = v1[i - 1] + 1;
					int c = v0[i - 1] + cost;

					if (b < m_min) m_min = b;
					if (c < m_min) m_min = c;

					v1[i] = m_min;
				}

				/// Swap the vectors
				vTmp = v0;
				v0 = v1;
				v1 = vTmp;
			}

			// Step 7
			return v0[n];
		}


		/// <summary>
		/// Compute Levenshtein distance
		/// 2 Dimensional array matrix version
		/// </summary>
		/// <returns>Levenshtein edit distance</returns>
		internal static int MatrixLevenshtein(string s, string t){
			int[,] matrix;		  // matrix
			int n = s.Length;	   // length of s
			int m = t.Length;	   // length of t
			int cost;			   // cost

			// Step 1
			if (n == 0) return m;
			if (m == 0) return n;

			// Create matirx
			matrix = new int[n + 1, m + 1];

			// Step 2
			// Initialize
			for (int i = 0; i <= n; i++) matrix[i, 0] = i;
			for (int j = 0; j <= m; j++) matrix[0, j] = j;

			// Step 3
			for (int i = 1; i <= n; i++)
			{

				// Step 4
				for (int j = 1; j <= m; j++)
				{

					// Step 5
					if (s[i - 1] == t[j - 1])
						cost = 0;
					else
						cost = 1;

					// Step 6
					// Find minimum
					int min = matrix[i - 1, j] + 1;
					int b = matrix[i, j - 1] + 1;
					int c = matrix[i - 1, j - 1] + cost;

					if (b < min) min = b;
					if (c < min) min = c;

					matrix[i, j] = min;
				}
			}

			// Step 7
			return matrix[n, m];
		}


		/// <summary>
		/// Compute Levenshtein distance
		/// Jagged array matrix version
		/// </summary>
		/// <returns>Levenshtein edit distance</returns>
		internal static int JaggedLevenshtein(string s, string t){
			int[][] matrix;		 // matrix
			int n = s.Length;	   // length of s
			int m = t.Length;	   // length of t
			int cost;			   // cost

			// Step 1
			if (n == 0) return m;
			if (m == 0) return n;

			// Create matirx
			matrix = new int[n + 1][];


			// Step 2
			// Initialize
			for (int i = 0; i <= n; i++)
			{
				matrix[i] = new int[m + 1];
				matrix[i][0] = i;
			}
			for (int j = 0; j <= m; j++) matrix[0][j] = j;

			// Step 3
			for (int i = 1; i <= n; i++)
			{

				// Step 4
				for (int j = 1; j <= m; j++)
				{

					// Step 5
					if (s[i - 1] == t[j - 1])
						cost = 0;
					else
						cost = 1;

					// Step 6
					// Find minimum
					int min = matrix[i - 1][j] + 1;
					int b = matrix[i][j - 1] + 1;
					int c = matrix[i - 1][j - 1] + cost;

					if (b < min) min = b;
					if (c < min) min = c;

					matrix[i][j] = min;
				}
			}

			// Step 7
			return matrix[n][m];
		}


		/// <summary>
		/// this is not needed for a couple of reasons:
		/// - string .Length is an Int32
		/// - not all the 32 bits of the .Length are used, some are control bits,
		///   so you end up with 29 bits
		/// - CLR limits *all* reference type objects to 2GB of memory (on both 32 and 64 OS)
		/// </summary>
		private static void TestStringLength(string s, string t)
		{
			// Test string length
			if (Math.Max(s.Length, t.Length) > Math.Pow(2, 31))
				throw new Exception("\nMaximum string length in Levenshtein.LD is "
						+ Math.Pow(2, 31)
						+ ".\nYours is "
						+ Math.Max(s.Length, t.Length) + ".");

		}
	} // class EditDistance
}