using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Infrastructure.Extensions
{
	public static class IntExtensions
	{
		/// <summary>
		/// convert to bool
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static bool ToBool(this int self)
		{
			if (self.Equals(0))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

	}
}
